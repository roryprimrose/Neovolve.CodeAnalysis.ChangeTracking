namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class StructModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IStructModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new StructModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}