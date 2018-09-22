namespace Audacia.Typescript
{
    public class Comment : Element,
        IMemberOf<Class>,
        IMemberOf<Interface>,
        IMemberOf<Property>,
        IMemberOf<Function>,
        IMemberOf<Accessor>
    {
        public Comment(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
        
        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent) => builder
            .Append("// ")
            .Append(Text);
    }
}