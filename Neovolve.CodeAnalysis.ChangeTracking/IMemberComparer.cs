namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMemberComparer<T> : IElementComparer<T> where T : IMemberDefinition
    {
    }
}