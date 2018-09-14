using FluentAssertions;
using Xunit;

namespace Audacia.Templating.Typescript.Tests
{
    public class TypescriptFileTests
    {
        [Fact]
        public void Returns_a_correctly_formed_file()
        {
            var expected = System.IO.File.ReadAllText("typescriptFile.ts");
            var file = new TypescriptFile
                {
                    new Interface("IStudent")
                    {
                        Comment = "Interface",
                        Members =
                        {
                            new Property("yearOfBirth", "number"),
                            new Property("age", "number"),
                            new Function("printDetails", "void")
                        }
                    },
                    new Class("School")
                    {
                        Comment = "Base class",
                        Members =
                        {
                             new Constructor
                            {
                                { "name", "string" },
                                { "city", "string" }
                            },

                            new Property("name", "string"),
                            new Property("city", "string"),
                            new Function("address")
                            {
                                Modifiers = { Modifier.Public },
                                Arguments = { { "streetName", "string" } },
                                Statements =
                                {
                                    "return ('College Name:' + this.name + ' City: ' + this.city + ' Street Name: ' + streetName);"
                                }
                            }
                        }
                    },
                    new Class("Student")
                    {
                        Comment = "Child Class implements IStudent and inherits from College",
                        Extends = "School",
                        Implements = { "IStudent", "IArguments" },
                        Members =
                        {
                            { "length", "number" },
                            { "callee", "Function" },
                            { "firstName", "string" },
                            { "lastName", "string" },
                            { "yearOfBirth", "number" },
                            new Constructor
                            {
                                Comment = "Constructor",
                                Arguments =
                                {
                                    { "firstName", "string" },
                                    { "lastName", "string" },
                                    { "name", "string" },
                                    { "city", "string" },
                                    { "yearOfBirth", "number" }
                                },
                                Statements =
                                {
                                    "super(name, city);",
                                    "this.firstName = firstName;",
                                    "this.lastName = lastName;",
                                    "this.yearOfBirth = yearOfBirth;"
                                }                                
                            },
                            new Property("age", "number")
                            {
                                Get = new Getter
                                {
                                    "return 2014 - this.yearOfBirth;"
                                },
                                Set = new Setter("value")
                                {
                                    "// Do a thing",
                                    "var self = this;",
                                    new Function("nested")
                                    {
                                        new Interface("IAlsoNested")
                                        {
                                            Extends = { "IStudent", "IArguments" },
                                            Members = { new Property("faveColor", "string") }
                                        },

                                        "return self.firstName;"
                                    },

                                    "nested();"
                                }
                            },
                            new Function("collegeDetails")
                            {
                                "var y = super.address('Maple Street');",
                                "alert(y);"
                            },
                            new Function("printDetails", "void")
                            {
                                "alert(this.firstName + ' ' + this.lastName + ' College is: ' + this.name);"
                            }
                        }
                    }
                };

            file.ToString().Should().BeEquivalentTo(expected);
        }
    }
}