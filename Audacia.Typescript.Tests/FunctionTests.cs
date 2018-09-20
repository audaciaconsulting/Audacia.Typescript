using System.IO;
using FluentAssertions;
using Xunit;

namespace Audacia.Typescript.Tests
{
    public class FunctionTests
    {
        [Fact]
        public void Returns_a_correctly_formed_function()
        {
            var expected = File.ReadAllText("function.ts");
            var function = new Function("address")
            {
                Modifiers = { Modifier.Export },
                Arguments = { { "streetName", "string" } },
                Statements =
                {
                    "return ('College Name:' + this.name + ' City: ' + this.city + ' Street Name: ' + streetName);"
                }
            };

            function.ToString().Should().Be(expected);
        }
    }
}