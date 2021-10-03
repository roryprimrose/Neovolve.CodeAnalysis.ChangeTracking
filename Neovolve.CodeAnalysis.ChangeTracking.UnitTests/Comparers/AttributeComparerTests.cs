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
        public void CompareMatchReturnsBreakingWhenMultipleArgumentsAdded()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named),
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Ordinal)
                }
            };
            var newItem = oldItem.JsonClone();

            oldItem.Arguments = new List<IArgumentDefinition>();

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleArgumentsRemoved()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named),
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Ordinal)
                }
            };
            var newItem = oldItem.JsonClone();

            newItem.Arguments = new List<IArgumentDefinition>();

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleNamedArgumentsAdded()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named),
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
                }
            };
            var newItem = oldItem.JsonClone();

            oldItem.Arguments = new List<IArgumentDefinition>();

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenMultipleNamedArgumentsRemoved()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named),
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
                }
            };
            var newItem = oldItem.JsonClone();

            newItem.Arguments = new List<IArgumentDefinition>();

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamedArgumentAdded()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
                }
            };
            var newItem = oldItem.JsonClone();

            oldItem.Arguments = new List<IArgumentDefinition>();

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeNull();
            actual[0].NewItem.As<IArgumentDefinition>().ParameterName.Should()
                .Be(newItem.Arguments.First().ParameterName);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamedArgumentAddedAndOtherNamedArgumentRenamed()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named),
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
                }
            };
            var newItem = oldItem.JsonClone();

            oldItem.Arguments = new List<IArgumentDefinition>
            {
                new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
            };

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamedArgumentParameterNameChanged()
        {
            var oldArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Named};
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = new TestAttributeDefinition {Arguments = oldArguments};
            var newArgument = new TestArgumentDefinition
            {
                ArgumentType = ArgumentType.Named,
                Value = oldArgument.Value
            };
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = new TestAttributeDefinition {Arguments = newArguments};
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeNull();
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamedArgumentRemoved()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
                }
            };
            var newItem = oldItem.JsonClone();

            newItem.Arguments = new List<IArgumentDefinition>();

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.As<IArgumentDefinition>().ParameterName.Should()
                .Be(oldItem.Arguments.First().ParameterName);
            actual[0].NewItem.Should().BeNull();
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamedArgumentRemovedAndOtherNamedArgumentRenamed()
        {
            var oldItem = new TestAttributeDefinition
            {
                Arguments = new[]
                {
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named),
                    new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
                }
            };
            var newItem = oldItem.JsonClone();

            newItem.Arguments = new List<IArgumentDefinition>
            {
                new TestArgumentDefinition().Set(x => x.ArgumentType = ArgumentType.Named)
            };

            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamedArgumentValueChanged()
        {
            var oldArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Named};
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = new TestAttributeDefinition {Arguments = oldArguments};
            var newArgument = new TestArgumentDefinition
            {
                ArgumentType = ArgumentType.Named,
                ParameterName = oldArgument.ParameterName
            };
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = new TestAttributeDefinition {Arguments = newArguments};
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeAssignableTo<IArgumentDefinition>();
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenOrdinalArgumentsAdded()
        {
            var oldArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Named};
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = new TestAttributeDefinition {Arguments = oldArguments};
            var newArgument = new TestArgumentDefinition
            {
                ArgumentType = ArgumentType.Ordinal,
                Value = oldArgument.Value
            };
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = new TestAttributeDefinition {Arguments = newArguments};
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldAttribute);
            actual[0].NewItem.Should().Be(newAttribute);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenOrdinalArgumentsRemoved()
        {
            var oldArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Ordinal};
            var oldArguments = new[]
            {
                oldArgument
            };
            var newArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Named};
            var newArguments = new[]
            {
                newArgument
            };
            var oldAttribute = new TestAttributeDefinition {Arguments = oldArguments};
            var newAttribute = new TestAttributeDefinition {Arguments = newArguments};
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldAttribute);
            actual[0].NewItem.Should().Be(newAttribute);
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenOrdinalArgumentValueChanged()
        {
            var oldArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Ordinal};
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = new TestAttributeDefinition {Arguments = oldArguments};
            var newArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Ordinal};
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = new TestAttributeDefinition {Arguments = newArguments};
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeAssignableTo<IArgumentDefinition>();
        }

        [Fact]
        public void CompareMatchReturnsEmptyChangesWhenAttributesDoNotHaveArguments()
        {
            var oldItem = new TestAttributeDefinition();
            var newItem = new TestAttributeDefinition();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsEmptyResultsWhenNoChangeFound()
        {
            var ordinalArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Ordinal};
            var namedArgument = new TestArgumentDefinition {ArgumentType = ArgumentType.Named};
            var oldArguments = new[]
            {
                ordinalArgument,
                namedArgument
            };
            var attribute = new TestAttributeDefinition {Arguments = oldArguments};
            var match = new ItemMatch<IAttributeDefinition>(attribute, attribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareMatch(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullMatch()
        {
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            Action action = () => sut.CompareMatch(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareMatchThrowsExceptionWithNullOptions()
        {
            var oldItem = new TestAttributeDefinition();
            var newItem = new TestAttributeDefinition();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);

            var sut = new AttributeComparer();

            Action action = () => sut.CompareMatch(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}