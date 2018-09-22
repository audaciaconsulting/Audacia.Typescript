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

        public ArgumentList Arguments { get; } = new ArgumentList();

        public FunctionMemberList Statements { get; } = new FunctionMemberList();

        public IList<IModifier<Function>> Modifiers { get; } = new List<IModifier<Function>>();

        public void Add(string code) => Statements.Add(new Statement(code));

        public void Add(string code, string type) => Arguments.Add(new Argument(code, type));

        public void Add(IMemberOf<Function> member) => Statements.Add(member);

        IEnumerator<IMemberOf<Function>> IEnumerable<IMemberOf<Function>>.GetEnumerator() => Statements.GetEnumerator();

        public IEnumerator<Argument> GetEnumerator() => Arguments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent) => builder
            .If(!string.IsNullOrWhiteSpace(Comment), b => b
                .Append(new Comment(Comment), this).NewLine())
            .Join(Modifiers, ' ')
            .If(Modifiers.Any(), b => b.Append(' '))
            .If(parent == null || parent is Function || parent is Accessor, b => b.Append("function "))
            .Append(Name)
            .Append('(')
            .Join(Arguments, this, ", ")
            .Append(")")
            .If(!string.IsNullOrWhiteSpace(Type), b => b
                .Append(": ")
                .Append(Type))
            .If(!(parent is Interface) && !Modifiers.Contains(Modifier.Abstract), b => b
                .Append(" {")
                .Indent()
                .NewLine()
                .Join(Statements, this, Environment.NewLine + builder.Indentation)
                .Unindent()
                .NewLine()
                .Append('}'))
            .If(parent is Interface || Modifiers.Contains(Modifier.Abstract), b => b
                .Append(";"));
    }
}
