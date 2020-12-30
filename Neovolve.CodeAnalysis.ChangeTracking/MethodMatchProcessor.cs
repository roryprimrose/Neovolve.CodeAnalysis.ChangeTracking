namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodMatchProcessor : ElementMatchProcessor<IMethodDefinition>, IMethodMatchProcessor
    {
        public MethodMatchProcessor(
            IMethodMatchEvaluator evaluator,
            IMethodComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}