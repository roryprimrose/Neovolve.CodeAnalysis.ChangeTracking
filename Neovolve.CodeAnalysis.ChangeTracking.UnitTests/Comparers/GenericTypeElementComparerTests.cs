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
        public void CompareMatchReturnsBreakingWhenDifferentGenericConstraintsAddedAndRemoved()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "class", "Stream" }.AsReadOnly()
            };
            var newConstraints = new List<IConstraintListDefinition> { newConstraintList }.AsReadOnly();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = newConstraints;
            });
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "class", "new" }.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> { oldConstraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = oldConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);
            
            actual.Where(x => x.ChangeType == SemVerChangeType.Breaking).Should().HaveCount(1);
            actual.Where(x => x.ChangeType == SemVerChangeType.Feature).Should().HaveCount(1);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericTypeParameterAddedWithGenericConstraints()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var constraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var constraints = new List<IConstraintListDefinition> { constraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = constraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenGenericTypeParameterRemovedWithGenericConstraints()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var constraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var constraints = new List<IConstraintListDefinition> { constraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = constraints;
            });
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleGenericConstraintsAdded()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var newConstraints = new List<IConstraintListDefinition> { newConstraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => { x.GenericTypeParameters = parameters; });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = newConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().NotContain("struct");
            actual[0].Message.Should().NotContain("Enum");
            actual[0].Message.Should().Contain(" 2 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleGenericTypeParameterAdded()
        {
            var parameters = new List<string> { "TKey", "TValue" }.AsReadOnly();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().NotContain("TKey");
            actual[0].Message.Should().NotContain("TValue");
            actual[0].Message.Should().Contain(" 2 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleGenericTypeParameterRemoved()
        {
            var parameters = new List<string> { "TKey", "TValue" }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
            actual[0].Message.Should().NotContain("TKey");
            actual[0].Message.Should().NotContain("TValue");
            actual[0].Message.Should().Contain(" 2 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSingleGenericConstraintsAdded()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "Enum" }.AsReadOnly()
            };
            var newConstraints = new List<IConstraintListDefinition> { newConstraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => { x.GenericTypeParameters = parameters; });
            var newItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = newConstraints;
            });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().Contain("Enum");
            actual[0].Message.Should().NotContain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSingleGenericTypeParameterAdded()
        {
            var parameters = new List<string> { "TValue" }.AsReadOnly();
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("added");
            actual[0].Message.Should().Contain("TValue");
            actual[0].Message.Should().NotContain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSingleGenericTypeParameterRemoved()
        {
            var parameters = new List<string> { "TValue" }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().Contain("removed");
            actual[0].Message.Should().Contain("TValue");
            actual[0].Message.Should().NotContain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenSomeGenericConstraintsAdded()
        {
            var oldParameters = new List<string> { "T" }.AsReadOnly();
            var newParameters = new List<string> { "T" }.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "Enum" }.AsReadOnly()
            };
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> { oldConstraintList }.AsReadOnly();
            var newConstraints = new List<IConstraintListDefinition> { newConstraintList }.AsReadOnly();
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
            var options = TestComparerOptions.Default;

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
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenGenericTypeConstraintsMatch()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var constraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var constraints = new List<IConstraintListDefinition> { constraintList }.AsReadOnly();
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
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenGenericTypeParametersHaveNoConstraints()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var newItem = new TestClassDefinition().Set(x => x.GenericTypeParameters = parameters);
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenRenamedGenericTypeConstraintsMatch()
        {
            var oldParameters = new List<string> { "TOld" }.AsReadOnly();
            var newParameters = new List<string> { "TNew" }.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "TOld",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "TNew",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> { oldConstraintList }.AsReadOnly();
            var newConstraints = new List<IConstraintListDefinition> { newConstraintList }.AsReadOnly();
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
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsFeatureWhenMultipleGenericConstraintsRemoved()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> { oldConstraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = oldConstraints;
            });
            var newItem = new TestClassDefinition().Set(x => { x.GenericTypeParameters = parameters; });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
            actual[0].Message.Should().NotContain("struct");
            actual[0].Message.Should().NotContain("Enum");
            actual[0].Message.Should().Contain(" 2 ");
        }

        [Fact]
        public void CompareMatchReturnsFeatureWhenSingleGenericConstraintsRemoved()
        {
            var parameters = new List<string> { "T" }.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "Enum" }.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> { oldConstraintList }.AsReadOnly();
            var oldItem = new TestClassDefinition().Set(x =>
            {
                x.GenericTypeParameters = parameters;
                x.GenericConstraints = oldConstraints;
            });
            var newItem = new TestClassDefinition().Set(x => { x.GenericTypeParameters = parameters; });
            var match = new ItemMatch<IGenericTypeElement>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
            actual[0].Message.Should().Contain("Enum");
            actual[0].Message.Should().NotContain(" 1 ");
        }

        [Fact]
        public void CompareMatchReturnsFeatureWhenSomeGenericConstraintsRemoved()
        {
            var oldParameters = new List<string> { "T" }.AsReadOnly();
            var newParameters = new List<string> { "T" }.AsReadOnly();
            var oldConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "struct", "Enum" }.AsReadOnly()
            };
            var newConstraintList = new TestConstraintListDefinition
            {
                Name = "T",
                Constraints = new List<string> { "Enum" }.AsReadOnly()
            };
            var oldConstraints = new List<IConstraintListDefinition> { oldConstraintList }.AsReadOnly();
            var newConstraints = new List<IConstraintListDefinition> { newConstraintList }.AsReadOnly();
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
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().Be(SemVerChangeType.Feature);
            actual[0].Message.Should().Contain("removed");
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullMatch()
        {
            var options = TestComparerOptions.Default;

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