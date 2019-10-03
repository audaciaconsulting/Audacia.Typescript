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
            Documentation = Input.Documentation;
            Dependencies = SourceType.Dependencies();
            ClassAttributeDependencies = SourceType.ClassAttributeDependencies();
            PropertyAttributeDependencies = SourceType.PropertyAttributeDependencies();
        }

        public XmlDocumentation Documentation { get; }

        public IEnumerable<Type> Dependencies { get; }

        public IEnumerable<Type> ClassAttributeDependencies { get; }

        public IEnumerable<Type> PropertyAttributeDependencies { get; }

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
    }
}