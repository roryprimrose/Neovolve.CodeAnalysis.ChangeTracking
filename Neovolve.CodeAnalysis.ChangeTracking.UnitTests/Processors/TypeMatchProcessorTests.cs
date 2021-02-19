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

    public class TypeMatchProcessorTests : Tests<TypeMatchProcessor>
    {
        public TypeMatchProcessorTests(ITestOutputHelper output) : base(output.BuildLogger(LogLevel.Debug))
        {
        }

        [Fact]
        public void CompareItemsEvaluatesChildTypes()
        {
            var oldChildItem = new TestClassDefinition().Set(x => x.Name = "OldChild");
            var oldChildItems = new []
            {
                oldChildItem
            };
            var oldParentItem = new TestClassDefinition().Set(x =>
            {
                x.Name = "OldParent";
                x.ChildTypes = oldChildItems;
            });
            var oldParentItems = new List<IClassDefinition>
            {
                oldParentItem
            };
            var newChildItem = new TestClassDefinition().Set(x => x.Name = "NewChild");
            var newChildItems = new[]
            {
                newChildItem
            };
            var newParentItem = new TestClassDefinition().Set(x =>
            {
                x.Name = "NewParent";
                x.ChildTypes = newChildItems;
            });
            var newParentItems = new List<IClassDefinition>
            {
                newParentItem
            };
            var childMatch = new ItemMatch<ITypeDefinition>(oldChildItem, newChildItem);
            var childMatches = new List<ItemMatch<ITypeDefinition>> { childMatch };
            var childMatchResults = new MatchResults<ITypeDefinition>(childMatches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var childMessage = Guid.NewGuid().ToString();
            var childResult = new ComparisonResult(SemVerChangeType.Breaking, oldChildItem, newChildItem, childMessage);
            var childResults = new List<ComparisonResult> { childResult };
            var parentMatch = new ItemMatch<ITypeDefinition>(oldParentItem, newParentItem);
            var parentMatches = new List<ItemMatch<ITypeDefinition>> { parentMatch };
            var parentMatchResults = new MatchResults<ITypeDefinition>(parentMatches, Array.Empty<IClassDefinition>(),
                Array.Empty<IClassDefinition>());
            var parentMessage = Guid.NewGuid().ToString();
            var parentResult = new ComparisonResult(SemVerChangeType.Breaking, oldParentItem, newParentItem, parentMessage);
            var parentResults = new List<ComparisonResult> { parentResult};
            var options = ComparerOptions.Default;

            Service<IMatchEvaluator<ITypeDefinition>>().MatchItems(oldParentItems, newParentItems).Returns(parentMatchResults);
            Service<ITypeComparer<ITypeDefinition>>().CompareItems(parentMatch, options).Returns(parentResults);
            Service<IMatchEvaluator<ITypeDefinition>>().MatchItems(oldChildItems, newChildItems).Returns(childMatchResults);
            Service<ITypeComparer<ITypeDefinition>>().CompareItems(childMatch, options).Returns(childResults);

            var actual = SUT.CalculateChanges(oldParentItems, newParentItems, options).ToList();
            
            actual.Should().HaveCount(2);
            actual.Should().Contain(parentResult);
            actual.Should().Contain(childResult);
        }
    }
}