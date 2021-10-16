namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IParameterMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IParameterDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IParameterMatchProcessor : IMatchProcessor<IParameterDefinition>
    {

    }
}