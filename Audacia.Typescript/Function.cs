using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Collections;

namespace Audacia.Typescript
{
    public class Function : Element,
        IMemberOf<Class>,
        IMemberOf<Interface>,
        IEnumerable<Argument>,
        IMemberOf<Function>,
        IEnumerable<IMemberOf<Function>>
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public Function(string name) => Name = name;

        public Function(string name, string type) : this(name) => Type = type;

        public IList<Decorator> Decorators { get; } = new List<Decorator>();

        public ArgumentList Arguments { get; } = new ArgumentList();

        public TypeArgumentList TypeArguments { get; } = new TypeArgumentList();

        public FunctionMemberList Statements { get; } = new FunctionMemberList();

        public IList<IModifier<Function>> Modifiers { get; } = new List<IModifier<Function>>();

        public void Add(string code) => Statements.Add(new Statement(code));

        public void Add(string code, string type) => Arguments.Add(new Argument(code, type));

        public void Add(IMemberOf<Function> member) => Statements.Add(member);

        IEnumerator<IMemberOf<Function>> IEnumerable<IMemberOf<Function>>.GetEnumerator() => Statements.GetEnumerator();

        public IEnumerator<Argument> GetEnumerator() => Arguments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
        {
            builder.Join(Decorators, b => b.NewLine());
            
            if (!string.IsNullOrWhiteSpace(Comment))
                builder.Append(new Comment(Comment), this).NewLine();

            builder.Join(Modifiers.Select(m => m.ToString()), " ");

            if (Modifiers.Any()) builder.Append(' ');

            if (parent == null || parent is Function || parent is Accessor)
                builder.Append("function ");

            builder.Append(Name);

            if (TypeArguments.Any())
            {
                builder.Append('<');
                builder.Join(TypeArguments, this, ", ");
                builder.Append('>');
            }

            builder.Append('(')
                .Join(Arguments, this, ", ")
                .Append(")");

            if (!string.IsNullOrWhiteSpace(Type)) builder.Append(": ").Append(Type);

            if (!(parent is Interface) && !Modifiers.Contains(Modifier.Abstract))
            {
                if (Statements.Any())
                {
                    builder.Append(" {")
                        .Indent()
                        .NewLine()
                        .Join(Statements, this, Environment.NewLine + builder.Indentation)
                        .Unindent()
                        .NewLine()
                        .Append('}');
                }
                else builder.Append(" { }");
            }
            else builder.Append(";");

            return builder;
        }
    }
}