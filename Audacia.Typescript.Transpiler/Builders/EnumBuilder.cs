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
            var @enum = new Enum(SourceType.Name) {Modifiers = {Modifier.Export}};

            var classDocumentation = Documentation.ForClass(SourceType);
            if (classDocumentation != null)
                @enum.Comment = classDocumentation.Summary;

            var values = (int[]) System.Enum.GetValues(SourceType);

            foreach (var val in values)
            {
                object value;
                var name = System.Enum.GetName(SourceType, val);

                var member = SourceType.GetMember(name);
                //If we've specified we want number enum values, just add the value it finds.
                if (Settings.EnumSettings?.ValueType == EnumValueType.Number)
                {
                    value = val;
                }
                //Use string enum values by default
                else
                {
                    var enumMemberAttribute = member.Single()
                        .GetCustomAttributes(true)
                        .FirstOrDefault(a => a.GetType().FullName == "System.Runtime.Serialization.EnumMemberAttribute");

                    var label = enumMemberAttribute?.GetType()
                        .GetProperty("Value")
                        .GetValue(enumMemberAttribute)
                        ?.ToString();

                    // No EnumMemberAttribute, try for a DisplayAttribute instead.
                    if (label == null)
                    {
                        var displayAttribute = member.Single()
                            .GetCustomAttributes(true)
                            .FirstOrDefault(a => a.GetType().FullName == "System.ComponentModel.DataAnnotations.DisplayAttribute");

                        label = displayAttribute?.GetType()
                            .GetProperty("Name")
                            .GetValue(displayAttribute)
                            ?.ToString();
                    }

                    value = $"\"{label ?? name}\"";
                }

                @enum.Members.Add(name, value);
            }

            ReportProgress(ConsoleColor.DarkYellow, "enum", @enum.Name);
            return @enum;
        }
    }
}