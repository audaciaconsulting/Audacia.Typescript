using System;
using System.Collections;
using System.Linq;

namespace Audacia.Typescript.Transpiler.Extensions
{
    public static class TypeExtensions
    {
        private static Type[] ArrayTypes =
        {
            typeof(IEnumerable)
        };
        
        public static string CamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length < 2) return s.ToLowerInvariant();
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
        
        public static string TypescriptName(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
                return Nullable.GetUnderlyingType(type).TypescriptName();

            var genericArguments = type.GetGenericArguments();

            // Check built-in types first
            if (type.Namespace.StartsWith(nameof(System)))
            {
                if (type.IsArray)
                {
                    var at = type.GetElementType();
                    return at.TypescriptName() + "[]";
                }

                var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                if (genericArguments.Any() && isEnumerable)
                {
                    var collectionType = type.GetGenericArguments()[0];
                    return collectionType.TypescriptName() + "[]";
                }
                
                if (type.IsPrimitive)
                {
                    if (type == typeof(bool)) return "boolean";
                    if (type == typeof(char)) return "string";
                    return "number";
                }

                if (type == typeof(decimal)) return "number";
                if (type == typeof(string)) return "string";
                if (type == typeof(Guid)) return "string";
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