using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript
{
    public class Property : Element, IMemberOf<Class>, IMemberOf<Interface>
    {
        public Property(string name)
        {
            Name = name;
        }

        public Property(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public ICollection<IModifier<Property>> Modifiers { get; } = new List<IModifier<Property>>();

        public string Name { get; set; }

        public string Type { get; set; }

        public Getter Get { get; set; }

        public Setter Set { get; set; }

        public string Value { get; set; }

        public bool HasSetter => Set != null;

        public bool HasGetter => Get != null;

        public bool HasType => Type != null;

        public bool HasValue => Value != null;

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (parent == null)
            {
                builder.Append("var ").Append(Name);

                if (HasType) builder.Append(": ").Append(Type);
                if (HasValue) builder.Append(" = ").Append(Value);

                builder.Append(";");
            }

            if (HasGetter && parent is Class)
                builder.Append(Get, this);

            if (HasGetter && HasSetter) builder.NewLine();

            if (HasSetter && parent is Class) builder.Append(Set, this);

            if (!HasSetter && !HasGetter && parent != null || parent is Interface)
            {
                var modifiers = Modifiers.Distinct().OrderBy(m => !(m is IAccessor)).Select(m => m.ToString());
                builder.Join(modifiers, " ");

                if (Modifiers.Any()) builder.Append(' ');

                builder.Append(Name);

                if (HasType) builder.Append(": ").Append(Type);
                if (HasValue) builder.Append(" = ").Append(Value);

                builder.Append(";");
            }

            return builder;
        }
    }
}