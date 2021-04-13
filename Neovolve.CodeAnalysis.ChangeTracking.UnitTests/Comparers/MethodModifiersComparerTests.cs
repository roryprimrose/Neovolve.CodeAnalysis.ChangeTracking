namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class MethodModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IMethodModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new MethodModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}