using System;
using System.Linq;
using Audacia.Typescript.Transpiler.Configuration;
using Audacia.Typescript.Transpiler.Extensions;

namespace Audacia.Typescript.Transpiler.Builders
{
    public class EnumBuilder : TypeBuilder
    {
        public EnumBuilder(Type sourceType, FileBuilder input, Transpilation outputContext)
            : base(sourceType, input, outputContext) { }

        public override Element Build()
        {
            var @enum = new Enum(SourceType.Name) {Modifiers = {Modifier.Export}};

            var classDocumentation = Documentation?.ForClass(SourceType);
            if (classDocumentation != null)
                @enum.Comment = classDocumentation.Summary;

            var names= System.Enum.GetNames(SourceType);

            foreach (var name in names)
            {
                object value;

                var member = SourceType.GetMember(name);
                //If we've specified we want number enum values, just add the value it finds.
                if (OutputContext.EnumSettings?.ValueType == EnumValueType.Number)
                {
                    value = System.Enum.Parse(SourceType, name);
                }
                //Use string enum values by default
                else
                {
                    var enumMemberAttribute = member.Single()
                        .GetCustomAttributes(true)
                        .FirstOrDefault(a => a.GetType().FullName == "System.Runtime.Serialization.EnumMemberAttribute");

                    var label = enumMemberAttribute
                        ?.GetType()
                        .GetProperty("Value")
                        ?.GetValue(enumMemberAttribute)
                        ?.ToString();

                    // No EnumMemberAttribute, try for a DisplayAttribute instead.
                    if (label == null)
                    {
                        var displayAttribute = member.Single()
                            .GetCustomAttributes(true)
                            .FirstOrDefault(a => a.GetType().FullName == "System.ComponentModel.DataAnnotations.DisplayAttribute");

                        label = displayAttribute?.GetType()
                            .GetProperty("Name")
                            ?.GetValue(displayAttribute)
                            ?.ToString();
                    }

                    value = $"\"{label ?? name}\"";
                }

                @enum.Members.Add(name.CamelCase(), value);
            }

            WriteLine(ConsoleColor.DarkYellow, "enum", @enum.Name);
            return @enum;
        }
    }
}