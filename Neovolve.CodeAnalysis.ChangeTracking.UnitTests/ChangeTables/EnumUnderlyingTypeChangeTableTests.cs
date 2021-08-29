namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Xunit;

    public class EnumUnderlyingTypeChangeTableTests
    {
        [Theory]
        // @formatter:off — disable formatter after this line
        [InlineData("", "", SemVerChangeType.None)]
        [InlineData("", "byte", SemVerChangeType.Breaking)]
        [InlineData("", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("", "short", SemVerChangeType.Breaking)]
        [InlineData("", "ushort", SemVerChangeType.Breaking)]
        [InlineData("", "int", SemVerChangeType.None)]
        [InlineData("", "uint", SemVerChangeType.Breaking)]
        [InlineData("", "long", SemVerChangeType.Breaking)]
        [InlineData("", "ulong", SemVerChangeType.Breaking)]
        [InlineData("byte", "", SemVerChangeType.Breaking)]
        [InlineData("byte", "byte", SemVerChangeType.None)]
        [InlineData("byte", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("byte", "short", SemVerChangeType.Breaking)]
        [InlineData("byte", "ushort", SemVerChangeType.Breaking)]
        [InlineData("byte", "int", SemVerChangeType.Breaking)]
        [InlineData("byte", "uint", SemVerChangeType.Breaking)]
        [InlineData("byte", "long", SemVerChangeType.Breaking)]
        [InlineData("byte", "ulong", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "byte", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "sbyte", SemVerChangeType.None)]
        [InlineData("sbyte", "short", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "ushort", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "int", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "uint", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "long", SemVerChangeType.Breaking)]
        [InlineData("sbyte", "ulong", SemVerChangeType.Breaking)]
        [InlineData("short", "", SemVerChangeType.Breaking)]
        [InlineData("short", "byte", SemVerChangeType.Breaking)]
        [InlineData("short", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("short", "short", SemVerChangeType.None)]
        [InlineData("short", "ushort", SemVerChangeType.Breaking)]
        [InlineData("short", "int", SemVerChangeType.Breaking)]
        [InlineData("short", "uint", SemVerChangeType.Breaking)]
        [InlineData("short", "long", SemVerChangeType.Breaking)]
        [InlineData("short", "ulong", SemVerChangeType.Breaking)]
        [InlineData("ushort", "", SemVerChangeType.Breaking)]
        [InlineData("ushort", "byte", SemVerChangeType.Breaking)]
        [InlineData("ushort", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("ushort", "short", SemVerChangeType.Breaking)]
        [InlineData("ushort", "ushort", SemVerChangeType.None)]
        [InlineData("ushort", "int", SemVerChangeType.Breaking)]
        [InlineData("ushort", "uint", SemVerChangeType.Breaking)]
        [InlineData("ushort", "long", SemVerChangeType.Breaking)]
        [InlineData("ushort", "ulong", SemVerChangeType.Breaking)]
        [InlineData("int", "", SemVerChangeType.None)]
        [InlineData("int", "byte", SemVerChangeType.Breaking)]
        [InlineData("int", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("int", "short", SemVerChangeType.Breaking)]
        [InlineData("int", "ushort", SemVerChangeType.Breaking)]
        [InlineData("int", "int", SemVerChangeType.None)]
        [InlineData("int", "uint", SemVerChangeType.Breaking)]
        [InlineData("int", "long", SemVerChangeType.Breaking)]
        [InlineData("int", "ulong", SemVerChangeType.Breaking)]
        [InlineData("uint", "", SemVerChangeType.Breaking)]
        [InlineData("uint", "byte", SemVerChangeType.Breaking)]
        [InlineData("uint", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("uint", "short", SemVerChangeType.Breaking)]
        [InlineData("uint", "ushort", SemVerChangeType.Breaking)]
        [InlineData("uint", "int", SemVerChangeType.Breaking)]
        [InlineData("uint", "uint", SemVerChangeType.None)]
        [InlineData("uint", "long", SemVerChangeType.Breaking)]
        [InlineData("uint", "ulong", SemVerChangeType.Breaking)]
        [InlineData("long", "", SemVerChangeType.Breaking)]
        [InlineData("long", "byte", SemVerChangeType.Breaking)]
        [InlineData("long", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("long", "short", SemVerChangeType.Breaking)]
        [InlineData("long", "ushort", SemVerChangeType.Breaking)]
        [InlineData("long", "int", SemVerChangeType.Breaking)]
        [InlineData("long", "uint", SemVerChangeType.Breaking)]
        [InlineData("long", "long", SemVerChangeType.None)]
        [InlineData("long", "ulong", SemVerChangeType.Breaking)]
        [InlineData("ulong", "", SemVerChangeType.Breaking)]
        [InlineData("ulong", "byte", SemVerChangeType.Breaking)]
        [InlineData("ulong", "sbyte", SemVerChangeType.Breaking)]
        [InlineData("ulong", "short", SemVerChangeType.Breaking)]
        [InlineData("ulong", "ushort", SemVerChangeType.Breaking)]
        [InlineData("ulong", "int", SemVerChangeType.Breaking)]
        [InlineData("ulong", "uint", SemVerChangeType.Breaking)]
        [InlineData("ulong", "long", SemVerChangeType.Breaking)]
        [InlineData("ulong", "ulong", SemVerChangeType.None)]
        // @formatter:on — enable formatter after this line
        public void CalculateChangeReturnsExpectedValue(
            string oldValue,
            string newValue,
            SemVerChangeType expected)
        {
            var sut = new EnumUnderlyingTypeChangeTable();

            var actual = sut.CalculateChange(oldValue, newValue);

            actual.Should().Be(expected);
        }
    }
}