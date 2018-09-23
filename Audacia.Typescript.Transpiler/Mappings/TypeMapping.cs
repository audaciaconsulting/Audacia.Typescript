using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings 
{
    /// <summary>Maps a CLR Type to a Typescript one.</summary>
    public abstract class TypeMapping
    {
        public Type SourceType { get; }

        protected InputSettings Settings { get; }
        
        public TypeMapping(Type sourceType, InputSettings settings, XmlDocumentation documentation)
        {
            SourceType = sourceType;
            Settings = settings;
            this.Documentation = documentation;
        }

        public XmlDocumentation Documentation { get; set; }

        public abstract IEnumerable<Type> Dependencies { get; }
        
        public abstract Element Build();

        public static TypeMapping Create(Type type, InputSettings settings, XmlDocumentation documentation)
        {
            if (type.IsClass) return new ClassMapping(type, settings, documentation);
            if (type.IsInterface) return new InterfaceMapping(type, settings, documentation);
            if (type.IsEnum) return new EnumMapping(type, settings, documentation);
            
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