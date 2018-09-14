using System;
using FluentAssertions;
using Xunit;

namespace Audacia.Templating.Typescript.Tests
{
    public class CommentTests
    {
        public new class ToString : CommentTests
        {
            [Fact]
            public void Returns_a_correctly_formed_comment()
            {
                var comment = new Comment("This is a great comment");
                comment.ToString().Should().Be("// This is a great comment" + Environment.NewLine);
            }
        }
    }
}
