namespace Audacia.Templating.Typescript
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
            return builder
                .AppendIndentation()
                .Append(Code)
                .If(!string.IsNullOrWhiteSpace(Comment), b => b
                    .Append(new Comment(Comment), this));
        }
    }
}