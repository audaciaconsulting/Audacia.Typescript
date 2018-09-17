using System;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Templating.Typescript 
{
	public class Enum<T> : Enum
	{
		public Enum(string name) => Name = name;

		public IDictionary<string, T> Members { get; } = new Dictionary<string, T>();
        
		public override TypescriptBuilder Build(TypescriptBuilder builder, IElement parent)
		{
			return builder
				.AppendIndentation()
				.JoinDistinct(Modifiers.Select(m => m.ToString()), ' ')
				.Append("interface ")
				.Append(Name)
				.Append(" {")
				.AppendLine()
				.Indent() // TODO: Implement generic
				.Join(Members.Select(m => m.Key + ":string = " + m.Value + ','), Environment.NewLine)
				.AppendLine()
				.Unindent()
				.AppendIndentation()
				.AppendLine("}");
		}
	}

	public abstract class Enum : Element
	{
		public string Name { get; set; }
        
		public IList<IModifier<Enum>> Modifiers { get; } = new List<IModifier<Enum>>();
	}
}