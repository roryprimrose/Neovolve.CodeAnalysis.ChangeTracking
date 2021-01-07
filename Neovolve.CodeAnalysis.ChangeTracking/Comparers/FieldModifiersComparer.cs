namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldModifiersComparer : ModifiersElementComparer<FieldModifiers>, IFieldModifiersComparer
    {
        public FieldModifiersComparer(IFieldModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}