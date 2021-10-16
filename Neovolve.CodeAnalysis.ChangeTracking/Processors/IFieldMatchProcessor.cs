namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IFieldMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IFieldDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IFieldMatchProcessor : IMatchProcessor<IFieldDefinition>
    {

    }
}