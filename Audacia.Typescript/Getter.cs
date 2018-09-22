using System;

namespace Audacia.Typescript
{
    public class Getter : Accessor
    {
        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            var property = (Property)parent;

            return builder
                .Append("get ")
                .Append(property.Name)
                .Append("()")
                .If(property.HasType, b => b
                    .Append(": ")
                    .Append(property.Type))
                .Append(" {")
                .Indent()
                .NewLine()
                .Join(Statements, this, Environment.NewLine+ builder.Indentation)
                .Unindent()
                .NewLine()
                .Append('}');
        }
    }
}