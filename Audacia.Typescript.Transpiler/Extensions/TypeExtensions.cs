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

            var properties = type
                .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Cast<PropertyInfo>();
            
            foreach (var property in properties)
            {
                results.Add(property.GetType());
                results.AddRange(property.PropertyType.GetGenericDependencies());
            }

            return results;
        }

        private static IEnumerable<Type> GetGenericDependencies(this Type type)
        {
            var results = new List<Type>();
            
            if (!type.ContainsGenericParameters) return results;
            
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
            
            var genericArguments = type.GetGenericArguments();

            // Check built-in types first
            if (type.Namespace.StartsWith(nameof(System)))
            {
                if (type.IsArray)
                {
                    var at = type.GetElementType();
                    return "Array<" + at.TypescriptName() + ">";
                }

                if (type.Name.StartsWith("IDictionary") || type.Name.StartsWith("Dictionary") && genericArguments.Length == 2)
                    return $"Map<{genericArguments[0].TypescriptName()}, {genericArguments[1].TypescriptName()}>";

                var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                if (genericArguments.Any() && isEnumerable)
                {
                    var collectionType = type.GetGenericArguments()[0];
                    return "Array<" +  collectionType.TypescriptName() + ">";
                }

                if (type == typeof(bool)) return "boolean";
                if (type == typeof(char)) return "string";
                if (type == typeof(decimal)) return "number";
                if (type == typeof(string)) return "string";
                if (type == typeof(Guid)) return "string";
                if (type == typeof(DateTime)) return "Date";
                
                if (type.IsPrimitive) return "number";
            }

            if (genericArguments.Any())
            {
                return type.Name.Substring(0, type.Name.Length - 2)
                       + '<'
                       + string.Join(", ", genericArguments.Select(a => a.TypescriptName()))
                       + '>';
            }
            
            //if (type.IsEnum) return type.Name;
            return type.Name;
        }
    }
}