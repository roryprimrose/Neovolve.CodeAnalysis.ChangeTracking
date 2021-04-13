namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class PropertyModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IPropertyModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}