namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ConstructorMatchProcessor : ElementMatchProcessor<IConstructorDefinition>, IConstructorMatchProcessor
    {
        public ConstructorMatchProcessor(
            IConstructorEvaluator evaluator,
            IConstructorComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}