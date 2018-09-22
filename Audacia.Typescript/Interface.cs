using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript
{
    public class Interface : Element, IMemberOf<Function>, IEnumerable<IMemberOf<Interface>>
    {
        public string Name { get; }
        
        public IList<string> Extends { get; } = new List<string>();

        public Interface(string name)
        {
            Name = name;
        }

        public void Add(IMemberOf<Interface> member)
        {
            Members.Add(member);
        }

        public IList<IMemberOf<Interface>> Members { get; } = new List<IMemberOf<Interface>>();

        public IList<IModifier<Class>> Modifiers { get; } = new List<IModifier<Class>>();

        public IEnumerator<IMemberOf<Interface>> GetEnumerator() => Members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            return builder
                .If(!string.IsNullOrWhiteSpace(Comment), b => b
                    .Append(new Comment(Comment), this).NewLine())
                .Join(Modifiers, ' ')
                .If(Modifiers.Any(), b => b
                    .Append(' '))
                .Append("interface ")
                .Append(Name)
                .If(Extends.Any(), b => b
                    .Append(" extends ")
                    .Join(Extends, ", "))
                .Append(" {")
                .NewLine()
                .Indent()
                .Join(Members, this, Environment.NewLine + builder.Indentation)
                .Unindent()
                .NewLine()
                .Append("}");
        }
    }
}