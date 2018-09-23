using System;
using System.Linq;

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

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            if (string.IsNullOrWhiteSpace(Text)) return builder;

            var lines = Text.Split('\r', '\n') //.SelectMany(t => t.Split('\n'))
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(s => s.Trim(' ', '\t'))
                .ToList();

            if (string.IsNullOrWhiteSpace(lines.Last()))
                lines.Remove(lines.Last());

            if (string.IsNullOrWhiteSpace(lines.First()))
                lines.Remove(lines.First());

            if (parent == null)
            {
                var text = string.Join(Environment.NewLine, lines.Select(s => "// " + s));
                return builder.Append(text);
            }
            else // Treat these as jsdoc comments. 
            {
                var text = "//** " + string.Join(" ", lines);
                return builder.Append(text);
            }
        }
    }
}