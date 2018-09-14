using System.IO;
using FluentAssertions;
using Xunit;

namespace Audacia.Templating.Typescript.Tests
{
    public class PropertyTests
    {
        [Fact]
        public void Returns_a_correctly_formed_property()
        {
            var expected = File.ReadAllText("property.ts");
            var property = new Property("luckyNumber", "number");
            property.ToString().Should().Be(expected);
        }
    }
}
