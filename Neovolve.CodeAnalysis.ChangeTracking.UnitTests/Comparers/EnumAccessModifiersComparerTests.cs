namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class EnumAccessModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IEnumAccessModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EnumAccessModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}