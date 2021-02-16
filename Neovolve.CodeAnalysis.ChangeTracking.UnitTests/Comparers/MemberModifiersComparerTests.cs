namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class MemberModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IMemberModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new MemberModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}