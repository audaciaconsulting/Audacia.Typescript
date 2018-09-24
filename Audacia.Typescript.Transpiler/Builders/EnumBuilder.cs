using System;
using System.Collections.Generic;
using System.Linq;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;

namespace Audacia.Typescript.Transpiler.Builders 
{
    public class EnumBuilder : TypeBuilder
    {
        public EnumBuilder(Type sourceType, InputSettings settings, XmlDocumentation documentation) 
            : base(sourceType, settings, documentation) { }

        public override IEnumerable<Type> Dependencies { get; } = Enumerable.Empty<Type>();

        public override Element Build()
        {
            var @enum = new Enum(SourceType.Name) { Modifiers = { Modifier.Export }};
            
            var classDocumentation = Documentation.ForClass(SourceType);
            if (classDocumentation != null)
                @enum.Comment = classDocumentation.Summary;
            
            var values = (int[]) System.Enum.GetValues(SourceType);
			
            foreach (var val in values)
            {
                var name = System.Enum.GetName(SourceType, val);
				
                var attribute = SourceType.GetMember(name)
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