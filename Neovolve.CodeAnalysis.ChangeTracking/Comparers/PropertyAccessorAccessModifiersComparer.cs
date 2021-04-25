namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorAccessModifiersComparer :
        AccessModifiersElementComparer<PropertyAccessorAccessModifiers>, IPropertyAccessorAccessModifiersComparer
    {
        public PropertyAccessorAccessModifiersComparer(IPropertyAccessorAccessModifiersChangeTable changeTable) :
            base(changeTable)
        {
        }
    }
}