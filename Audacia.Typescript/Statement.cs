namespace Audacia.Typescript
{
    public class Statement : Element, IMemberOf<Function>
    {
        public string Code { get; set; }

        public Statement(string code)
        {
            Code = code;
        }

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            builder.Append(Code);

            if (!string.IsNullOrWhiteSpace(Comment))
                builder.Append(new Comment(Comment), this);

            return builder;
        }
    }
}