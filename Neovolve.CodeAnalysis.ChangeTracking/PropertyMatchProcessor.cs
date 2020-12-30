namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyMatchProcessor : ElementMatchProcessor<IPropertyDefinition>, IPropertyMatchProcessor
    {
        public PropertyMatchProcessor(
            IMatchEvaluator<IPropertyDefinition> evaluator,
            IPropertyComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}