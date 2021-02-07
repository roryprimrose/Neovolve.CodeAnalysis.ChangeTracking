namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class ChangeTableTests
    {
        [Fact]
        public void CalculateChangeReturnsStoredChange()
        {
            var sut = new Wrapper();

            sut.StoreChange(AccessModifiers.Public, AccessModifiers.Internal, SemVerChangeType.Breaking);

            var expected = sut.CalculateChange(AccessModifiers.Public, AccessModifiers.Internal);

            expected.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CalculateChangeReturnsNoneWhenMatchingValuesProvided()
        {
            var sut = new Wrapper();

            var expected = sut.CalculateChange(AccessModifiers.Public, AccessModifiers.Public);

            expected.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public void CalculateChangeThrowsExceptionWhenNewValueNotStored()
        {
            var sut = new Wrapper();

            sut.StoreChange(AccessModifiers.Public, AccessModifiers.Internal, SemVerChangeType.Breaking);

            Action action = () => sut.CalculateChange(AccessModifiers.Public, AccessModifiers.Private);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CalculateChangeThrowsExceptionWhenOldValueNotStored()
        {
            var sut = new Wrapper();

            Action action = () => sut.CalculateChange(AccessModifiers.Public, AccessModifiers.Internal);

            action.Should().Throw<InvalidOperationException>();
        }

        private class Wrapper : ChangeTable<AccessModifiers>
        {
            public void StoreChange(
                AccessModifiers oldModifiers,
                AccessModifiers newModifiers,
                SemVerChangeType changeType)
            {
                AddChange(oldModifiers, newModifiers, changeType);
            }

            protected override void BuildChanges()
            {
            }
        }
    }
}