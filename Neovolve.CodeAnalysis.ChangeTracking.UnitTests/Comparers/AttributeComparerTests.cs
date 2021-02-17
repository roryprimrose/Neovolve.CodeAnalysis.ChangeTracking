namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
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
        public void CompareItemsReturnsBreakingWhenArgumentsAdded()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Ignoring<TestAttributeDefinition>(x => x.Arguments)
                .Create<TestAttributeDefinition>();
            var newItem = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenArgumentsRemoved()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>();
            var newItem = Model.UsingModule<ConfigurationModule>().Ignoring<TestAttributeDefinition>(x => x.Arguments)
                .Create<IAttributeDefinition>();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldItem);
            actual[0].NewItem.Should().Be(newItem);
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenNamedArgumentParameterNameChanged()
        {
            var oldArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Named);
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = oldArguments);
            var newArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>().Set(
                x =>
                {
                    x.ArgumentType = ArgumentType.Named;
                    x.Value = oldArgument.Value;
                });
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = newArguments);
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeNull();
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenNamedArgumentValueChanged()
        {
            var oldArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Named);
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = oldArguments);
            var newArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>().Set(
                x =>
                {
                    x.ArgumentType = ArgumentType.Named;
                    x.ParameterName = oldArgument.ParameterName;
                });
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = newArguments);
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeAssignableTo<IArgumentDefinition>();
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenOrdinalArgumentsAdded()
        {
            var oldArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Named);
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = oldArguments);
            var newArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>().Set(
                x =>
                {
                    x.ArgumentType = ArgumentType.Ordinal;
                    x.Value = oldArgument.Value;
                });
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = newArguments);
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldAttribute);
            actual[0].NewItem.Should().Be(newAttribute);
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenOrdinalArgumentsRemoved()
        {
            var oldArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Ordinal);
            var oldArguments = new[]
            {
                oldArgument
            };
            var newArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Named);
            var newArguments = new[]
            {
                newArgument
            };
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = oldArguments);
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = newArguments);
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().Be(oldAttribute);
            actual[0].NewItem.Should().Be(newAttribute);
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenOrdinalArgumentValueChanged()
        {
            var oldArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Ordinal);
            var oldArguments = new[]
            {
                oldArgument
            };
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = oldArguments);
            var newArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>().Set(
                x => { x.ArgumentType = ArgumentType.Ordinal; });
            var newArguments = new[]
            {
                newArgument
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = newArguments);
            var match = new ItemMatch<IAttributeDefinition>(oldAttribute, newAttribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].OldItem.Should().BeAssignableTo<IArgumentDefinition>();
            actual[0].NewItem.Should().BeAssignableTo<IArgumentDefinition>();
        }

        [Fact]
        public void CompareItemsReturnsEmptyChangesWhenAttributesDoNotHaveArguments()
        {
            var oldItem = Model.UsingModule<ConfigurationModule>().Ignoring<TestAttributeDefinition>(x => x.Arguments)
                .Create<TestAttributeDefinition>();
            var newItem = Model.UsingModule<ConfigurationModule>().Ignoring<TestAttributeDefinition>(x => x.Arguments)
                .Create<TestAttributeDefinition>();
            var match = new ItemMatch<IAttributeDefinition>(oldItem, newItem);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsEmptyResultsWhenNoChangeFound()
        {
            var ordinalArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Ordinal);
            var namedArgument = Model.UsingModule<ConfigurationModule>().Create<TestArgumentDefinition>()
                .Set(x => x.ArgumentType = ArgumentType.Named);
            var oldArguments = new[]
            {
                ordinalArgument,
                namedArgument
            };
            var attribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Arguments = oldArguments);
            var match = new ItemMatch<IAttributeDefinition>(attribute, attribute);
            var options = OptionsFactory.BuildOptions();

            var sut = new AttributeComparer();

            var actual = sut.CompareItems(match, options).ToList();

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