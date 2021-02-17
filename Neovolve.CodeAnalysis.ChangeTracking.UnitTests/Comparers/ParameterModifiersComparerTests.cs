namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using NSubstitute;
    using Xunit;

    public class ParameterModifiersComparerTests
    {
        [Fact]
        public void CanCreateWithChangeTable()
        {
            var changeTable = Substitute.For<IParameterModifiersChangeTable>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterModifiersComparer(changeTable);

            action.Should().NotThrow();
        }
    }
}