namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IBaseTypeComparer<T> : IElementComparer<T> where T : IBaseTypeDefinition
    {
    }
}