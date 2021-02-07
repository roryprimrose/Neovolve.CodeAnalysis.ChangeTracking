namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ParameterModifierChangeTable : ChangeTable<ParameterModifier>, IParameterModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(ParameterModifier.None, ParameterModifier.None, SemVerChangeType.None);
            AddChange(ParameterModifier.None, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.None, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.None, ParameterModifier.This, SemVerChangeType.Feature);
            AddChange(ParameterModifier.None, ParameterModifier.Params, SemVerChangeType.Feature);
            AddChange(ParameterModifier.Ref, ParameterModifier.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Ref, ParameterModifier.Ref, SemVerChangeType.None);
            AddChange(ParameterModifier.Ref, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Ref, ParameterModifier.This, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Ref, ParameterModifier.Params, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Out, ParameterModifier.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Out, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Out, ParameterModifier.Out, SemVerChangeType.None);
            AddChange(ParameterModifier.Out, ParameterModifier.This, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Out, ParameterModifier.Params, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.This, ParameterModifier.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.This, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.This, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.This, ParameterModifier.This, SemVerChangeType.None);
            AddChange(ParameterModifier.This, ParameterModifier.Params, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Params, ParameterModifier.None, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Params, ParameterModifier.Ref, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Params, ParameterModifier.Out, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Params, ParameterModifier.This, SemVerChangeType.Breaking);
            AddChange(ParameterModifier.Params, ParameterModifier.Params, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line
        }
    }
}