using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Collections;

namespace Audacia.Typescript
{
    public class Class : Element, IEnumerable<IMemberOf<Class>>
    {
        public string Name { get; }

        public IList<string> Implements { get; } = new List<string>();

        public string Extends { get; set; }

        public Class(string name)
        {
            Name = name;
        }

        public ClassMemberList Members { get; } = new ClassMemberList();

        public IEnumerable<Property> Properties => Members.OfType<Property>()
            .Where(p => !p.HasGetter && !p.HasSetter)
            .ToArray();

        public IEnumerable<Property> PropertyAccessors => Members.OfType<Property>()
            .Where(p => p.HasGetter || p.HasSetter)
            .ToArray();

        public IEnumerable<Constructor> Constructors => Members.OfType<Constructor>().ToArray();

        public IEnumerable<Function> Functions => Members.OfType<Function>()
            .Where(m => !(m is Constructor))
            .ToArray();

        public IList<IModifier<Class>> Modifiers { get; } = new List<IModifier<Class>>();

        public void Add(IMemberOf<Class> member)
        {
            Members.Add(member);
        }

        public IEnumerator<IMemberOf<Class>> GetEnumerator() => Members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            var memberWritten = false;

            if (!string.IsNullOrWhiteSpace(Comment))
                builder.Append(new Comment(Comment), this).NewLine();

            builder.Join(Modifiers.Distinct().Select(m => m.ToString()), " ");

            if (Modifiers.Any()) builder.Append(' ');

            builder.Append("class ").Append(Name);

            if (!string.IsNullOrWhiteSpace(Extends)) builder.Append(" extends ").Append(Extends);

            if (Implements.Any()) builder.Append(" implements ").Join(Implements.Distinct(), ", ");

            builder.Append(" {")
                .Indent()
                .NewLine();

            if (Properties.Any())
            {
                builder.Join(Properties, this, Environment.NewLine + builder.Indentation);

                memberWritten = true;
            }

            if (Constructors.Any())
            {
                if (memberWritten) builder.NewLine().NewLine();

                builder.Join(Constructors, this, Environment.NewLine);

                memberWritten = true;
            }

            if (Functions.Any())
            {
                if (memberWritten) builder.NewLine().NewLine();

                builder.Join(Functions, this, Environment.NewLine + Environment.NewLine + builder.Indentation);

                memberWritten = true;
            }

            if (PropertyAccessors.Any())
            {
                if (memberWritten) builder.NewLine().NewLine();

                builder.Join(PropertyAccessors, this, Environment.NewLine + builder.Indentation);
            }

            return builder.Unindent()
                .NewLine()
                .Append("}");
        }
    }
}