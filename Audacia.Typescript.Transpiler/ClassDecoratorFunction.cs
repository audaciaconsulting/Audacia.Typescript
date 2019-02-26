namespace Audacia.Typescript.Transpiler
{
    public class ClassDecoratorFunction : DecoratorFunction
    {
        public ClassDecoratorFunction(string name, string argumentsClassName) : base(name, argumentsClassName) { }

        private string prefix = "___attributes";

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
//            export function noun(args: Partial<DisplayNameAttribute>) {
//                return function<T extends { new (...args: any[]): {} }>(ctor: T) {
//                    return class extends ctor {
//                        ___attributes;
//                        constructor(...args: any[]) {
//                            super();
//                            if (!this.___attributes)
//                                this.___attributes = {};
//                            this.___attributes.noun = args;
//                        }
//                    };
//                };
//            }

            return builder.Append("export function ").Append(Identifier).Append("(args: Partial<").Append(ArgumentsClassName).Append(">) {")
                .Indent().NewLine()
                .Append("return function<T extends { new (...args: any[]): {} }>(ctor: T) {")
                .Indent().NewLine()
                .Append("return class extends ctor {")
                .Indent().NewLine()
                .Append(prefix).Append(';')
                .Append("constructor(...args: any[]) {")
                .Indent().NewLine()
                .Append("super();").NewLine()
                .Append("if (!this.").Append(prefix).Append(")")
                .Indent().NewLine()
                .Append("this.").Append(prefix).Append(" = { };")
                .Unindent().NewLine()
                .Append("this.").Append(prefix).Append('.').Append(ShortName).Append(" = args;")
                .Unindent().NewLine().Append('}')
                .Unindent().NewLine().Append("}")
                .Unindent().NewLine().Append("}")
                .Unindent().NewLine().Append('}');
        }
    }
}