namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodModifiersComparer : ModifiersElementComparer<MethodModifiers>, IMethodModifiersComparer
    {
        public MethodModifiersComparer(IMethodModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}