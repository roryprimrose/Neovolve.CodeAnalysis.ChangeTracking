namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class PropertyAccessorAccessModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IPropertyAccessorAccessModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyAccessorAccessModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}