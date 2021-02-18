namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using NSubstitute;
    using Xunit;

    public class InterfaceComparerTests
    {
        [Fact]
        public void CanCreateWithDependencies()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var genericTypeElementComparer = Substitute.For<IGenericTypeElementComparer>();
            var propertyProcessor = Substitute.For<IPropertyMatchProcessor>();
            var methodProcessor = Substitute.For<IMethodMatchProcessor>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new InterfaceComparer(accessModifiersComparer, genericTypeElementComparer,
                propertyProcessor,
                methodProcessor, attributeProcessor);

            action.Should().NotThrow();
        }
    }
}