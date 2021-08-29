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

        protected override SemVerChangeType GetItemAddedChangeType(IMethodDefinition memberAdded)
        {
            if (memberAdded.DeclaringType is IInterfaceDefinition)
            {
                if (memberAdded.HasBody)
                {
                    // This is a default interface method that does not require classes that implement it to change
                    return SemVerChangeType.Feature;
                }

                return SemVerChangeType.Breaking;
            }

            if (memberAdded.Modifiers.HasFlag(MethodModifiers.Abstract))
            {
                return SemVerChangeType.Breaking;
            }

            return base.GetItemAddedChangeType(memberAdded);
        }
    }
}