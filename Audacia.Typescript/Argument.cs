namespace Audacia.Typescript
{
    public class Argument : Element
    {
        public string Name { get; }

        public string Type { get; }

        public bool HasType => Type != null;

        public Argument(string name, string type) : this(name) => Type = type;

        public Argument(string name) => Name = name;

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            builder.Append(Name);
            
            if (HasType) builder.Append(": ").Append(Type);

            return builder;
        }
    }
}