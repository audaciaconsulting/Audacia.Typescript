using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    /// <summary>Maps a CLR Type to a Typescript one.</summary>
    public abstract class TypeBuilder
    {
        public Type SourceType { get; }

        protected InputSettings Settings { get; }

        public TypeBuilder(Type sourceType, InputSettings settings, XmlDocumentation documentation)
        {
            SourceType = sourceType;
            Settings = settings;
            Documentation = documentation;
        }

        public XmlDocumentation Documentation { get; set; }

        public IEnumerable<Type> Dependencies => SourceType.Dependencies();

        public Type Inherits => SourceType.BaseType !=  null
            && SourceType.BaseType != typeof(object)
            && !SourceType.BaseType.Namespace.StartsWith(nameof(System)) ? SourceType.BaseType : null;

        public abstract Element Build();

        public static TypeBuilder Create(Type type, InputSettings settings, XmlDocumentation documentation)
        {
            if (type.IsClass) return new ClassBuilder(type, settings, documentation);
            if (type.IsInterface) return new InterfaceBuilder(type, settings, documentation);
            if (type.IsEnum) return new EnumBuilder(type, settings, documentation);

            throw new ArgumentOutOfRangeException();
        }

        protected void ReportProgress(ConsoleColor color, string type, string name)
        {
            Console.ForegroundColor = color;
            Console.Write(type + ' ');
            Console.ResetColor();
            Console.WriteLine(name);
        }

        protected string TypescriptName(Type type)
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
                if (type == typeof(TimeSpan)) return "string";
                if (type == typeof(DateTime)) return "Date";
                if (type == typeof(DateTimeOffset)) return "Date";

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