namespace Audacia.Typescript.Transpiler
{
    public class PropertyDecoratorFunction : DecoratorFunction
    {
        public PropertyDecoratorFunction(string name, string argumentsClassName) : base(name, argumentsClassName) { }

        private string prefix = "___attributes";

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
//            export function displayName(args: Partial<DisplayNameAttribute>)
//            {
//                return function(target: any, propertyKey: string | symbol) {
//                    if (!target.___attributes)
//                        target.___attributes = {};
//                    if (!target.___attributes.properties)
//                        target.___attributes.properties = {}
//                    if (!target.___attributes.properties[propertyKey])
//                        target.___attributes.properties[propertyKey] = {}
//                    target.___attributes.properties[propertyKey].displayName = args;
//                }
//            }

            return builder.Append("export function ").Append(Identifier).Append("(args: Partial<").Append(ArgumentsClassName).Append(">) {")
                .Indent().NewLine()
                .Append("return function(target: any, propertyKey: string | symbol) {")
                .Indent().NewLine()
                .Append("if (!target.___attributes)")
                .Indent().NewLine()
                .Append("target.___attributes = {};")
                .Unindent().NewLine()
                .Append("if (!target.___attributes.properties)")
                .Indent().NewLine()
                .Append("target.___attributes.properties = {};")
                .Unindent().NewLine()
                .Append("if (!target.___attributes.properties.").Append(ShortName).Append(")")
                .Indent().NewLine()
                .Append("target.___attributes.properties.").Append(ShortName).Append(" = {};")
                .Unindent().NewLine()
                .Append("target.___attributes.properties[propertyKey].").Append(ShortName).Append(" = args;")
                .Unindent().NewLine().Append("}")
                .Unindent().NewLine().Append('}');
        }
    }
}