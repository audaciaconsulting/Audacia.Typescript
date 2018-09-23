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
			if (Modifiers.Any())
				builder.Join(Modifiers.Distinct().Select(m => m.ToString()), " ").Append(" ");
			
			return builder
				.Append("enum ")
				.Append(Name)
				.Append(" {")
				.Indent()
				.NewLine()
				.Join(Members.Select(m => m.Key + " = \"" + m.Value + "\""), "," + Environment.NewLine + builder.Indentation)
				.Unindent()
				.NewLine()
				.Append("}");
		}

		public IEnumerator GetEnumerator() => ((IEnumerable) Members).GetEnumerator();
	}

	public abstract class Enum : Element
	{
		public string Name { get; set; }
        
		public IList<IModifier<Enum>> Modifiers { get; } = new List<IModifier<Enum>>();
	}
}