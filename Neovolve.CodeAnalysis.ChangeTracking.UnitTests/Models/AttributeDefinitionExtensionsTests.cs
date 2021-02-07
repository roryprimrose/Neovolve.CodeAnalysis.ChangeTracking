namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class AttributeDefinitionExtensionsTests
    {
        [Theory]
        [InlineData("SomethingAttribute", "Something")]
        [InlineData("Something", "Something")]
        public void GetRawNameReturnsExpectedValueForAttribute(string name, string expected)
        {
            var attribute = Substitute.For<IAttributeDefinition>();

            attribute.Name.Returns(name);

            var actual = attribute.GetRawName();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("SomethingAttribute", "Something")]
        [InlineData("Something", "Something")]
        public async Task GetRawNameReturnsExpectedValueForNode(string name, string expected)
        {
            var node = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute.Replace("SimpleAttribute", name))
                .ConfigureAwait(false);

            var actual = node.GetRawName();

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetRawNameThrowsExceptionWithNullAttribute()
        {
            Action action = () => ((IAttributeDefinition)null!).GetRawName();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetRawNameThrowsExceptionWithNullNode()
        {
            Action action = () => ((AttributeSyntax)null!).GetRawName();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}