namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IConstructorEvaluator" />
    ///     interface defines the members for identifying matches between old and new <see cref="IConstructorDefinition"/> items.
    /// </summary>
    public interface IConstructorEvaluator : IEvaluator<IConstructorDefinition>
    {
    }
}