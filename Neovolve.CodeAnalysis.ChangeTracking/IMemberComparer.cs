﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMemberComparer<T> : IElementComparer<T> where T : IMemberDefinition
    {
    }
}