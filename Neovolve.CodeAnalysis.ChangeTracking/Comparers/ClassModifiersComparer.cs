namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ClassModifiersComparer : ModifiersElementComparer<ClassModifiers>, IClassModifiersComparer
    {
        public ClassModifiersComparer(IClassModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}