namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class ClassModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IClassModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}