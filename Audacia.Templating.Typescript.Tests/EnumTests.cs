﻿using System.IO;
using FluentAssertions;
using Xunit;

namespace Audacia.Templating.Typescript.Tests
{
    public class EnumTests
    {
        [Fact]
        public void Returns_a_correctly_formed_enum()
        {
            var expected = File.ReadAllText("enum.ts");
            var @enum = new Enum<string>("IceCream")
            {
                Modifiers = {Modifier.Export},
                Members =
                {
                    {"vanilla", "Vanilla"},
                    {"chocolate", "Chocolate"},
                    {"strawberry", "Strawberry"}
                }
            };

            @enum.ToString().Should().Be(expected);
        }
    }
}