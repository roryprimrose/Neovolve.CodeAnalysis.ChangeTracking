namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorMatchProcessor : ElementMatchProcessor<IPropertyAccessorDefinition>,
        IPropertyAccessorMatchProcessor
    {
        public PropertyAccessorMatchProcessor(
            IPropertyAccessorEvaluator evaluator,
            IPropertyAccessorComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}