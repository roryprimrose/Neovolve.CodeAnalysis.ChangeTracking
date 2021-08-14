namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class EnumMemberDefinitionTests
    {
        private const string EnumMemberWithExplicitValue = @"
namespace MyNamespace 
{
    public enum MyEnum
    {
        First = 123
    }   
}
";

        private const string EnumMemberWithFlagValues = @"
namespace MyNamespace 
{
    [Flags]
    public enum MyEnum
    {
        First = 1,
        Second = 2,
        Third = 4,
        All = First | Second | Third
    }   
}
";

        private const string EnumMemberWithImplicitValues = @"
namespace MyNamespace 
{
    public enum MyEnum
    {
        First
    }   
}
";

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var declaringType = new TestEnumDefinition();

            var node = await TestNode.FindNode<EnumMemberDeclarationSyntax>(EnumMemberWithExplicitValue)
                .ConfigureAwait(false);

            var sut = new EnumMemberDefinition(declaringType, node, 0);

            sut.DeclaringType.Should().Be(declaringType);
            sut.Name.Should().Be("First");
            sut.RawName.Should().Be("First");
            sut.FullName.Should().Be(declaringType.FullName + ".First");
            sut.FullRawName.Should().Be(declaringType.FullRawName + ".First");
            sut.IsVisible.Should().Be(declaringType.IsVisible);
        }

        [Fact]
        public async Task ValueReturnsExplicitValue()
        {
            var index = Environment.TickCount;
            var declaringType = new TestEnumDefinition();

            var node = await TestNode.FindNode<EnumMemberDeclarationSyntax>(EnumMemberWithExplicitValue)
                .ConfigureAwait(false);

            var sut = new EnumMemberDefinition(declaringType, node, index);

            sut.Value.Should().Be("123");
        }

        [Fact]
        public async Task ValueReturnsFlagsValues()
        {
            var index = Environment.TickCount;
            var declaringType = new TestEnumDefinition();

            var nodes = await TestNode.FindNodes<EnumMemberDeclarationSyntax>(EnumMemberWithFlagValues)
                .ConfigureAwait(false);

            var memberNode = nodes.Single(x => x.Identifier.Text == "All");

            var sut = new EnumMemberDefinition(declaringType, memberNode, index);

            sut.Value.Should().Be("First | Second | Third");
        }

        [Fact]
        public async Task ValueReturnsIndexWhenNoExplicitValueFound()
        {
            var index = Environment.TickCount;
            var declaringType = new TestEnumDefinition();

            var node = await TestNode.FindNode<EnumMemberDeclarationSyntax>(EnumMemberWithImplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumMemberDefinition(declaringType, node, index);

            sut.Value.Should().Be(index.ToString());
        }
    }
}