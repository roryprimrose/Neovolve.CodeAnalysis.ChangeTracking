namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class FieldModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IFieldModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}