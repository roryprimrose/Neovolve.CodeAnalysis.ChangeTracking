namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    public interface IBaseTypeDefinition<out T> : IBaseTypeDefinition, IAccessModifiersElement<T> where T : struct, Enum
    {
    }
}