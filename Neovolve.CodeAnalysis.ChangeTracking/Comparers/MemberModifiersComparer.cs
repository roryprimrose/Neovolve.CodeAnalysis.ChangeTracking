namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MemberModifiersComparer : ModifiersElementComparer<MemberModifiers>, IMemberModifiersComparer
    {
        public MemberModifiersComparer(IMemberModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}