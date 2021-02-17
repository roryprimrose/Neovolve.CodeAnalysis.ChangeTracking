namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ParameterModifiersChangeTable : ChangeTable<ParameterModifiers>, IParameterModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(ParameterModifiers.None, ParameterModifiers.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.None, ParameterModifiers.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.None, ParameterModifiers.This, SemVerChangeType.Feature);
            AddChange(ParameterModifiers.None, ParameterModifiers.Params, SemVerChangeType.Feature);
            AddChange(ParameterModifiers.Ref, ParameterModifiers.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Ref, ParameterModifiers.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Ref, ParameterModifiers.This, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Ref, ParameterModifiers.Params, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Out, ParameterModifiers.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Out, ParameterModifiers.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Out, ParameterModifiers.This, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Out, ParameterModifiers.Params, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.This, ParameterModifiers.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.This, ParameterModifiers.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.This, ParameterModifiers.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.This, ParameterModifiers.Params, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Params, ParameterModifiers.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Params, ParameterModifiers.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Params, ParameterModifiers.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifiers.Params, ParameterModifiers.This, SemVerChangeType.Breaking);
            // @formatter:on — enable formatter after this line
        }
    }
}