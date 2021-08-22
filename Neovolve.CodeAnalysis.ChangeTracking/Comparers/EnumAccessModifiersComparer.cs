namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class EnumAccessModifiersComparer : AccessModifiersElementComparer<EnumAccessModifiers>, IEnumAccessModifiersComparer
    {
        public EnumAccessModifiersComparer(IEnumAccessModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}