namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class MethodComparerTests : Tests<MethodComparer>
    {
        private readonly ITestOutputHelper _output;

        public MethodComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsDoesNotContinueEvaluationWhenGenericTypeChangeIsBreaking()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            const SemVerChangeType changeType = SemVerChangeType.Breaking;
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IGenericTypeElementComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IGenericTypeElement>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);

            Service<IParameterComparer>().DidNotReceive()
                .CompareItems(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareItemsDoesNotContinueEvaluationWhenModifierChangeIsBreaking()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Name = "Renamed");
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            const SemVerChangeType changeType = SemVerChangeType.Breaking;
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IMethodModifiersComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IModifiersElement<MethodModifiers>>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);

            Service<IGenericTypeElementComparer>().DidNotReceive()
                .CompareItems(Arg.Any<ItemMatch<IGenericTypeElement>>(), Arg.Any<ComparerOptions>());
            Service<IParameterComparer>().DidNotReceive()
                .CompareItems(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareItemsDoesNotContinueEvaluationWhenNameChanged()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Name = "Renamed");
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            Service<IGenericTypeElementComparer>().DidNotReceive()
                .CompareItems(Arg.Any<ItemMatch<IGenericTypeElement>>(), Arg.Any<ComparerOptions>());
            Service<IParameterComparer>().DidNotReceive()
                .CompareItems(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenNameChanged()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Name = "Renamed");
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().BeEquivalentTo(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenParameterAdded()
        {
            var parameter = new TestParameterDefinition();
            var parameters = new List<IParameterDefinition> {parameter}.AsReadOnly();
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = parameters);
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().BeEquivalentTo(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenParameterRemoved()
        {
            var parameter = new TestParameterDefinition();
            var parameters = new List<IParameterDefinition> {parameter}.AsReadOnly();
            var oldItem = new TestMethodDefinition().Set(x => x.Parameters = parameters);
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = Array.Empty<IParameterDefinition>());
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().BeEquivalentTo(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenItemsMatch()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsResultFromGenericTypeElementComparer()
        {
            var item = new TestMethodDefinition();
            var match = new ItemMatch<IMethodDefinition>(item, item);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] {result};

            Service<IGenericTypeElementComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IGenericTypeElement>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareItemsReturnsResultFromMethodModifierComparer()
        {
            var item = new TestMethodDefinition();
            var match = new ItemMatch<IMethodDefinition>(item, item);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] {result};

            Service<IMethodModifiersComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IModifiersElement<MethodModifiers>>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareItemsReturnsResultFromParameterComparer()
        {
            var oldParameter = new TestParameterDefinition();
            var oldParameters = new List<IParameterDefinition> {oldParameter}.AsReadOnly();
            var oldItem = new TestMethodDefinition().Set(x => x.Parameters = oldParameters);
            var newParameter = new TestParameterDefinition();
            var newParameters = new List<IParameterDefinition> {newParameter}.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = newParameters);
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};
            var options = ComparerOptions.Default;

            Service<IParameterComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IParameterDefinition>>(x =>
                        x.OldItem == oldParameter && x.NewItem == newParameter), options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullGenericTypeElementComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var methodModifiersComparer = Substitute.For<IMethodModifiersComparer>();
            var parameterComparer = Substitute.For<IParameterComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new MethodComparer(accessModifiersComparer, methodModifiersComparer, null!, parameterComparer,
                    attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullMethodModifiersChangeTable()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var parameterComparer = Substitute.For<IParameterComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new MethodComparer(accessModifiersComparer, null!, genericTypeElementComparer, parameterComparer,
                    attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var methodModifiersComparer = Substitute.For<IMethodModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new MethodComparer(accessModifiersComparer, methodModifiersComparer, genericTypeElementComparer, null!,
                    attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}