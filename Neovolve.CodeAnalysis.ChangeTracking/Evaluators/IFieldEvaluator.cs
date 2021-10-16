namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IFieldEvaluator" />
    ///     interface defines the members for identifying matches between old and new <see cref="IFieldDefinition"/> items.
    /// </summary>
    public interface IFieldEvaluator : IEvaluator<IFieldDefinition>
    {
    }
}