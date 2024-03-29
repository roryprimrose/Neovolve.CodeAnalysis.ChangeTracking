﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
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

    public class TypeComparerTests : TestsPartOf<TypeComparer<IClassDefinition>>
    {
        private readonly ITestOutputHelper _output;

        public TypeComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareMatchReturnsBreakingWhenNamespaceChanged()
        {
            var oldItem = new TestClassDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Namespace = Guid.NewGuid().ToString());
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CompareMatchReturnsEmptyWhenTypesAreEqual()
        {
            var item = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(item, item);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options);

            actual.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "", 0)]
        [InlineData("ThisType", "ThisType", 0)]
        [InlineData("ThisType,ThatType", "ThisType,ThatType", 0)]
        [InlineData("ThisType", "ThisType,ThatType", 1)]
        [InlineData("ThisType", "", 1)]
        [InlineData("", "ThisType", 1)]
        [InlineData("", "ThisType,ThatType", 2)]
        [InlineData("ThisType,ThatType", "", 2)]
        [InlineData("ThisType,ThatType", "ThisType", 1)]
        public void CompareMatchReturnsResultBasedOnImplementedTypeChanges(string oldTypes, string newTypes,
            int expected)
        {
            var oldImplementedTypes = oldTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var newImplementedTypes = newTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var oldItem = new TestClassDefinition().Set(x => x.ImplementedTypes = oldImplementedTypes);
            var newItem = oldItem.JsonClone().Set(x => x.ImplementedTypes = newImplementedTypes);
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(expected);
            actual.All(x => x.ChangeType == SemVerChangeType.Breaking).Should().BeTrue();
        }

        [Fact]
        public void CompareMatchReturnsResultFromAccessModifiersComparer()
        {
            var oldItem = new TestClassDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IAccessModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IAccessModifiersElement<AccessModifiers>>>(
                        x => x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromGenericTypeElementComparer()
        {
            var oldItem = new TestClassDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IGenericTypeElementComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IGenericTypeElement>>(
                        x => x.OldItem == oldItem && x.NewItem == newItem),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromMethodMatchProcessor()
        {
            var oldItem = new TestClassDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IMethodMatchProcessor>()
                .CalculateChanges(oldItem.Methods,
                    newItem.Methods,
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromPropertyMatchProcessor()
        {
            var oldItem = new TestClassDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] { result };

            Service<IPropertyMatchProcessor>()
                .CalculateChanges(oldItem.Properties,
                    newItem.Properties,
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultsFromAccessModifierComparer()
        {
            var oldItem = new TestClassDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);
            var options = TestComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var result = new ComparisonResult(changeType, oldItem,
                newItem, Guid.NewGuid().ToString());
            var results = new List<ComparisonResult>
            {
                result
            };

            Service<IAccessModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IAccessModifiersElement<AccessModifiers>>>(x =>
                        x.OldItem == oldItem && x.NewItem == newItem), options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);

            actual[0].ChangeType.Should().Be(changeType);
            actual[0].Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullAccessModifiersComparer()
        {
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!, genericTypeElementComparer,
                propertyProcessor, methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullGenericTypeElementComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(accessModifiersComparer, null!,
                propertyProcessor, methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullMethodProcessor()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(accessModifiersComparer,
                genericTypeElementComparer, propertyProcessor, null!, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyProcessor()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(accessModifiersComparer,
                genericTypeElementComparer, null!, methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : TypeComparer<IClassDefinition>
        {
            public Wrapper(IAccessModifiersComparer accessModifiersComparer,
                IGenericTypeElementComparer genericTypeElementComparer, IPropertyMatchProcessor propertyProcessor,
                IMethodMatchProcessor methodProcessor, IAttributeMatchProcessor attributeProcessor) : base(
                accessModifiersComparer, genericTypeElementComparer, propertyProcessor, methodProcessor,
                attributeProcessor)
            {
            }
        }
    }
}