﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface ITypeComparer<T> : IBaseTypeComparer<T> where T : ITypeDefinition
    {
    }
}