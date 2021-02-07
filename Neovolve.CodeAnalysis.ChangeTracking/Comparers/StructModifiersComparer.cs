namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class StructModifiersComparer : ModifiersElementComparer<StructModifiers>, IStructModifiersComparer
    {
        public StructModifiersComparer(IStructModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}