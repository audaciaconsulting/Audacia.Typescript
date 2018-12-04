using System;
using System.Linq;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Documentation;

namespace Audacia.Typescript.Transpiler.Builders 
{
    public class EnumBuilder : TypeBuilder
    {
        public EnumBuilder(Type sourceType, InputSettings settings, XmlDocumentation documentation) 
            : base(sourceType, settings, documentation) { }

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

                var enumMemberAttribute = SourceType.GetMember(name)
                    .Single()
                    .GetCustomAttributes(true)
                    .FirstOrDefault(a => a.GetType().Name == "System.Runtime.Serialization.EnumMemberAttribute");
                
                var label = enumMemberAttribute?.GetType()
                    .GetProperty("Value")
                    .GetValue(enumMemberAttribute)
                    .ToString();

                // No EnumMemberAttribute, try for a DisplayAttribute instead.
                if (label == null)
                {
                    var displayAttribute = SourceType.GetMember(name)
                        .Single()
                        .GetCustomAttributes(true)
                        .FirstOrDefault(a => a.GetType().FullName == "System.ComponentModel.DataAnnotations.DisplayAttribute");

                    label = displayAttribute?.GetType()
                        .GetProperty("Name")
                        .GetValue(displayAttribute)
                        .ToString();
                }
                
                @enum.Members.Add(name, label ?? name);
            }

            ReportProgress(ConsoleColor.DarkYellow, "enum", @enum.Name);
            return @enum;
        }
    }
}