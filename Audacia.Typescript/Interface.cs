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

        public Interface(string name) => Name = name;

        public void Add(IMemberOf<Interface> member) => Members.Add(member);

        public IList<IMemberOf<Interface>> Members { get; } = new List<IMemberOf<Interface>>();

        public IList<IModifier<Class>> Modifiers { get; } = new List<IModifier<Class>>();

        public IEnumerator<IMemberOf<Interface>> GetEnumerator() => Members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (!string.IsNullOrWhiteSpace(Comment))
                builder.Append(new Comment(Comment), this).NewLine();

            builder.Join(Modifiers, ' ');

            if (Modifiers.Any()) builder.Append(' ');
            
            builder.Append("interface ").Append(Name);
            
            if (Extends.Any()) builder.Append(" extends ").Join(Extends, ", ");

            return builder.Append(" {")
                .NewLine()
                .Indent()
                .Join(Members, this, Environment.NewLine + builder.Indentation)
                .Unindent()
                .NewLine()
                .Append("}");
        }
    }
}