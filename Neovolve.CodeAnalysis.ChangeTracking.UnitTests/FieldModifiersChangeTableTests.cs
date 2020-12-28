namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class FieldModifiersChangeTableTests
    {
        [Theory]
        [InlineData(FieldModifiers.None, FieldModifiers.None, SemVerChangeType.None)]
        [InlineData(FieldModifiers.None, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.None, SemVerChangeType.Feature)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.None)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.Static, SemVerChangeType.None)]
        [InlineData(FieldModifiers.Static, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.Static, SemVerChangeType.Feature)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.None)]
        public void CalculateChangeReturnsExpectedValue(FieldModifiers oldModifiers,
            FieldModifiers newModifiers,
            SemVerChangeType expected)
        {
            var oldField = new TestFieldDefinition().Set(x => x.Modifiers = oldModifiers);
            var newField = new TestFieldDefinition().Set(x => x.Modifiers = newModifiers);
            var match = new ItemMatch<IFieldDefinition>(oldField, newField);

            var actual = FieldModifiersChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }
    }
}