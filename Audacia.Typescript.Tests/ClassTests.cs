using System.IO;
using FluentAssertions;
using Xunit;

namespace Audacia.Typescript.Tests
{
    public class ClassTests
    {
        [Fact]
        public void Returns_a_correctly_formed_class()
        {
            var expected = File.ReadAllText("class.ts");
            var @class = new Class("College")
            {
                new Property("city", "string"),
                new Property("name", "string") { Value = "\"Nigel\"" },
                new Constructor
                {
                    { "city", "string" },
                    "this.city = city;"
                },

                new Function("address")
                {
                    Modifiers = { Modifier.Public },
                    Arguments = { { "streetName", "string" } },
                    Statements = { "return ('College Name:' + this.name + ' City: ' + this.city + ' Street Name: ' + streetName);" }
                }
            };

            @class.ToString().Should().Be(expected);
        }

        [Fact]
        public void Returns_a_correctly_formed_abstract_class()
        {
            var expected = File.ReadAllText("abstractClass.ts");
            var @class = new Class("University")
            {
                Modifiers = { Modifier.Abstract },
                Members =
                {
                    new Property("city", "string"),
                    new Property("name", "string"),
                    new Constructor
                    {
                        { "name", "string" },
                        { "city", "string" },
                        "this.city = city;",
                        "this.name = name;"
                    },

                    new Function("thing", "string") {Modifiers = { Modifier.Abstract }},
                    new Function("address")
                    {
                        Modifiers = { Modifier.Public, Modifier.Abstract },
                        Arguments = { { "streetName", "string" } }
                    },

                    new Function("age", "number")
                    {
                        "return 12;"
                    }}
            };

            @class.ToString().Should().Be(expected);
        }

        [Fact]
        public void Returns_a_correctly_formed_generic_class()
        {
            var expected = File.ReadAllText("generics.ts");
            var @class = new Class("Wrapper")
            {
                TypeArguments = { { "T", "{ length:string, name:string }" }, "T2" },
                Members =
                {
                    new Property("value", "T"),
                    new Property("otherValue", "T2"),
                    new Function("longest")
                    {
                        Modifiers = { Modifier.Public },
                        TypeArguments = { { "TOther", "Wrapper<T, T2>" } },
                        Arguments = { { "input", "TOther" } },
                        Type = "Wrapper<T, T2>",
                        Statements =
                        {
                            "if (this.value.length > input.value.length) return this;",
                            "else return input;"
                        }
                    }
                }
            };

            @class.ToString().Should().Be(expected);
        }
    }
}