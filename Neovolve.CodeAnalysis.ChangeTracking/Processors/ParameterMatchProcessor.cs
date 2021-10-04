namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ParameterMatchProcessor : ElementMatchProcessor<IParameterDefinition>, IParameterMatchProcessor
    {
        public ParameterMatchProcessor(
            IParameterEvaluator evaluator,
            IParameterComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }

        protected override SemVerChangeType GetItemAddedChangeType(IParameterDefinition memberAdded)
        {
            return SemVerChangeType.Breaking;
        }
    }
}