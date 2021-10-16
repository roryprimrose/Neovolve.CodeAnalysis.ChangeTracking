namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="IMatchAgent{T}" />
    ///     interface defines the members that process item match logic and collection the results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatchAgent<T> where T : IItemDefinition
    {
        void MatchOn(Func<T, T, bool> condition);
        IEvaluationResults<T> Results { get; }
    }
}