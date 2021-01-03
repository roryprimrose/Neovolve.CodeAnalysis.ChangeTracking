namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AccessModifiersComparer : AccessModifiersElementComparer<AccessModifiers>, IAccessModifiersComparer
    {
        public AccessModifiersComparer(IAccessModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
    public class ClassModifiersComparer : ModifiersElementComparer<ClassModifiers>, IClassModifiersComparer
    {
        public ClassModifiersComparer(IClassModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}