using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> ClassAttributeDependencies(this Type type)
        {
            return type.GetCustomAttributes(false)
                .Where(t => t.GetType().IsPublic)
                .Select(a => a.GetType());
        }

        public static IEnumerable<Type> PropertyAttributeDependencies(this Type type)
        {
            return type
                .GetProperties()
                .SelectMany(p => p.GetCustomAttributes(false))
                .Select(a => a.GetType())
                .Where(t => t.IsPublic)
                .Distinct();
        }

        public static bool IsClassAttribute(this Type type)
        {
            if (type.BaseType == typeof(Attribute)) return false;
            var usage = type.GetCustomAttribute<AttributeUsageAttribute>();
            if (usage == null) return true;
            return usage.ValidOn.HasFlag(AttributeTargets.Enum)
                ||usage.ValidOn.HasFlag(AttributeTargets.Class);
        }
        
        public static bool IsPropertyAttribute(this Type type)
        {
            if (type.BaseType == typeof(Attribute)) return false;
            var usage = type.GetCustomAttribute<AttributeUsageAttribute>();
            if (usage == null) return true;
            return usage.ValidOn.HasFlag(AttributeTargets.Property);
        }
        
        public static IEnumerable<Type> Dependencies(this Type type)
        {
            if (type == typeof(object)) return Enumerable.Empty<Type>();
            if (type == typeof(System.Enum)) return Enumerable.Empty<Type>();
            if (type.BaseType == typeof(System.Enum)) return Enumerable.Empty<Type>();

            var results = new List<Type> { type.BaseType };
            results.AddRange(type.GetGenericDependencies());
            results.AddRange(type.GetDeclaredInterfaces());
            results.AddRange(type.GetDeclaredInterfaces().SelectMany(i => i.GetGenericDependencies()));
            results.AddRange(type.GetCustomAttributesData().Select(a => a.AttributeType)
                .Where(t => t.IsPublic)
                .Where(a => a != type) // We want to instantiate enums on attributes and need to import them to do it
                .SelectMany(a => a.GetProperties().Select(p => p.PropertyType))
                .Where(d => d.IsEnum));

            var properties = type
                .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();

            foreach (var property in properties)
            {
                results.Add(property.PropertyType);
                results.AddRange(property.PropertyType.GetGenericDependencies());
                results.AddRange(property.GetCustomAttributesData().Select(a => a.AttributeType)
                    .Where(t => t.IsPublic)
                    .Where(a => a != type) // We want to instantiate enums on attributes and need to import them to do it
                    .SelectMany(a => a.GetProperties().Select(p => p.PropertyType))
                    .Where(d => d.IsEnum));
            }

            return results
                .Where(r => r != typeof(Attribute))
                .Where(result => result != null)
                .Where(result => result.FullName != null)
                .DistinctBy(result => result.FullName.SanitizeTypeName());
        }

        private static IEnumerable<Type> GetGenericDependencies(this Type type)
        {
            var results = new List<Type>();

            //if (!type.ContainsGenericParameters) return results;

            var generics = type.GetGenericArguments();
            results.AddRange(generics);

            foreach(var generic in type.GetGenericArguments())
                results.AddRange(GetGenericDependencies(generic));

            return results;
        }

        // Filters out runtime generic types that aren't definitions
        public static IEnumerable<Type> Declarations(this IEnumerable<Type> types) =>
            types.Where(type => !types
                .Any(other => type.Namespace == other.Namespace
                              && type.Name.Split('`').First() == other.Name.Split('`').First()
                              && other.GetGenericArguments().Length > type.GetGenericArguments().Length));

        public static IEnumerable<Type> GetDeclaredInterfaces(this Type type)
        {
            var allInterfaces = type.GetInterfaces();

            var baseInterfaces = Enumerable.Empty<Type>();
            if (type.BaseType != null)
            {
                baseInterfaces = type.BaseType.GetInterfaces();
            }
            return allInterfaces.Except(baseInterfaces.Concat(allInterfaces.SelectMany(i => i.GetInterfaces()))).Distinct();

        }

        public static string ClassDecoratorName(this Type type)
        {
            var name = type.Name.EndsWith("Attribute")
                ? type.Name.Substring(0, type.Name.Length - 9).CamelCase()
                : type.Name.CamelCase();

            var usage = type.GetCustomAttribute<AttributeUsageAttribute>();
            if (usage != null && usage.ValidOn.HasFlag(AttributeTargets.Property))
                name += "Type";

            return name;
        }

        public static string PropertyDecoratorName(this Type type)
        {
            var name = type.Name.EndsWith("Attribute")
                ? type.Name.Substring(0, type.Name.Length - 9).CamelCase()
                : type.Name.CamelCase();

            var usage = type.GetCustomAttribute<AttributeUsageAttribute>();
            if (usage != null && (usage.ValidOn.HasFlag(AttributeTargets.Class) || usage.ValidOn.HasFlag(AttributeTargets.Enum)))
                name += "Prop";

            return name;
        }


        public static string TypescriptName(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
                return Nullable.GetUnderlyingType(type).TypescriptName();

            if (type == typeof(object)) return "any";

            var genericArguments =
                type.GetGenericArguments();

            var primitive = Primitive.Identifier(type);
            if (primitive != null) return primitive;

            if (!genericArguments.Any()) return type.Name;

            return type.Name.Substring(0, type.Name.Length - 2)
                   + '<'
                   + string.Join(", ", genericArguments.Select(a => a.TypescriptName()))
                   + '>';

        }
    }
}