namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IMethodMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IMethodDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IMethodMatchProcessor : IMatchProcessor<IMethodDefinition>
    {
    }
}