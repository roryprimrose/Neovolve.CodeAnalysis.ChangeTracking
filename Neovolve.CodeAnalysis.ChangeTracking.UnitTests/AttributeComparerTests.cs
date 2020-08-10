namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;
    using Xunit.Abstractions;

    public class AttributeComparerTests
    {
        private readonly ITestOutputHelper _output;

        public AttributeComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenArgumentsAdded()
        {
            var oldNode = await TestNode.FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithOrdinalArguments)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenArgumentsRemoved()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithOrdinalArguments)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenNamedArgumentValueChanged()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedOrdinalAndNamedArguments)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedArgumentsWhereNamedValueChanged)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeAssignableTo<IArgumentDefinition>();
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenNamedArgumentParameterNameChanged()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedOrdinalAndNamedArguments)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedArgumentsWhereNamedParameterNameChanged)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeNull();
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenOrdinalArgumentChanged()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedOrdinalAndNamedArguments)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithMixedArgumentsWhereOrdinalValueChanged)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeAssignableTo<IArgumentDefinition>();
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenOrdinalArgumentsAdded()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithOneOrdinalAndTwoNamedArguments)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithTwoOrdinalAndOneNamedArguments)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public async Task CompareItemsReturnsBreakingWhenOrdinalArgumentsRemoved()
        {
            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithTwoOrdinalAndOneNamedArguments)
                .ConfigureAwait(false);
            var oldItem = new AttributeDefinition(oldNode);
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.AttributeWithOneOrdinalAndTwoNamedArguments)
                .ConfigureAwait(false);
            var newItem = new AttributeDefinition(newNode);
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(1);

            _output.WriteLine(actual[0].Message);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareItemsReturnsEmptyChangesWhenAttributesDoNotHaveArguments()
        {
            var oldItem = new TestAttributeDefinition();
            var newItem = new TestAttributeDefinition();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullMatch()
        {
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            Action action = () => sut.CompareItems(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullOptions()
        {
            var oldItem = new TestAttributeDefinition();
            var newItem = new TestAttributeDefinition();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);

            var sut = new AttributeComparer();

            Action action = () => sut.CompareItems(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}