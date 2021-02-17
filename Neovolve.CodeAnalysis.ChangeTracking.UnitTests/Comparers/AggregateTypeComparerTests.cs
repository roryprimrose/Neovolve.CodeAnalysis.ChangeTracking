namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class AggregateTypeComparerTests : Tests<AggregateTypeComparer>
    {
        private readonly ITestOutputHelper _output;

        public AggregateTypeComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsBreakingChangeWhenTypeChangedToClass()
        {
            var oldItem = new TestStructDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CompareItemsReturnsBreakingChangeWhenTypeChangedToInterface()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestInterfaceDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CompareItemsReturnsBreakingChangeWhenTypeChangedToStruct()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestStructDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CompareItemsReturnsClassComparerResult()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IClassComparer>()
                .CompareItems(Arg.Is<ItemMatch<IClassDefinition>>(x => x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].Should().Be(result);
        }

        [Fact]
        public void CompareItemsReturnsInterfaceComparerResult()
        {
            var oldItem = new TestInterfaceDefinition();
            var newItem = new TestInterfaceDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IInterfaceComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IInterfaceDefinition>>(x => x.OldItem == oldItem && x.NewItem == newItem), options)
                .Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].Should().Be(result);
        }

        [Fact]
        public void CompareItemsReturnsStructComparerResult()
        {
            var oldItem = new TestStructDefinition();
            var newItem = new TestStructDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IStructComparer>()
                .CompareItems(Arg.Is<ItemMatch<IStructDefinition>>(x => x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            
            actual[0].Should().Be(result);
        }

        [Fact]
        public void CompareItemsThrowsExceptionWhenTypedChangedToUnsupportedType()
        {
            var oldItem = new TestStructDefinition();
            var newItem = Substitute.For<ITypeDefinition>();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();

            Action action = () => SUT.CompareItems(match, options);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullCompareOptions()
        {
            var oldItem = new TestClassDefinition();
            var newItem = new TestClassDefinition();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);

            Action action = () => SUT.CompareItems(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullMatch()
        {
            var options = Model.Create<ComparerOptions>();

            Action action = () => SUT.CompareItems(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithUnsupportedTypeDefinitions()
        {
            var oldItem = Substitute.For<ITypeDefinition>();
            var newItem = Substitute.For<ITypeDefinition>();
            var match = new ItemMatch<ITypeDefinition>(oldItem, newItem);
            var options = Model.Create<ComparerOptions>();

            Action action = () => SUT.CompareItems(match, options);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullClassComparer()
        {
            var interfaceComparer = Substitute.For<IInterfaceComparer>();
            var structComparer = Substitute.For<IStructComparer>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AggregateTypeComparer(null!, interfaceComparer, structComparer);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullInterfaceComparer()
        {
            var classComparer = Substitute.For<IClassComparer>();
            var structComparer = Substitute.For<IStructComparer>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AggregateTypeComparer(classComparer, null!, structComparer);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullStructComparer()
        {
            var interfaceComparer = Substitute.For<IInterfaceComparer>();
            var classComparer = Substitute.For<IClassComparer>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AggregateTypeComparer(classComparer, interfaceComparer, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}