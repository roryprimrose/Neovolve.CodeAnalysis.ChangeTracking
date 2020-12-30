namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;
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