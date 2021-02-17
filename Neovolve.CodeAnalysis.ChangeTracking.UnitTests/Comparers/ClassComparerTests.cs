﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
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

    public class ClassComparerTests : Tests<ClassComparer>
    {
        private readonly ITestOutputHelper _output;

        public ClassComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsClassModifierComparerResults()
        {
            var item = new TestClassDefinition();
            var match = new ItemMatch<IClassDefinition>(item, item);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, item, item, message);
            var results = new[] {result};

            Service<IClassModifiersComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IModifiersElement<ClassModifiers>>>(x => x.OldItem == item && x.NewItem == item),
                    options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().BeEquivalentTo(result);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullClassModifiersComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var fieldProcessor = Substitute.For<IFieldMatchProcessor>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassComparer(accessModifiersComparer, null!, genericTypeElementComparer,
                fieldProcessor, propertyProcessor, methodProcessor, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}