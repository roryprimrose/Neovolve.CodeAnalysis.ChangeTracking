namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public interface IMatchAgent<T> where T : IItemDefinition
    {
        void MatchOn(Func<T, T, bool> condition);
        IReadOnlyCollection<T> NewItems { get; }
        IReadOnlyCollection<T> OldItems { get; }
        IMatchResults<T> Results { get; }
    }
}