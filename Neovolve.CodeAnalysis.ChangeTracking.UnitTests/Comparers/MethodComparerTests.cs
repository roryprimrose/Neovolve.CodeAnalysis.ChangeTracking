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
        public void CompareMatchDoesNotContinueEvaluationWhenGenericTypeChangeIsBreaking()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            const SemVerChangeType changeType = SemVerChangeType.Breaking;
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IGenericTypeElementComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IGenericTypeElement>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);

            Service<IParameterComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareMatchDoesNotContinueEvaluationWhenModifierChangeIsBreaking()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            const SemVerChangeType changeType = SemVerChangeType.Breaking;
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IMethodModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IModifiersElement<MethodModifiers>>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);

            Service<IGenericTypeElementComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IGenericTypeElement>>(), Arg.Any<ComparerOptions>());
            Service<IParameterComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareMatchDoesNotContinueEvaluationWhenNameChanged()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone().Set(x =>
            {
                x.Name = "Renamed";
                x.RawName = "Renamed";
            });
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            Service<IGenericTypeElementComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IGenericTypeElement>>(), Arg.Any<ComparerOptions>());
            Service<IParameterComparer>().DidNotReceive()
                .CompareMatch(Arg.Any<ItemMatch<IParameterDefinition>>(), Arg.Any<ComparerOptions>());
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleParametersAdded()
        {
            var oldItem = new TestMethodDefinition();
            var firstParameter = new TestParameterDefinition().Set(x => x.DeclaringMember = oldItem);
            var secondParameter = new TestParameterDefinition().Set(x => x.DeclaringMember = oldItem);
            var parameters = new List<IParameterDefinition> { firstParameter, secondParameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = parameters);
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
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
            var oldItem = new TestMethodDefinition().Set(x => x.Parameters = oldParameters);
            var thirdParameter = new TestParameterDefinition();
            var fourthParameter = new TestParameterDefinition();
            var fifthParameter = new TestParameterDefinition();
            var newParameters = new List<IParameterDefinition> { thirdParameter, fourthParameter, fifthParameter }
                .AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = newParameters);
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
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
            var oldItem = new TestMethodDefinition().Set(x => x.Parameters = parameters);
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = Array.Empty<IParameterDefinition>());
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
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
        public void CompareMatchReturnsBreakingWhenNameChanged()
        {
            var oldItem = new TestMethodDefinition().Set(x =>
            {
                x.Name = "Original";
                x.RawName = "Original";
            });
            var newItem = oldItem.JsonClone().Set(x =>
            {
                x.Name = "Renamed";
                x.RawName = "Renamed";
            });
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);
            actual.First().Message.Should().Contain("renamed");

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNameChangedWithGenericTypeParameters()
        {
            var genericTypeParameters = new List<string>
            {
                "T"
            }.AsReadOnly();
            var oldItem = new TestMethodDefinition().Set(x =>
            {
                x.Name = "Original<T>";
                x.RawName = "Original";
                x.GenericTypeParameters = genericTypeParameters;
            });
            var newItem = oldItem.JsonClone().Set(x =>
            {
                x.Name = "Renamed<T>";
                x.RawName = "Renamed";
            });
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);
            actual.First().Message.Should().Contain("renamed");

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSingleParameterAdded()
        {
            var oldItem = new TestMethodDefinition();
            var parameter = new TestParameterDefinition().Set(x => x.DeclaringMember = oldItem);
            var parameters = new List<IParameterDefinition> { parameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = parameters);
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
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
            var oldItem = new TestMethodDefinition().Set(x => x.Parameters = parameters);
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = Array.Empty<IParameterDefinition>());
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
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
        public void CompareMatchReturnsEmptyChangesWhenGenericTypeParameterRenamed()
        {
            var oldTypeParameters = new List<string>
            {
                "T"
            }.AsReadOnly();
            var oldItem = new TestMethodDefinition().Set(x =>
            {
                x.Name = "Original<T>";
                x.RawName = "Original";
                x.GenericTypeParameters = oldTypeParameters;
            });
            var newTypeParameters = new List<string>
            {
                "V"
            }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x =>
            {
                x.Name = "Original<V>";
                x.RawName = "Original";
                x.GenericTypeParameters = newTypeParameters;
            });
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenItemsMatch()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsResultFromGenericTypeElementComparer()
        {
            var item = new TestMethodDefinition();
            var match = new ItemMatch<IMethodDefinition>(item, item);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] { result };

            Service<IGenericTypeElementComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IGenericTypeElement>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromMethodModifierComparer()
        {
            var item = new TestMethodDefinition();
            var match = new ItemMatch<IMethodDefinition>(item, item);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] { result };

            Service<IMethodModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IModifiersElement<MethodModifiers>>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromParameterComparer()
        {
            var oldParameter = new TestParameterDefinition();
            var oldParameters = new List<IParameterDefinition> { oldParameter }.AsReadOnly();
            var oldItem = new TestMethodDefinition().Set(x => x.Parameters = oldParameters);
            var newParameter = new TestParameterDefinition();
            var newParameters = new List<IParameterDefinition> { newParameter }.AsReadOnly();
            var newItem = oldItem.JsonClone().Set(x => x.Parameters = newParameters);
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
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