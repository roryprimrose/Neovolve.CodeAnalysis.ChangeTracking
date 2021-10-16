namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IParameterEvaluator" />
    ///     interface defines the members for identifying matches between old and new <see cref="IParameterDefinition"/> items.
    /// </summary>
    public interface IParameterEvaluator : IEvaluator<IParameterDefinition>
    {
    }
}