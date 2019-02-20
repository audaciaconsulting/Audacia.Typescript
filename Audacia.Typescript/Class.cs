using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Collections;

namespace Audacia.Typescript
{
    /// <summary>A typescript class.</summary>
    public class Class : Element, IEnumerable<IMemberOf<Class>>, IGeneric
    {
        /// <summary>The identifier of the class.</summary>
        public string Name { get; set; }

        public IList<string> Implements { get; } = new List<string>();

        public string Extends { get; set; }

        private Class() { }

        public Class(string name) => Name = name;

        public TypeArgumentList TypeArguments { get; } = new TypeArgumentList();

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

        public void Add(IMemberOf<Class> member) => Members.Add(member);

        public IEnumerator<IMemberOf<Class>> GetEnumerator() => Members.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (!string.IsNullOrWhiteSpace(Comment))
                builder.Append(new Comment(Comment), this).NewLine();

            var modifiers = Modifiers.Distinct().OrderBy(m => !(m is IAccessor)).Select(m => m.ToString());
            builder.Join(modifiers, " ");

            if (Modifiers.Any()) builder.Append(' ');

            builder.Append("class ").Append(Name);

            if (TypeArguments.Any())
            {
                builder.Append('<');
                builder.Join(TypeArguments, this, ", ");
                builder.Append('>');
            }

            if (!string.IsNullOrWhiteSpace(Extends)) builder.Append(" extends ").Append(Extends);

            if (Implements.Any()) builder.Append(" implements ").Join(Implements.Distinct(), ", ");

            builder.Append(" {");

            if (Members.Any()) builder.Indent().NewLine();
            else return builder.Append(" }");

            if (Properties.Any())
                builder.Join(Properties, this, Environment.NewLine + builder.Indentation);

            var functionMembers = Constructors
                .Concat<IElement>(Functions)
                .Concat(PropertyAccessors)
                .ToArray();

            if (Properties.Any() && functionMembers.Any())
                builder.NewLine().NewLine();

            foreach (var element in functionMembers)
            {
                builder.Append(element, this);

                if (element != functionMembers.Last())
                    builder.NewLine().NewLine();
            }

            return builder.Unindent()
                .NewLine()
                .Append("}");
        }
    }
}