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

    public class MethodComparerTests : Tests<MethodComparer>
    {
        private readonly ITestOutputHelper _output;

        public MethodComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsResultFromMethodModifierComparer()
        {
            var item = new TestMethodDefinition();
            var match = new ItemMatch<IMethodDefinition>(item, item);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] { result };

            Service<IMethodModifiersComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IModifiersElement<MethodModifiers>>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void CompareItemsReturnsBreakingWhenNameChanged()
        {
            var oldItem = new TestMethodDefinition();
            var newItem = oldItem.JsonClone().Set(x => x.Name = "Renamed");
            var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            
            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].ChangeType.Should().BeEquivalentTo(SemVerChangeType.Breaking);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullMethodModifiersChangeTable()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var parameterComparer = Substitute.For<IParameterComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new MethodComparer(accessModifiersComparer, null!, genericTypeElementComparer, parameterComparer, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullGenericTypeElementComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var methodModifiersComparer = Substitute.For<IMethodModifiersComparer>();
            var parameterComparer = Substitute.For<IParameterComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new MethodComparer(accessModifiersComparer, methodModifiersComparer, null!, parameterComparer, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var methodModifiersComparer = Substitute.For<IMethodModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () =>
                new MethodComparer(accessModifiersComparer, methodModifiersComparer, genericTypeElementComparer, null!, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}