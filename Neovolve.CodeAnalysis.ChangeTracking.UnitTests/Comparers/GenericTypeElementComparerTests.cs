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
        public void CompareItemsReturnsBreakingWhenGenericConstraintsAdded()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenGenericTypeParameterAdded()
        {
            var parameters = new List<string> {"TKey", "TValue"}.AsReadOnly();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenGenericTypeParameterAddedWithGenericConstraints()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenGenericTypeParameterRemoved()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenGenericTypeParameterRemovedWithGenericConstraints()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenSomeGenericConstraintsAdded()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenElementIsNotGenericType()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenGenericTypeConstraintsMatch()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenGenericTypeParametersHaveNoConstraints()
        {
            var parameters = new List<string> {"T"}.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenRenamedGenericTypeConstraintsMatch()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsFeatureWhenGenericConstraintsRemoved()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareItemsReturnsFeatureWhenSomeGenericConstraintsRemoved()
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

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullMatch()
        {
            var options = ComparerOptions.Default;

            Action action = () => SUT.CompareItems(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullOptions()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);

            Action action = () => SUT.CompareItems(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}