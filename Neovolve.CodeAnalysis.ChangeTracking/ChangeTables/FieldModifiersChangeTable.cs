namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldModifiersChangeTable : ChangeTable<FieldModifiers>, IFieldModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(FieldModifiers.None, FieldModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.None, FieldModifiers.Static, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.None, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.ReadOnly, FieldModifiers.None, SemVerChangeType.Feature);
            AddChange(FieldModifiers.ReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.ReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.Static, FieldModifiers.None, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.Static, FieldModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.Static, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.StaticReadOnly, FieldModifiers.None, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.StaticReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddChange(FieldModifiers.StaticReadOnly, FieldModifiers.Static, SemVerChangeType.Feature);
            // @formatter:on — enable formatter after this line
        }
    }
}