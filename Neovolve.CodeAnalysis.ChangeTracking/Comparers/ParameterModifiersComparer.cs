namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ParameterModifiersComparer : ModifiersElementComparer<ParameterModifiers>, IParameterModifiersComparer
    {
        public ParameterModifiersComparer(IParameterModifiersChangeTable changeTable) : base(changeTable)
        {
        }
    }
}