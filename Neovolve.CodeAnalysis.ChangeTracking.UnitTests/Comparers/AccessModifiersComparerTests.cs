namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class AccessModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IAccessModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new AccessModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}