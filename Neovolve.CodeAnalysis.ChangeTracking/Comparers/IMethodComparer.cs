namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IMethodComparer" />
    ///     interface defines the members for comparing <see cref="IMethodDefinition"/> items.
    /// </summary>
    public interface IMethodComparer : IItemComparer<IMethodDefinition>
    {
    }
}