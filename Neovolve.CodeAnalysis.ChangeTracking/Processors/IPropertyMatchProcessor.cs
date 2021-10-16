namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IPropertyMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IPropertyDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IPropertyMatchProcessor : IMatchProcessor<IPropertyDefinition>
    {
    }
}