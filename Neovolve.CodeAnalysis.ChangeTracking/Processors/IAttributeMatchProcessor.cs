namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IAttributeMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IAttributeDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IAttributeMatchProcessor : IMatchProcessor<IAttributeDefinition>
    {
    }
}