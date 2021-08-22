namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class EnumAccessModifiersChangeTable : ChangeTable<EnumAccessModifiers>, IEnumAccessModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(EnumAccessModifiers.Internal, EnumAccessModifiers.Private, SemVerChangeType.None);
            AddChange(EnumAccessModifiers.Internal, EnumAccessModifiers.Protected, SemVerChangeType.Feature);
            AddChange(EnumAccessModifiers.Internal, EnumAccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(EnumAccessModifiers.Private, EnumAccessModifiers.Internal, SemVerChangeType.None);
            AddChange(EnumAccessModifiers.Private, EnumAccessModifiers.Protected, SemVerChangeType.Feature);
            AddChange(EnumAccessModifiers.Private, EnumAccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(EnumAccessModifiers.Protected, EnumAccessModifiers.Internal, SemVerChangeType.Breaking);
            AddChange(EnumAccessModifiers.Protected, EnumAccessModifiers.Private, SemVerChangeType.Breaking);
            AddChange(EnumAccessModifiers.Protected, EnumAccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(EnumAccessModifiers.Public, EnumAccessModifiers.Internal, SemVerChangeType.Breaking);
            AddChange(EnumAccessModifiers.Public, EnumAccessModifiers.Private, SemVerChangeType.Breaking);
            AddChange(EnumAccessModifiers.Public, EnumAccessModifiers.Protected, SemVerChangeType.Breaking);
            // @formatter:on — enable formatter after this line
        }
    }
}