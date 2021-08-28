namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class TypeMatchProcessorTests : Tests<BaseTypeMatchProcessor>
    {
        public TypeMatchProcessorTests(ITestOutputHelper output) : base(output.BuildLogger(LogLevel.Debug))
        {
        }

        [Fact]
        public void CompareMatchDoesNotMergePartialTypesMatchingDifferentNames()
        {
            var firstTypeName = Guid.NewGuid().ToString();
            var secondTypeName = Guid.NewGuid().ToString();
            var oldFirstItem = Substitute.For<ITypeDefinition>();
            var oldSecondItem = Substitute.For<ITypeDefinition>();
            var oldItems = new List<ITypeDefinition>
            {
                oldFirstItem,
                oldSecondItem
            };
            var newFirstItem = Substitute.For<ITypeDefinition>();
            var newSecondItem = Substitute.For<ITypeDefinition>();
            var newItems = new List<ITypeDefinition>
            {
                newFirstItem,
                newSecondItem
            };
            var match = new ItemMatch<IBaseTypeDefinition>(oldFirstItem, newFirstItem);
            var matches = new List<ItemMatch<IBaseTypeDefinition>> {match};
            var matchResults = new MatchResults<IBaseTypeDefinition>(matches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldFirstItem, newFirstItem, message);
            var results = new List<ComparisonResult> {result};
            var options = ComparerOptions.Default;

            oldFirstItem.FullName.Returns(firstTypeName);
            oldSecondItem.FullName.Returns(secondTypeName);
            newFirstItem.FullName.Returns(firstTypeName);
            newSecondItem.FullName.Returns(secondTypeName);

            Service<IBaseTypeEvaluator>().FindMatches(
                Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(oldItems)),
                Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(newItems))).Returns(matchResults);
            Service<IBaseTypeComparer>().CompareMatch(match, options).Returns(results);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual.Should().Contain(result);

            oldFirstItem.DidNotReceive().MergePartialType(oldSecondItem);
            newFirstItem.DidNotReceive().MergePartialType(newSecondItem);
        }

        [Fact]
        public void CompareMatchDoesNotMergePartialTypesMatchingDifferentTypeDefinitions()
        {
            var typeName = Guid.NewGuid().ToString();
            var oldFirstItem = Substitute.For<IClassDefinition>();
            var oldSecondItem = Substitute.For<IInterfaceDefinition>();
            var oldItems = new List<ITypeDefinition>
            {
                oldFirstItem,
                oldSecondItem
            };
            var newFirstItem = Substitute.For<IClassDefinition>();
            var newSecondItem = Substitute.For<IInterfaceDefinition>();
            var newItems = new List<ITypeDefinition>
            {
                newFirstItem,
                newSecondItem
            };
            var match = new ItemMatch<IBaseTypeDefinition>(oldFirstItem, newFirstItem);
            var matches = new List<ItemMatch<IBaseTypeDefinition>> {match};
            var matchResults = new MatchResults<IBaseTypeDefinition>(matches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldFirstItem, newFirstItem, message);
            var results = new List<ComparisonResult> {result};
            var options = ComparerOptions.Default;

            oldFirstItem.FullName.Returns(typeName);
            oldSecondItem.FullName.Returns(typeName);
            newFirstItem.FullName.Returns(typeName);
            newSecondItem.FullName.Returns(typeName);

            Service<IBaseTypeEvaluator>().FindMatches(
                Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(oldItems)),
                Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(newItems))).Returns(matchResults);
            Service<IBaseTypeComparer>().CompareMatch(match, options).Returns(results);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual.Should().Contain(result);

            oldFirstItem.DidNotReceive().MergePartialType(oldSecondItem);
            newFirstItem.DidNotReceive().MergePartialType(newSecondItem);
        }

        [Fact]
        public void CompareMatchEvaluatesChildTypes()
        {
            var oldChildItem = new TestClassDefinition().Set(x => x.Name = "OldChild");
            var oldChildItems = new List<IClassDefinition>
            {
                oldChildItem
            };
            var oldParentItem = new TestClassDefinition().Set(x =>
            {
                x.Name = "OldParent";
                x.ChildClasses = oldChildItems;
                x.ChildTypes = oldChildItems;
            });
            var oldParentItems = new List<IClassDefinition>
            {
                oldParentItem
            };
            var newChildItem = new TestClassDefinition().Set(x => x.Name = "NewChild");
            var newChildItems = new List<IClassDefinition>
            {
                newChildItem
            };
            var newParentItem = new TestClassDefinition().Set(x =>
            {
                x.Name = "NewParent";
                x.ChildClasses = newChildItems;
                x.ChildTypes = newChildItems;
            });
            var newParentItems = new List<IClassDefinition>
            {
                newParentItem
            };
            var childMatch = new ItemMatch<IBaseTypeDefinition>(oldChildItem, newChildItem);
            var childMatches = new List<ItemMatch<IBaseTypeDefinition>> {childMatch};
            var childMatchResults = new MatchResults<IBaseTypeDefinition>(childMatches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var childMessage = Guid.NewGuid().ToString();
            var childResult = new ComparisonResult(SemVerChangeType.Breaking, oldChildItem, newChildItem, childMessage);
            var childResults = new List<ComparisonResult> {childResult};
            var parentMatch = new ItemMatch<IBaseTypeDefinition>(oldParentItem, newParentItem);
            var parentMatches = new List<ItemMatch<IBaseTypeDefinition>> {parentMatch};
            var parentMatchResults = new MatchResults<IBaseTypeDefinition>(parentMatches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var parentMessage = Guid.NewGuid().ToString();
            var parentResult =
                new ComparisonResult(SemVerChangeType.Breaking, oldParentItem, newParentItem, parentMessage);
            var parentResults = new List<ComparisonResult> {parentResult};
            var options = ComparerOptions.Default;

            Service<IBaseTypeEvaluator>().FindMatches(
                    Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(oldParentItems)),
                    Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(newParentItems)))
                .Returns(parentMatchResults);
            Service<IBaseTypeComparer>().CompareMatch(parentMatch, options).Returns(parentResults);
            Service<IBaseTypeEvaluator>().FindMatches(
                    Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(oldChildItems)),
                    Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().BeEquivalentTo(newChildItems)))
                .Returns(childMatchResults);
            Service<IBaseTypeComparer>().CompareMatch(childMatch, options).Returns(childResults);

            var actual = SUT.CalculateChanges(oldParentItems, newParentItems, options).ToList();

            actual.Should().HaveCount(2);
            actual.Should().Contain(parentResult);
            actual.Should().Contain(childResult);
        }

        [Fact]
        public void CompareMatchMergesPartialTypes()
        {
            var typeName = Guid.NewGuid().ToString();
            var oldFirstItem = Substitute.For<ITypeDefinition>();
            var oldSecondItem = Substitute.For<ITypeDefinition>();
            var oldItems = new List<ITypeDefinition>
            {
                oldFirstItem,
                oldSecondItem
            };
            var newFirstItem = Substitute.For<ITypeDefinition>();
            var newSecondItem = Substitute.For<ITypeDefinition>();
            var newItems = new List<ITypeDefinition>
            {
                newFirstItem,
                newSecondItem
            };
            var match = new ItemMatch<IBaseTypeDefinition>(oldFirstItem, newFirstItem);
            var matches = new List<ItemMatch<IBaseTypeDefinition>> {match};
            var matchResults = new MatchResults<IBaseTypeDefinition>(matches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(SemVerChangeType.Breaking, oldFirstItem, newFirstItem, message);
            var results = new List<ComparisonResult> {result};
            var options = ComparerOptions.Default;

            oldFirstItem.FullName.Returns(typeName);
            oldSecondItem.FullName.Returns(typeName);
            newFirstItem.FullName.Returns(typeName);
            newSecondItem.FullName.Returns(typeName);

            Service<IBaseTypeEvaluator>().FindMatches(
                Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().Contain(oldFirstItem)),
                Match.On<IEnumerable<IBaseTypeDefinition>>(x => x.Should().Contain(newFirstItem))).Returns(matchResults);
            Service<IBaseTypeComparer>().CompareMatch(match, options).Returns(results);

            var actual = SUT.CalculateChanges(oldItems, newItems, options).ToList();

            actual.Should().HaveCount(1);
            actual.Should().Contain(result);

            oldFirstItem.Received().MergePartialType(oldSecondItem);
            newFirstItem.Received().MergePartialType(newSecondItem);
            Service<IBaseTypeEvaluator>().DidNotReceive().FindMatches(
                Match.On<IEnumerable<ITypeDefinition>>(x => x.Should().Contain(oldSecondItem)),
                Match.On<IEnumerable<ITypeDefinition>>(x => x.Should().Contain(newSecondItem)));
        }
    }
}