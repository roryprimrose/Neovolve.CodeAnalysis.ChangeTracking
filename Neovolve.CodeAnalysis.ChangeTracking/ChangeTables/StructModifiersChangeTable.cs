namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    
    public class StructModifiersChangeTable : ChangeTable<StructModifiers>, IStructModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(StructModifiers.None, StructModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddChange(StructModifiers.None, StructModifiers.Partial, SemVerChangeType.None);
            AddChange(StructModifiers.None, StructModifiers.ReadOnlyPartial, SemVerChangeType.Breaking);
            AddChange(StructModifiers.ReadOnly, StructModifiers.None, SemVerChangeType.Feature);
            AddChange(StructModifiers.ReadOnly, StructModifiers.Partial, SemVerChangeType.Feature);
            AddChange(StructModifiers.ReadOnly, StructModifiers.ReadOnlyPartial, SemVerChangeType.None);
            AddChange(StructModifiers.Partial, StructModifiers.None, SemVerChangeType.None);
            AddChange(StructModifiers.Partial, StructModifiers.ReadOnly, SemVerChangeType.Breaking);
            AddChange(StructModifiers.Partial, StructModifiers.ReadOnlyPartial, SemVerChangeType.Breaking);
            AddChange(StructModifiers.ReadOnlyPartial, StructModifiers.None, SemVerChangeType.Feature);
            AddChange(StructModifiers.ReadOnlyPartial, StructModifiers.ReadOnly, SemVerChangeType.None);
            AddChange(StructModifiers.ReadOnlyPartial, StructModifiers.Partial, SemVerChangeType.Feature);
            // @formatter:on — enable formatter after this line
        }
    }
}