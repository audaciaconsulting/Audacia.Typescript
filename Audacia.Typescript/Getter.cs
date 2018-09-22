using System;

namespace Audacia.Typescript
{
    public class Getter : Accessor
    {
        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            var property = (Property) parent;

            builder.Append("get ").Append(property.Name).Append("()");
            
            if (property.HasType) builder.Append(": ").Append(property.Type);

            return builder.Append(" {")
                .Indent()
                .NewLine()
                .Join(Statements, this, Environment.NewLine + builder.Indentation)
                .Unindent()
                .NewLine()
                .Append('}');
        }
    }
}