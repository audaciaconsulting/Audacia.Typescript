using System;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Transpiler.Configuration;

namespace Audacia.Typescript.Transpiler.Mappings 
{
    public class EnumMapping : TypeMapping
    {
        public EnumMapping(Type type, InputSettings settings) : base(type, settings) { }

        public override IEnumerable<Type> Dependencies { get; } = Enumerable.Empty<Type>();

        public override Element Build()
        {
            var @enum = new Enum(Type.Name) { Modifiers = { Modifier.Export }};
            var values = (int[]) System.Enum.GetValues(Type);
			
            foreach (var val in values)
            {
                var name = System.Enum.GetName(Type, val);
				
                var attribute = Type.GetMember(name)
                    .Single()
                    .GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType().Name == "EnumMemberAttribute");
			
                var label = attribute?.GetType()
                    .GetProperty("Value")
                    .GetValue(attribute)
                    .ToString();
				
                @enum.Members.Add(name, label ?? name);				
            }
			
            ReportProgress(ConsoleColor.DarkYellow, "enum", @enum.Name);
            return @enum;
        }
    }
}