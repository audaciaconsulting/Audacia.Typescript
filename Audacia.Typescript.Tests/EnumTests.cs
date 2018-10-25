using System.IO;
using FluentAssertions;
using Xunit;

namespace Audacia.Typescript.Tests
{
    public class EnumTests
    {
        [Fact]
        public void Returns_a_correctly_formed_string_enum()
        {
            var expected = File.ReadAllText("stringEnum.ts");
            var @enum = new Enum("IceCream")
            {
                Modifiers = {Modifier.Export},
                Members =
                {
                    {"vanilla", "\"Vanilla\""},
                    {"chocolate", "\"Chocolate\""},
                    {"strawberry", "\"Strawberry\""}
                }
            };

            var output = @enum.ToString();
            output.Should().Be(expected);
        }
        [Fact]
        public void Returns_a_correctly_formed_number_enum()
        {
            var expected = File.ReadAllText("numberEnum.ts");
            var @enum = new Enum("IceCream")
            {
                Modifiers = {Modifier.Export},
                Members =
                {
                    {"vanilla", 100},
                    {"chocolate", 200},
                    {"strawberry", 300}
                }
            };

            var output = @enum.ToString();
            output.Should().Be(expected);
        }
    }
}