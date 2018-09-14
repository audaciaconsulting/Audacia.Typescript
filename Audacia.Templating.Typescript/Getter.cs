using System;

namespace Audacia.Templating.Typescript
{
    public class Getter : Accessor
    {
        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            var property = (Property)parent;

            return builder
                .AppendIndentation()
                .Append("get ")
                .Append(property.Name)
                .Append("()")
                .If(property.HasType, b => b
                    .Append(": ")
                    .Append(property.Type))
                .AppendLine(" {")
                .Indent()
                .Join(Statements, this, Environment.NewLine)
                .Unindent()
                .AppendLine()
                .AppendIndentation()
                .AppendLine('}');
        }
    }
}