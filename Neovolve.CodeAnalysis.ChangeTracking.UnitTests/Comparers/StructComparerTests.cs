namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
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

    public class StructComparerTests : Tests<StructComparer>
    {
        private readonly ITestOutputHelper _output;

        public StructComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanCreateWithDependencies()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var structModifiersComparer = Substitute.For<IStructModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var fieldProcessor = Substitute.For<IFieldMatchProcessor>();
            var constructorProcessor = Substitute.For<IConstructorMatchProcessor>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new StructComparer(accessModifiersComparer, structModifiersComparer,
                genericTypeElementComparer,
                fieldProcessor, constructorProcessor, propertyProcessor,
                methodProcessor, attributeProcessor);

            action.Should().NotThrow();
        }

        [Fact]
        public void CompareMatchReturnsResultFromConstructorMatchProcessor()
        {
            var oldItem = new TestStructDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IStructDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IConstructorMatchProcessor>()
                .CalculateChanges(oldItem.Constructors,
                    newItem.Constructors,
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromFieldMatchProcessor()
        {
            var oldItem = new TestStructDefinition();
            var newItem = oldItem.JsonClone();
            var match = new ItemMatch<IStructDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldItem, newItem, message);
            var results = new[] {result};

            Service<IFieldMatchProcessor>()
                .CalculateChanges(oldItem.Fields,
                    newItem.Fields,
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareMatchReturnsResultFromStructModifierComparer()
        {
            var item = new TestStructDefinition();
            var match = new ItemMatch<IStructDefinition>(item, item);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] {result};

            Service<IStructModifiersComparer>()
                .CompareMatch(
                    Arg.Is<ItemMatch<IModifiersElement<StructModifiers>>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullConstructorMatchProcessor()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var structModifiersComparer = Substitute.For<IStructModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var fieldProcessor = Substitute.For<IFieldMatchProcessor>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new StructComparer(accessModifiersComparer, structModifiersComparer,
                genericTypeElementComparer,
                fieldProcessor, null!, propertyProcessor, methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullFieldMatchProcessor()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var structModifiersComparer = Substitute.For<IStructModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var constructorProcessor = Substitute.For<IConstructorMatchProcessor>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new StructComparer(accessModifiersComparer, structModifiersComparer,
                genericTypeElementComparer,
                null!, constructorProcessor, propertyProcessor, methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullStructModifiersComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var constructorProcessor = Substitute.For<IConstructorMatchProcessor>();
            var fieldProcessor = Substitute.For<IFieldMatchProcessor>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new StructComparer(accessModifiersComparer, null!, genericTypeElementComparer,
                fieldProcessor, constructorProcessor, propertyProcessor,
                methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}