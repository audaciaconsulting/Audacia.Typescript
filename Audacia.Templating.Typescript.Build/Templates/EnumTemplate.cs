using System;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Templating.Typescript.Build.Templates 
{
    public class EnumTemplate : Template
    {
        public EnumTemplate(Type type, IEnumerable<Settings> settings) : base(type, settings) { }

        public override IEnumerable<Type> Dependencies { get; } = Enumerable.Empty<Type>();

        public override Element Build(IEnumerable<Template> context)
        {
            var @enum = new Enum<string>(Type.Name){Modifiers = { Modifier.Export }};
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