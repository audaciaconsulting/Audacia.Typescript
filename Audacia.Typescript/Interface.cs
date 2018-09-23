using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Collections;

namespace Audacia.Typescript
{
    public class Interface : Element, IMemberOf<Function>, IEnumerable<IMemberOf<Interface>>
    {
        public string Name { get; }

        public IList<string> Extends { get; } = new List<string>();

        public Interface(string name) => Name = name;

        public void Add(IMemberOf<Interface> member) => Members.Add(member);

        public TypeArgumentList TypeArguments { get; } = new TypeArgumentList();
        
        public IList<IMemberOf<Interface>> Members { get; } = new List<IMemberOf<Interface>>();

        public IList<IModifier<Class>> Modifiers { get; } = new List<IModifier<Class>>();

        public IEnumerator<IMemberOf<Interface>> GetEnumerator() => Members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (!string.IsNullOrWhiteSpace(Comment))
                builder.Append(new Comment(Comment), this).NewLine();

            builder.Join(Modifiers.Select(m => m.ToString()), " ");

            if (Modifiers.Any()) builder.Append(' ');
            
            builder.Append("interface ").Append(Name);
        
            if (TypeArguments.Any())
            {
                builder.Append('<');
                builder.Join(TypeArguments, this, ", ");
                builder.Append('>');                
            }
            
            if (Extends.Any()) builder.Append(" extends ").Join(Extends, ", ");

            builder.Append(" {");

            if (Members.Any()) builder.Indent().NewLine();
            else return builder.Append(" }");
            
            return builder
                .Join(Members, this, Environment.NewLine + builder.Indentation)
                .Unindent()
                .NewLine()
                .Append("}");
        }
    }
}