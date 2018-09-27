using System;
using System.Collections.Generic;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;

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
            this.Documentation = documentation;
        }

        public XmlDocumentation Documentation { get; set; }

        public abstract IEnumerable<Type> Dependencies { get; }
        
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