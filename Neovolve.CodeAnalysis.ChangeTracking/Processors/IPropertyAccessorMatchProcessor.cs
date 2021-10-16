namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IPropertyAccessorMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IPropertyAccessorDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IPropertyAccessorMatchProcessor : IMatchProcessor<IPropertyAccessorDefinition>
    {
    }
}