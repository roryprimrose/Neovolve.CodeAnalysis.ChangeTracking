namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IEnumMemberEvaluator" />
    ///     interface defines the members for identifying matches between old and new <see cref="IEnumMemberDefinition"/> items.
    /// </summary>
    public interface IEnumMemberEvaluator : IEvaluator<IEnumMemberDefinition>
    {
    }
}