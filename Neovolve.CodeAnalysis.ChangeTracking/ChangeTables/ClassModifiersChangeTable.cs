namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ClassModifiersChangeTable : ChangeTable<ClassModifiers>, IClassModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(ClassModifiers.None, ClassModifiers.None, SemVerChangeType.None);
            AddChange(ClassModifiers.None, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.None, ClassModifiers.Partial, SemVerChangeType.None);
            AddChange(ClassModifiers.None, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.None, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.None, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.None, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.None, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Abstract, ClassModifiers.None, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Abstract, ClassModifiers.Abstract, SemVerChangeType.None);
            AddChange(ClassModifiers.Abstract, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Abstract, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Abstract, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Abstract, ClassModifiers.AbstractPartial, SemVerChangeType.None);
            AddChange(ClassModifiers.Abstract, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Abstract, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Partial, ClassModifiers.None, SemVerChangeType.None);
            AddChange(ClassModifiers.Partial, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Partial, ClassModifiers.Partial, SemVerChangeType.None);
            AddChange(ClassModifiers.Partial, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Partial, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Partial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Partial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Partial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Sealed, ClassModifiers.None, SemVerChangeType.Feature);
            AddChange(ClassModifiers.Sealed, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Sealed, ClassModifiers.Partial, SemVerChangeType.Feature);
            AddChange(ClassModifiers.Sealed, ClassModifiers.Sealed, SemVerChangeType.None);
            AddChange(ClassModifiers.Sealed, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Sealed, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Sealed, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Sealed, ClassModifiers.SealedPartial, SemVerChangeType.None);
            AddChange(ClassModifiers.Static, ClassModifiers.None, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Static, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Static, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Static, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Static, ClassModifiers.Static, SemVerChangeType.None);
            AddChange(ClassModifiers.Static, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.Static, ClassModifiers.StaticPartial, SemVerChangeType.None);
            AddChange(ClassModifiers.Static, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.None, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.Abstract, SemVerChangeType.None);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.AbstractPartial, SemVerChangeType.None);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.AbstractPartial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.None, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.Partial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.Static, SemVerChangeType.None);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.StaticPartial, SemVerChangeType.None);
            AddChange(ClassModifiers.StaticPartial, ClassModifiers.SealedPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.None, SemVerChangeType.Feature);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.Partial, SemVerChangeType.Feature);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.Sealed, SemVerChangeType.None);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.Static, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.AbstractPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.StaticPartial, SemVerChangeType.Breaking);
            AddChange(ClassModifiers.SealedPartial, ClassModifiers.SealedPartial, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line
        }
    }
}