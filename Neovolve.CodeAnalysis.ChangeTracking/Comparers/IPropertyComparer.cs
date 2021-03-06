﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IPropertyComparer" />
    ///     interface defines the members for comparing properties.
    /// </summary>
    public interface IPropertyComparer : IMemberComparer<IPropertyDefinition>
    {
    }
}