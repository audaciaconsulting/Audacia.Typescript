using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler
{
    public abstract class DecoratorFunction : Element
    {
        protected DecoratorFunction(string name, string argumentsClassName)
        {
            Identifier = name;
            ArgumentsClassName = argumentsClassName;
        }

        public string Identifier { get; set; }

        public string ShortName => ArgumentsClassName.EndsWith("Attribute")
            ? ArgumentsClassName.Substring(0, ArgumentsClassName.Length - 9).CamelCase()
            : ArgumentsClassName.CamelCase();

        public string ArgumentsClassName { get; set; }
    }
}