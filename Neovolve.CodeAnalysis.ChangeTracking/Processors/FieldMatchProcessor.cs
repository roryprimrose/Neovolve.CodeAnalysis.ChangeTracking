namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldMatchProcessor : ElementMatchProcessor<IFieldDefinition>, IFieldMatchProcessor
    {
        public FieldMatchProcessor(
            IMatchEvaluator<IFieldDefinition> evaluator,
            IFieldComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}