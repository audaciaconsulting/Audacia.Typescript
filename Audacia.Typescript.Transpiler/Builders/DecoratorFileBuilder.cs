using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    public class DecoratorFileBuilder : FileBuilder
    {
        public override void Build(Transpilation context)
        {
            if (Assembly == null)
                throw new InvalidDataException("Please specify an assembly.");

            var name = string.Join(".", Assembly.GetName().Name
                           .TrimEnd()
                           .Split('.')
                           .Select(s => s.CamelCase()))
                       + ".decorators.ts";

            File = new TypescriptFile { Path = Path.Combine(context.Path, name) };
            File.Elements.Add(new Comment("This file is generated from Audacia.Typescript.Transpiler. Any changes will be overwritten. \n"));

            foreach (var type in Types)
            {
                File.Elements.Add(new ClassDecoratorFunction(type.DecoratorName(),
                    type.Name));
            }
        }

        public override IEnumerable<Type> Dependencies => Types;

        public override IEnumerable<Type> IncludedTypes => Types;

        public override IEnumerable<Type> ClassAttributeDependencies => Enumerable.Empty<Type>();
    }
}