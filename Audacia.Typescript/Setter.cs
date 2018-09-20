using System;

namespace Audacia.Typescript
{
    public class Setter : Accessor
    {
        public string ArgumentName { get; }

        public Setter(string argumentName)
        {
            ArgumentName = argumentName;
        }

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            var property = (Property) parent;
            return builder
                .AppendIndentation()
                .Append("set ")
                .Append(property.Name)
                .Append('(')
                .Append(ArgumentName)
                .If(property.HasType, b => b
                    .Append(": ")
                    .Append(property.Type))
                .AppendLine(") {")
                .Indent()
                .Join(Statements, this, Environment.NewLine)
                .Unindent()
                .AppendLine()
                .AppendIndentation()
                .AppendLine('}');
        }
    }
}