namespace Audacia.Templating.Typescript
{
    public abstract class Element : IElement
    {
        public string Comment { get; set; }

        public override string ToString()
        {
            var builder = new TypescriptBuilder();
            Build(builder, null);
            return builder.ToString();
        }

        public abstract TypescriptBuilder Build(TypescriptBuilder builder, IElement parent);
    }
}