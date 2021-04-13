namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface ITypeComparer<T> : IElementComparer<T> where T : ITypeDefinition
    {
    }
}