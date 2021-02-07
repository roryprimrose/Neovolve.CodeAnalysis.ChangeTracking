namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AccessModifiersChangeTable : ChangeTable<AccessModifiers>, IAccessModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(AccessModifiers.Internal, AccessModifiers.Internal, SemVerChangeType.None);
            AddChange(AccessModifiers.Internal, AccessModifiers.Private, SemVerChangeType.None);
            AddChange(AccessModifiers.Internal, AccessModifiers.Protected, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Internal, AccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Internal, AccessModifiers.ProtectedInternal, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Internal, AccessModifiers.ProtectedPrivate, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Private, AccessModifiers.Internal, SemVerChangeType.None);
            AddChange(AccessModifiers.Private, AccessModifiers.Private, SemVerChangeType.None);
            AddChange(AccessModifiers.Private, AccessModifiers.Protected, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Private, AccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Private, AccessModifiers.ProtectedInternal, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Private, AccessModifiers.ProtectedPrivate, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Protected, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.Protected, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.Protected, AccessModifiers.Protected, SemVerChangeType.None);
            AddChange(AccessModifiers.Protected, AccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(AccessModifiers.Protected, AccessModifiers.ProtectedInternal, SemVerChangeType.None);
            AddChange(AccessModifiers.Protected, AccessModifiers.ProtectedPrivate, SemVerChangeType.None);
            AddChange(AccessModifiers.Public, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.Public, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.Public, AccessModifiers.Protected, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.Public, AccessModifiers.Public, SemVerChangeType.None);
            AddChange(AccessModifiers.Public, AccessModifiers.ProtectedInternal, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.Public, AccessModifiers.ProtectedPrivate, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.ProtectedInternal, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.ProtectedInternal, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.ProtectedInternal, AccessModifiers.Protected, SemVerChangeType.None);
            AddChange(AccessModifiers.ProtectedInternal, AccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(AccessModifiers.ProtectedInternal, AccessModifiers.ProtectedInternal, SemVerChangeType.None);
            AddChange(AccessModifiers.ProtectedInternal, AccessModifiers.ProtectedPrivate, SemVerChangeType.None);
            AddChange(AccessModifiers.ProtectedPrivate, AccessModifiers.Internal, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.ProtectedPrivate, AccessModifiers.Private, SemVerChangeType.Breaking);
            AddChange(AccessModifiers.ProtectedPrivate, AccessModifiers.Protected, SemVerChangeType.None);
            AddChange(AccessModifiers.ProtectedPrivate, AccessModifiers.Public, SemVerChangeType.Feature);
            AddChange(AccessModifiers.ProtectedPrivate, AccessModifiers.ProtectedInternal, SemVerChangeType.None);
            AddChange(AccessModifiers.ProtectedPrivate, AccessModifiers.ProtectedPrivate, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line
        }
    }
}