using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript 
{
	public class Enum<T> : Enum, IEnumerable
	{
		public Enum(string name) => Name = name;

		public IDictionary<string, T> Members { get; } = new Dictionary<string, T>();

		public void Add(string member, T value) => Members.Add(member, value);
		
		public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
		{
			return builder
				.AppendIndentation()
				.If(Modifiers.Any(), b => b.JoinDistinct(Modifiers.Select(m => m.ToString()), ' ').Append(' '))
				.Append("enum ")
				.Append(Name)
				.Append(" {")
				.AppendLine()
				.Indent() // TODO: Implement generic, also remove hard-coded indent here
				.Join(Members.Select(m => "    " + (m.Key + " = \"" + m.Value + "\",")), Environment.NewLine)
				.AppendLine()
				.Unindent()
				.AppendIndentation()
				.AppendLine("}");
		}

		public IEnumerator GetEnumerator() => ((IEnumerable) Members).GetEnumerator();
	}

	public abstract class Enum : Element
	{
		public string Name { get; set; }
        
		public IList<IModifier<Enum>> Modifiers { get; } = new List<IModifier<Enum>>();
	}
}