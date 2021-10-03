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

    public class ConstructorComparerTests : Tests<ConstructorComparer>
    {
        private readonly ITestOutputHelper _output;

        public ConstructorComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // Test reordering parameters

        [Theory]
        [InlineData(ConstructorModifiers.Static, ConstructorModifiers.None)]
        [InlineData(ConstructorModifiers.None, ConstructorModifiers.Static)]
        public void CompareMatchDoesNotContinueEvaluationWhenModifierChangeIsBreaking(ConstructorModifiers oldModifer,
            ConstructorModifiers newModifer)
        {
            var oldItem = new TestConstructorDefinition().Set(x => x.Modifiers = oldModifer);
            var newItem = oldItem.JsonClone().Set(x => x.Modifiers = newModifer);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);

            Service<IParameterComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleParametersAdded()
        {
            var oldItem = new TestConstructorDefinition();
            var firstParameter = new TestParameterDefinition().Set(x => x.DeclaringMember = oldItem);
            var secondParameter = new TestParameterDefinition().Set(x => x.DeclaringMember = oldItem);
            var parameters = new List<IParameterDefinition> { firstParameter, secondParameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = parameters);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().Contain("parameters");
            actual[0].Message.Should().NotContain(firstParameter.Name);
            actual[0].Message.Should().NotContain(secondParameter.Name);
            actual[0].Message.Should().Contain(" 2 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleParametersAddedAndRemoved()
        {
            var firstParameter = new TestParameterDefinition();
            var secondParameter = new TestParameterDefinition();
            var oldParameters = new List<IParameterDefinition> { firstParameter, secondParameter }.AsReadOnly();
            var oldItem = new TestConstructorDefinition().Set(x => x.Parameters = oldParameters);
            var thirdParameter = new TestParameterDefinition();
            var fourthParameter = new TestParameterDefinition();
            var fifthParameter = new TestParameterDefinition();
            var newParameters = new List<IParameterDefinition> { thirdParameter, fourthParameter, fifthParameter }
                .AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = newParameters);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().Contain("parameter");
            actual[0].Message.Should().NotContain(firstParameter.Name);
            actual[0].Message.Should().NotContain(secondParameter.Name);
            actual[0].Message.Should().NotContain(thirdParameter.Name);
            actual[0].Message.Should().NotContain(fourthParameter.Name);
            actual[0].Message.Should().NotContain(fifthParameter.Name);
            actual[0].Message.Should().Contain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleParametersRemoved()
        {
            var firstParameter = new TestParameterDefinition();
            var secondParameter = new TestParameterDefinition();
            var parameters = new List<IParameterDefinition> { firstParameter, secondParameter }.AsReadOnly();
            var oldItem = new TestConstructorDefinition().Set(x => x.Parameters = parameters);
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = Array.Empty<IParameterDefinition>());
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
            actual[0].Message.Should().Contain("parameters");
            actual[0].Message.Should().NotContain(firstParameter.Name);
            actual[0].Message.Should().NotContain(secondParameter.Name);
            actual[0].Message.Should().Contain(" 2 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSingleParameterAdded()
        {
            var oldItem = new TestConstructorDefinition();
            var parameter = new TestParameterDefinition().Set(x => x.DeclaringMember = oldItem);
            var parameters = new List<IParameterDefinition> { parameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = parameters);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().Contain("Parameter");
            actual[0].Message.Should().Contain(parameter.Name);
            actual[0].Message.Should().NotContain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSingleParameterRemoved()
        {
            var parameter = new TestParameterDefinition();
            var parameters = new List<IParameterDefinition> { parameter }.AsReadOnly();
            var oldItem = new TestConstructorDefinition().Set(x => x.Parameters = parameters);
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = Array.Empty<IParameterDefinition>());
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
            actual[0].Message.Should().Contain("Parameter");
            actual[0].Message.Should().Contain(parameter.Name);
            actual[0].Message.Should().NotContain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenItemsMatch()
        {
            var oldItem = new TestConstructorDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsResultFromParameterComparer()
        {
            var oldParameter = new TestParameterDefinition();
            var oldParameters = new List<TestParameterDefinition> { oldParameter }.AsReadOnly();
            var oldItem = new TestConstructorDefinition().Set(x => x.Parameters = oldParameters);
            var newParameter = new TestParameterDefinition();
            var newParameters = new List<TestParameterDefinition> { newParameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = newParameters);
            var match = new ItemMatch<IConstructorDefinition>(oldItem, newItem);
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };
            var options = TestComparerOptions.Default;

            Service<IParameterComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IParameterDefinition>>(x =>
                        x.OldItem == oldParameter && x.NewItem == newParameter), options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new ConstructorComparer(accessModifiersComparer, null!,
                    attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}