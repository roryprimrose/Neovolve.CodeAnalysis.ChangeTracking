namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class ClassModifiersChangeTableTests
    {
        [Theory]
        [InlineData(ClassModifiers.None, ClassModifiers.None, SemVerChangeType.None)]
        [InlineData(ClassModifiers.None, ClassModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.None, ClassModifiers.Partial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.None, ClassModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.None, ClassModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.None, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.None, ClassModifiers.StaticPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.None, ClassModifiers.SealedPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.Abstract, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.Partial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.AbstractPartial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.StaticPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Abstract, ClassModifiers.SealedPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.None, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.Partial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Partial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.None, SemVerChangeType.Feature)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.Partial, SemVerChangeType.Feature)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.Sealed, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.StaticPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Sealed, ClassModifiers.SealedPartial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Static, ClassModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Static, ClassModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Static, ClassModifiers.Partial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Static, ClassModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Static, ClassModifiers.Static, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Static, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.Static, ClassModifiers.StaticPartial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.Static, ClassModifiers.SealedPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.Abstract, SemVerChangeType.None)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.Partial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.AbstractPartial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.AbstractPartial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.Partial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.Static, SemVerChangeType.None)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.StaticPartial, SemVerChangeType.None)]
        [InlineData(ClassModifiers.StaticPartial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.None, SemVerChangeType.Feature)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.Partial, SemVerChangeType.Feature)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.Sealed, SemVerChangeType.None)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking)]
        [InlineData(ClassModifiers.SealedPartial, ClassModifiers.SealedPartial, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValueForMemberDefinition(ClassModifiers oldValue,
            ClassModifiers newValue,
            SemVerChangeType expected)
        {
            var oldItem = Substitute.For<IClassDefinition>();
            var newItem = Substitute.For<IClassDefinition>();

            oldItem.Modifiers.Returns(oldValue);
            newItem.Modifiers.Returns(newValue);

            var match = new ItemMatch<IClassDefinition>(oldItem, newItem);

            var actual = ClassModifiersChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }
    }
}