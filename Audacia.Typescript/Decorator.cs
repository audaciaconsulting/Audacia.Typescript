using System.Collections.Generic;

namespace Audacia.Typescript
{
    public class Decorator : Element,
        IMemberOf<Class>,
        IMemberOf<Interface>,
        IMemberOf<Property>,
        IMemberOf<Function>
    {
        public Decorator(string name)
        {
            Name = name;
        }

        public Decorator(string name, string[] arguments)
        {
            Name = name;

            foreach (var argument in arguments)
                Arguments.Add(argument);
        }

        public string Name { get; set; }

        public IList<string> Arguments { get; set; }

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent) =>
            builder.Append('@')
                .Append(Name)
                .Append('(')
                .Join(Arguments, ", ")
                .Append(')');
    }
}