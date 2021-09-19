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
        public void AddElementAddedResultAddsMessageResultToAggregator()
        {
            var item = new TestClassDefinition();
            var message = Guid.NewGuid().ToString();
            var changeType = Model.Create<SemVerChangeType>();

            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            formatter.FormatItem(item, ItemFormatType.ItemAdded,
                Arg.Is<FormatArguments>(x =>
                    x.MessageFormat == MessagePart.Identifier + " has been added" && x.OldValue == null && x.NewValue == null)).Returns(message);

            aggregator.AddElementAddedResult(changeType, item, formatter);

            aggregator.Received(1).AddResult(Arg.Any<ComparisonResult>());
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.OldItem == null));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.NewItem == item));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.Message == message));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.ChangeType == changeType));
        }

        [Fact]
        public void AddElementAddedResultThrowsExceptionWithNullAggregator()
        {
            var item = new TestClassDefinition();

            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                ChangeResultAggregatorExtensions.AddElementAddedResult(null!, SemVerChangeType.Breaking, item,
                    formatter);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementAddedResultThrowsExceptionWithNullElement()
        {
            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                aggregator.AddElementAddedResult<ITypeDefinition>(SemVerChangeType.Breaking,
                    null!, formatter);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementAddedResultThrowsExceptionWithNullMessageFormatter()
        {
            var item = new TestClassDefinition();

            var aggregator = Substitute.For<IChangeResultAggregator>();

            Action action = () =>
                aggregator.AddElementAddedResult(SemVerChangeType.Breaking, item,
                    null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementChangedResultAddsMessageResultToAggregator()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), oldItem.Name,
                newItem.Name);
            var message = Guid.NewGuid().ToString();
            var changeType = Model.Create<SemVerChangeType>();

            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            formatter.FormatMatch(match, ItemFormatType.ItemChanged, arguments).Returns(message);

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
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), oldItem.Name,
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
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), oldItem.Name,
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
            var arguments = new FormatArguments(Guid.NewGuid().ToString(), oldItem.Name,
                newItem.Name);

            var aggregator = Substitute.For<IChangeResultAggregator>();

            Action action = () =>
                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match,
                    null!, arguments);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementRemovedResultAddsMessageResultToAggregator()
        {
            var item = new TestClassDefinition();
            var message = Guid.NewGuid().ToString();
            var changeType = Model.Create<SemVerChangeType>();

            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            formatter.FormatItem(item, ItemFormatType.ItemRemoved,
                    Arg.Is<FormatArguments>(x =>
                        x.MessageFormat == MessagePart.Identifier + " has been removed" && x.OldValue == null && x.NewValue == null))
                .Returns(message);

            aggregator.AddElementRemovedResult(changeType, item, formatter);

            aggregator.Received(1).AddResult(Arg.Any<ComparisonResult>());
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.OldItem == item));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.NewItem == null));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.Message == message));
            aggregator.Received().AddResult(Arg.Is<ComparisonResult>(x => x.ChangeType == changeType));
        }

        [Fact]
        public void AddElementRemovedResultThrowsExceptionWithNullAggregator()
        {
            var item = new TestClassDefinition();

            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                ChangeResultAggregatorExtensions.AddElementRemovedResult(null!, SemVerChangeType.Breaking, item,
                    formatter);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementRemovedResultThrowsExceptionWithNullElement()
        {
            var aggregator = Substitute.For<IChangeResultAggregator>();
            var formatter = Substitute.For<IMessageFormatter>();

            Action action = () =>
                aggregator.AddElementRemovedResult<ITypeDefinition>(SemVerChangeType.Breaking,
                    null!, formatter);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddElementRemovedResultThrowsExceptionWithNullMessageFormatter()
        {
            var item = new TestClassDefinition();

            var aggregator = Substitute.For<IChangeResultAggregator>();

            Action action = () =>
                aggregator.AddElementRemovedResult(SemVerChangeType.Breaking, item,
                    null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}