﻿using System;
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
				.If(Modifiers.Any(), b => b.Join(Modifiers.Distinct().Select(m => m.ToString()), " ").Append(" "))
				.Append("enum ")
				.Append(Name)
				.Append(" {")
				.Indent() // TODO: Implement generic, also remove hard-coded indent here
				.NewLine()
				.Join(Members.Select(m => m.Key + " = \"" + m.Value + "\","), Environment.NewLine + builder.Indentation)
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