namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class ChangeResultAggregatorExtensionsTests
    {
        [Fact]
        public void AddElementChangedResultAddsMessageUsingOldItemResultToAggregator()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), oldItem.Name,
                newItem.Name);
            var message = Guid.NewGuid().ToString();
            var changeType = Model.Create<SemVerChangeType>();

            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            formatter.FormatMessage(match.OldItem, arguments).Returns(message);

            aggregator.AddElementChangedResult(changeType, match, formatter, arguments);

            aggregator.Received(1).AddResult(Arg.Any<ComparisonResult>());
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.OldItem == oldItem));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.NewItem == newItem));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.Message == message));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.ChangeType == changeType));
        }

        [Fact]
        public void AddElementChangedResultThrowsExceptionWithNullAggregator()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), oldItem.Name,
                newItem.Name);

            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                ChangeResultAggregatorExtensions.AddElementChangedResult(null!, SemVerChangeType.Breaking, match,
                    formatter, arguments);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementChangedResultThrowsExceptionWithNullArguments()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);

            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match,
                    formatter, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementChangedResultThrowsExceptionWithNullMatch()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), oldItem.Name,
                newItem.Name);

            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                aggregator.AddElementChangedResult<ITypeDefinition>(SemVerChangeType.Breaking,
                    null!, formatter, arguments);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementChangedResultThrowsExceptionWithNullMessageFormatter()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), oldItem.Name,
                newItem.Name);

            var aggregator = Substitute.For<IChangeResultAggregator>();

            Action action = () =>
                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match,
                    null!, arguments);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}