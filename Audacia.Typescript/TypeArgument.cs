namespace Audacia.Typescript
{
    public class TypeArgument : Element
    {
        public TypeArgument(string name) => Name = name;

        public TypeArgument(string name, string extends) : this(name) => Extends = extends;

        public string Name { get; }

        public string Extends { get; }

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            builder.Append(Name);

            if (Extends != null)
                builder.Append(" extends ").Append(Extends);

            return builder;
        }
    }
}