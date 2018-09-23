﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Typescript 
{
	public class Enum : Element
	{
		public string Name { get; set; }
        
		public IList<IModifier<Enum>> Modifiers { get; } = new List<IModifier<Enum>>();
		
		public Enum(string name) => Name = name;

		public IDictionary<string, object> Members { get; } = new Dictionary<string, object>();

		public void Add(string member, object value) => Members.Add(member, value);
		
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
}