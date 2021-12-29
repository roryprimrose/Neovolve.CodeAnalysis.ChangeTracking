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
        First = 123,
        Second = 432
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
        First,
        Second
    }   
}
";

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var declaringType = new TestEnumDefinition();
            var index = Environment.TickCount;

            var node = await TestNode.FindNode<EnumMemberDeclarationSyntax>(EnumMemberWithExplicitValue)
                .ConfigureAwait(false);

            var sut = new EnumMemberDefinition(declaringType, node, index);

            sut.DeclaringType.Should().Be(declaringType);
            sut.Name.Should().Be("First");
            sut.RawName.Should().Be("First");
            sut.FullName.Should().Be(declaringType.FullName + ".First");
            sut.FullRawName.Should().Be(declaringType.FullRawName + ".First");
            sut.IsVisible.Should().Be(declaringType.IsVisible);
            sut.Index.Should().Be(index);
        }

        [Fact]
        public async Task MatchesReturnsFalseWhenElementIsDifferentType()
        {
            var classType = new TestClassDefinition();
            var propertyNode = await TestNode.FindNode<PropertyDeclarationSyntax>(PropertyDefinitionCode.GetSetProperty)
                .ConfigureAwait(false);

            var otherMember = new PropertyDefinition(classType, propertyNode);

            var declaringType = new TestEnumDefinition();
            const int index = 1;
            const ElementMatchOptions options = ElementMatchOptions.All;

            var nodes = (await TestNode.FindNodes<EnumMemberDeclarationSyntax>(EnumMemberWithFlagValues)
                .ConfigureAwait(false)).ToList();
            var memberNode = nodes.Single(x => x.Identifier.Text == "Second");

            var sut = new EnumMemberDefinition(declaringType, memberNode, index);

            var actual = sut.Matches(otherMember, options);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(EnumMemberWithExplicitValue, 0, 0, ElementMatchOptions.All, true)]
        [InlineData(EnumMemberWithExplicitValue, 0, 1, ElementMatchOptions.All, true)]
        [InlineData(EnumMemberWithExplicitValue, 0, 0, ElementMatchOptions.IgnoreValue, true)]
        [InlineData(EnumMemberWithExplicitValue, 0, 1, ElementMatchOptions.IgnoreValue, true)]
        [InlineData(EnumMemberWithImplicitValues, 0, 0, ElementMatchOptions.All, true)]
        [InlineData(EnumMemberWithImplicitValues, 0, 1, ElementMatchOptions.All, false)]
        [InlineData(EnumMemberWithImplicitValues, 0, 0, ElementMatchOptions.IgnoreValue, true)]
        [InlineData(EnumMemberWithImplicitValues, 0, 1, ElementMatchOptions.IgnoreValue, true)]
        public async Task MatchesReturnsWhetherElementsMatchBasedOnIndex(string code, int otherIndex, int index,
            ElementMatchOptions options, bool expected)
        {
            var declaringType = new TestEnumDefinition();
            var nodes = (await TestNode.FindNodes<EnumMemberDeclarationSyntax>(code)
                .ConfigureAwait(false)).ToList();
            var memberNode = nodes.Single(x => x.Identifier.Text == "First");
            var otherMember = new EnumMemberDefinition(declaringType, memberNode, otherIndex);

            var sut = new EnumMemberDefinition(declaringType, memberNode, index);

            var actual = sut.Matches(otherMember, options);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("Second", ElementMatchOptions.All, true)]
        [InlineData("Second", ElementMatchOptions.IgnoreName, true)]
        [InlineData("Other", ElementMatchOptions.All, false)]
        [InlineData("Other", ElementMatchOptions.IgnoreName, true)]
        public async Task MatchesReturnsWhetherElementsMatchBasedOnName(string name,
            ElementMatchOptions options, bool expected)
        {
            var declaringType = new TestEnumDefinition();
            const int index = 1;

            var otherMemberNodes = (await TestNode.FindNodes<EnumMemberDeclarationSyntax>(EnumMemberWithFlagValues)
                .ConfigureAwait(false)).ToList();
            var otherMemberNode = otherMemberNodes.Single(x => x.Identifier.Text == "Second");
            var otherMember = new EnumMemberDefinition(declaringType, otherMemberNode, index);

            var memberNodes = (await TestNode
                .FindNodes<EnumMemberDeclarationSyntax>(EnumMemberWithFlagValues.Replace("Second", name))
                .ConfigureAwait(false)).ToList();
            var memberNode = memberNodes.Single(x => x.Identifier.Text == name);

            var sut = new EnumMemberDefinition(declaringType, memberNode, index);

            var actual = sut.Matches(otherMember, options);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("= 2", "= 2", ElementMatchOptions.All, true)]
        [InlineData("= 2", "= 8", ElementMatchOptions.All, false)]
        [InlineData("", "= 2", ElementMatchOptions.All, false)]
        [InlineData("= 2", "", ElementMatchOptions.All, false)]
        [InlineData("= 2", "= 2", ElementMatchOptions.IgnoreValue, true)]
        [InlineData("= 2", "= 8", ElementMatchOptions.IgnoreValue, true)]
        [InlineData("", "= 2", ElementMatchOptions.IgnoreValue, true)]
        [InlineData("= 2", "", ElementMatchOptions.IgnoreValue, true)]
        public async Task MatchesReturnsWhetherElementsMatchBasedOnValue(string otherValue, string value,
            ElementMatchOptions options, bool expected)
        {
            var declaringType = new TestEnumDefinition();
            const int index = 1;

            var otherMemberNodes = (await TestNode
                .FindNodes<EnumMemberDeclarationSyntax>(EnumMemberWithFlagValues.Replace("= 2", otherValue))
                .ConfigureAwait(false)).ToList();
            var otherMemberNode = otherMemberNodes.Single(x => x.Identifier.Text == "Second");
            var otherMember = new EnumMemberDefinition(declaringType, otherMemberNode, index);

            var memberNodes = (await TestNode
                .FindNodes<EnumMemberDeclarationSyntax>(EnumMemberWithFlagValues.Replace("= 2", value))
                .ConfigureAwait(false)).ToList();
            var memberNode = memberNodes.Single(x => x.Identifier.Text == "Second");

            var sut = new EnumMemberDefinition(declaringType, memberNode, index);

            var actual = sut.Matches(otherMember, options);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task ValueReturnsEmptyWhenNoExplicitValueFound()
        {
            var index = Environment.TickCount;
            var declaringType = new TestEnumDefinition();

            var node = await TestNode.FindNode<EnumMemberDeclarationSyntax>(EnumMemberWithImplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumMemberDefinition(declaringType, node, index);

            sut.Value.Should().BeEmpty();
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
    }
}