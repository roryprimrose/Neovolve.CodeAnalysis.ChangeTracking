namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    /// The <see cref="IEnumMemberMatchProcessor"/>
    /// interface defines the members for processing matches between old and new <see cref="IEnumMemberDefinition"/> items to calculate comparison results.
    /// </summary>
    public interface IEnumMemberMatchProcessor : IMatchProcessor<IEnumMemberDefinition>
    {
    }
}