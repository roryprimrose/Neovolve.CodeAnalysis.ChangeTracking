namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IBaseTypeMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IBaseTypeDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IBaseTypeMatchProcessor : IMatchProcessor<IBaseTypeDefinition>
    {

    }
}