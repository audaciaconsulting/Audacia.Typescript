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

        public string Name { get; set; }

        public string Type { get; set; }

        public Getter Get { get; set; }

        public Setter Set { get; set; }

        public bool HasSetter => Set != null;

        public bool HasGetter => Get != null;

        public bool HasType => Type != null;

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent) => builder
            .If(parent == null, b => b
                .AppendIndentation()
                .Append("var ")
                .Append(Name)
                .If(HasType, t => t
                    .Append(": ")
                    .Append(Type))
                .Append(";"))
            .If(HasGetter && parent is Class, b => b.Append(Get, this))
            .If(HasSetter && parent is Class, b => b.Append(Set, this))
            .If(!HasSetter && !HasGetter && parent != null || parent is Interface, b => b
                .AppendIndentation()
                .Append(Name)
                .If(HasType, t => t
                    .Append(": ")
                    .Append(Type))
            .Append(";"));
    }
}
