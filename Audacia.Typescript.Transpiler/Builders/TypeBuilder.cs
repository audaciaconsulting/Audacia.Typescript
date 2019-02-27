using System;
using System.Collections.Generic;
using Audacia.Typescript.Transpiler.Documentation;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    /// <summary>Maps a CLR Type to a Typescript one.</summary>
    public abstract class TypeBuilder
    {
        public Type SourceType { get; }

        public FileBuilder Input { get; }

        public Transpilation OutputContext { get; }

        public TypeBuilder(Type sourceType, FileBuilder input, Transpilation outputContext)
        {
            SourceType = sourceType;
            Input = input;
            OutputContext = outputContext;
        }

        public XmlDocumentation Documentation => Input.Documentation;

        public IEnumerable<Type> Dependencies => SourceType.Dependencies();

        public IEnumerable<Type> ClassAttributeDependencies => SourceType.ClassAttributeDependencies();

        public IEnumerable<Type> PropertyAttributeDependencies => SourceType.PropertyAttributeDependencies();

        public Type Inherits => SourceType.BaseType !=  null
            && SourceType.BaseType != typeof(object)
            && SourceType.BaseType != typeof(Attribute)
            && !Primitive.CanWrite(SourceType.BaseType) ? SourceType.BaseType : null;

        public abstract Element Build();

        public static TypeBuilder Create(Type type, FileBuilder input, Transpilation output)
        {
            if (type.IsInterface) return new InterfaceBuilder(type, input, output);
            if (type.IsEnum) return new EnumBuilder(type, input, output);

            return new ClassBuilder(type, input, output);
        }

        protected void WriteLine(ConsoleColor color, string type, string name)
        {
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.Write(type + ' ');
            Console.ResetColor();
            Console.Write(name);
        }

        protected void Write(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        protected void Write(string text)
        {
            Console.Write(text);
        }
    }
}