using System;
using System.Collections.Generic;
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

        public Type Inherits => SourceType.BaseType != typeof(object) && !SourceType.BaseType.Namespace.StartsWith(nameof(System)) ? SourceType.BaseType : null;
        
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
    }
}