namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    public class EnumUnderlyingTypeChangeTable : ChangeTable<string>, IEnumUnderlyingTypeChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line

            AddChange("", "byte", SemVerChangeType.Breaking);
            AddChange("", "sbyte", SemVerChangeType.Breaking);
            AddChange("", "short", SemVerChangeType.Breaking);
            AddChange("", "ushort", SemVerChangeType.Breaking);
            AddChange("", "int", SemVerChangeType.None);
            AddChange("", "uint", SemVerChangeType.Breaking);
            AddChange("", "long", SemVerChangeType.Breaking);
            AddChange("", "ulong", SemVerChangeType.Breaking);
            AddChange("byte", "", SemVerChangeType.Breaking);
            AddChange("byte", "sbyte", SemVerChangeType.Breaking);
            AddChange("byte", "short", SemVerChangeType.Breaking);
            AddChange("byte", "ushort", SemVerChangeType.Breaking);
            AddChange("byte", "int", SemVerChangeType.Breaking);
            AddChange("byte", "uint", SemVerChangeType.Breaking);
            AddChange("byte", "long", SemVerChangeType.Breaking);
            AddChange("byte", "ulong", SemVerChangeType.Breaking);
            AddChange("sbyte", "", SemVerChangeType.Breaking);
            AddChange("sbyte", "byte", SemVerChangeType.Breaking);
            AddChange("sbyte", "short", SemVerChangeType.Breaking);
            AddChange("sbyte", "ushort", SemVerChangeType.Breaking);
            AddChange("sbyte", "int", SemVerChangeType.Breaking);
            AddChange("sbyte", "uint", SemVerChangeType.Breaking);
            AddChange("sbyte", "long", SemVerChangeType.Breaking);
            AddChange("sbyte", "ulong", SemVerChangeType.Breaking);
            AddChange("short", "", SemVerChangeType.Breaking);
            AddChange("short", "byte", SemVerChangeType.Breaking);
            AddChange("short", "sbyte", SemVerChangeType.Breaking);
            AddChange("short", "ushort", SemVerChangeType.Breaking);
            AddChange("short", "int", SemVerChangeType.Breaking);
            AddChange("short", "uint", SemVerChangeType.Breaking);
            AddChange("short", "long", SemVerChangeType.Breaking);
            AddChange("short", "ulong", SemVerChangeType.Breaking);
            AddChange("ushort", "", SemVerChangeType.Breaking);
            AddChange("ushort", "byte", SemVerChangeType.Breaking);
            AddChange("ushort", "sbyte", SemVerChangeType.Breaking);
            AddChange("ushort", "short", SemVerChangeType.Breaking);
            AddChange("ushort", "int", SemVerChangeType.Breaking);
            AddChange("ushort", "uint", SemVerChangeType.Breaking);
            AddChange("ushort", "long", SemVerChangeType.Breaking);
            AddChange("ushort", "ulong", SemVerChangeType.Breaking);
            AddChange("int", "", SemVerChangeType.None);
            AddChange("int", "byte", SemVerChangeType.Breaking);
            AddChange("int", "sbyte", SemVerChangeType.Breaking);
            AddChange("int", "short", SemVerChangeType.Breaking);
            AddChange("int", "ushort", SemVerChangeType.Breaking);
            AddChange("int", "uint", SemVerChangeType.Breaking);
            AddChange("int", "long", SemVerChangeType.Breaking);
            AddChange("int", "ulong", SemVerChangeType.Breaking);
            AddChange("uint", "", SemVerChangeType.Breaking);
            AddChange("uint", "byte", SemVerChangeType.Breaking);
            AddChange("uint", "sbyte", SemVerChangeType.Breaking);
            AddChange("uint", "short", SemVerChangeType.Breaking);
            AddChange("uint", "ushort", SemVerChangeType.Breaking);
            AddChange("uint", "int", SemVerChangeType.Breaking);
            AddChange("uint", "long", SemVerChangeType.Breaking);
            AddChange("uint", "ulong", SemVerChangeType.Breaking);
            AddChange("long", "", SemVerChangeType.Breaking);
            AddChange("long", "byte", SemVerChangeType.Breaking);
            AddChange("long", "sbyte", SemVerChangeType.Breaking);
            AddChange("long", "short", SemVerChangeType.Breaking);
            AddChange("long", "ushort", SemVerChangeType.Breaking);
            AddChange("long", "int", SemVerChangeType.Breaking);
            AddChange("long", "uint", SemVerChangeType.Breaking);
            AddChange("long", "ulong", SemVerChangeType.Breaking);
            AddChange("ulong", "", SemVerChangeType.Breaking);
            AddChange("ulong", "byte", SemVerChangeType.Breaking);
            AddChange("ulong", "sbyte", SemVerChangeType.Breaking);
            AddChange("ulong", "short", SemVerChangeType.Breaking);
            AddChange("ulong", "ushort", SemVerChangeType.Breaking);
            AddChange("ulong", "int", SemVerChangeType.Breaking);
            AddChange("ulong", "uint", SemVerChangeType.Breaking);
            AddChange("ulong", "long", SemVerChangeType.Breaking);
            // @formatter:on — enable formatter after this line
        }
    }
}