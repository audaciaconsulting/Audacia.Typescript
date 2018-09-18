using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Templating.Typescript.Build.Templates {
    public abstract class Template
    {
        public Type Type { get; }
        public IEnumerable<Settings> Settings { get; }
        
        public Template(Type type, IEnumerable<Settings> settings)
        {
            Type = type;
            Settings = settings;
        }

        public abstract IEnumerable<Type> Dependencies { get; }
        
        public abstract Element Build(IEnumerable<Template> context);

        protected void ReportProgress(ConsoleColor color, string type, string name)
        {
            Console.ForegroundColor = color;
            Console.Write(type + ' ');
            Console.ResetColor();
            Console.WriteLine(name);
        }
        
        protected static IEnumerable<Property> Members(Type type)
        {
            return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Property)
                .Select(m => m as PropertyInfo)
                .Select(m => new Property(ToCamelCase(m.Name), GetTypeName(m.PropertyType)));
        }

        protected static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length < 2) return s.ToLowerInvariant();
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        protected static string GetTypeName(Type t)
        {
            if (t.IsPrimitive)
            {
                if (t == typeof(bool)) return "boolean";
                if (t == typeof(char)) return "string";
                return "number";
            }

            if (t == typeof(decimal)) return "number";
            if (t == typeof(string)) return "string";
            if (t == typeof(Guid)) return "string";
            if (t.IsArray)
            {
                var at = t.GetElementType();
                return GetTypeName(at) + "[]";
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t))
            {
                var collectionType = t.GetGenericArguments()[0]; // all my enumerables are typed, so there is a generic argument
                return GetTypeName(collectionType) + "[]";
            }

            if (Nullable.GetUnderlyingType(t) != null)
            {
                return GetTypeName(Nullable.GetUnderlyingType(t));
            }

            if (t.IsEnum) return t.Name;

            return t.Name;
        }
    }
}