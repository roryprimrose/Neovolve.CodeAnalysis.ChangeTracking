namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodMatchProcessor : ElementMatchProcessor<IMethodDefinition>, IMethodMatchProcessor
    {
        public MethodMatchProcessor(
            IMethodEvaluator evaluator,
            IMethodComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}