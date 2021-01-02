namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMemberComparer<T> : IElementComparer<T> where T : IMemberDefinition
    {
    }
}