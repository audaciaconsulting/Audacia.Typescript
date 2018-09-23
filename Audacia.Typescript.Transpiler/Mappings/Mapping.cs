using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Mappings {
    public abstract class Mapping
    {
        public Type Type { get; }
        
        public Settings Settings { get; }
        
        public Mapping(Type type, Settings settings)
        {
            Type = type;
            Settings = settings;
        }

        public abstract IEnumerable<Type> Dependencies { get; }
        
        public abstract Element Build();

        public static Mapping Create(Type type, Settings settings)
        {
            if (type.IsClass) return new ClassMapping(type, settings);
            if (type.IsInterface) return new InterfaceMapping(type, settings);
            if (type.IsEnum) return new EnumMapping(type, settings);
            
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