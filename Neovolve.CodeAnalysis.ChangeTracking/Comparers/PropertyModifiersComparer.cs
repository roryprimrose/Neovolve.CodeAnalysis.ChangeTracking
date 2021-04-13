namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyModifiersComparer : ModifiersElementComparer<PropertyModifiers>, IPropertyModifiersComparer
    {
        public PropertyModifiersComparer(IPropertyModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}