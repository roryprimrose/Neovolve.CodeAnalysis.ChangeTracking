namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class GenericTypeElementComparerTests : Tests<GenericTypeElementComparer>
    {
        private readonly ITestOutputHelper _output;

        public GenericTypeElementComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericConstraintsAdded()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var newConstraints = new List<IConstraintListDefinition> {newConstraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => { x.GenericTypeParameters = parameters; });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = newConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericTypeParameterAdded()
        {
            var parameters = new List<string> {"TKey", "TValue"}.AsReadOnly();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericTypeParameterAddedWithGenericConstraints()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var constraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var constraints = new List<IConstraintListDefinition> {constraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = constraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericTypeParameterRemoved()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericTypeParameterRemovedWithGenericConstraints()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var constraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var constraints = new List<IConstraintListDefinition> {constraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = constraints;
            });
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSomeGenericConstraintsAdded()
        {
            var oldParameters = new List<string> {"T"}.AsReadOnly();
            var newParameters = new List<string> {"T"}.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"Enum"}.AsReadOnly()
            };
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> {oldConstraintList}.AsReadOnly();
            var newConstraints = new List<IConstraintListDefinition> {newConstraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldParameters;
                x.GenericConstraints = oldConstraints;
            });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = newParameters;
                x.GenericConstraints = newConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenElementIsNotGenericType()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenGenericTypeConstraintsMatch()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var constraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var constraints = new List<IConstraintListDefinition> {constraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = constraints;
            });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = constraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenGenericTypeParametersHaveNoConstraints()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenRenamedGenericTypeConstraintsMatch()
        {
            var oldParameters = new List<string> {"TOld"}.AsReadOnly();
            var newParameters = new List<string> {"TNew"}.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "TOld",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "TNew",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> {oldConstraintList}.AsReadOnly();
            var newConstraints = new List<IConstraintListDefinition> {newConstraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldParameters;
                x.GenericConstraints = oldConstraints;
            });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = newParameters;
                x.GenericConstraints = newConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsFeatureWhenGenericConstraintsRemoved()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> {oldConstraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = oldConstraints;
            });
            var newItem = new TestClassDefinition().Set(x => { x.GenericTypeParameters = parameters; });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareMatchReturnsFeatureWhenSomeGenericConstraintsRemoved()
        {
            var oldParameters = new List<string> {"T"}.AsReadOnly();
            var newParameters = new List<string> {"T"}.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"struct", "Enum"}.AsReadOnly()
            };
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> {"Enum"}.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> {oldConstraintList}.AsReadOnly();
            var newConstraints = new List<IConstraintListDefinition> {newConstraintList}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldParameters;
                x.GenericConstraints = oldConstraints;
            });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = newParameters;
                x.GenericConstraints = newConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullMatch()
        {
            var options = ComparerOptions.Default;

            Action action = () => SUT.CompareMatch(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullOptions()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);

            Action action = () => SUT.CompareMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}