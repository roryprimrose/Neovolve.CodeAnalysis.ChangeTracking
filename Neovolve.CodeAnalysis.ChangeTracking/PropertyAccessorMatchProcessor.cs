namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorMatchProcessor : ElementMatchProcessor<IPropertyAccessorDefinition>,
        IPropertyAccessorMatchProcessor
    {
        public PropertyAccessorMatchProcessor(
            IMatchEvaluator<IPropertyAccessorDefinition> evaluator,
            IPropertyAccessorComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}