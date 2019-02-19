using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> Dependencies(this Type type)
        {
            if (type == typeof(object)) return Enumerable.Empty<Type>();
            if (type == typeof(Enum)) return Enumerable.Empty<Type>();

            var results = new List<Type> { type.BaseType };
            results.AddRange(type.GetGenericDependencies());
            results.AddRange(type.GetDeclaredInterfaces());
            results.AddRange(type.GetDeclaredInterfaces().SelectMany(i => i.GetGenericDependencies()));

            var properties = type
                .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();

            foreach (var property in properties)
            {
                results.Add(property.PropertyType);
                results.AddRange(property.PropertyType.GetGenericDependencies());
            }

            return results
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