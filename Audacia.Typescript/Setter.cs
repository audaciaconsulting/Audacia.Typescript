using System;

namespace Audacia.Typescript
{
    /// <summary>The setter for a specific <see cref="Property"/>.</summary>
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
            
            builder.Append("set ")
                .Append(property.Name)
                .Append('(')
                .Append(ArgumentName);

            if (property.HasType) builder.Append(": ").Append(property.Type);
                    
            return builder.Append(") {")
                .NewLine()
                .Indent()
                .Join(Statements, this, Environment.NewLine + builder.Indentation)
                .Unindent()
                .NewLine()
                .Append('}');
        }
    }
}