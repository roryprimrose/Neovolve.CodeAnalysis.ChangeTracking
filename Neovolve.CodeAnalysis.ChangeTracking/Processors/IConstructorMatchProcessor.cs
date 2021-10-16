namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IConstructorMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IConstructorDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IConstructorMatchProcessor : IMatchProcessor<IConstructorDefinition>
    {
    }
}