﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyMatchProcessor : ElementMatchProcessor<IPropertyDefinition>, IPropertyMatchProcessor
    {
        public PropertyMatchProcessor(
            IPropertyEvaluator evaluator,
            IPropertyComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }

        protected override SemVerChangeType GetItemAddedChangeType(IPropertyDefinition memberAdded)
        {
            if (memberAdded.DeclaringType is IInterfaceDefinition)
            {
                return SemVerChangeType.Breaking;
            }

            if (memberAdded.Modifiers.HasFlag(PropertyModifiers.Abstract))
            {
                return SemVerChangeType.Breaking;
            }

            return base.GetItemAddedChangeType(memberAdded);
        }
    }
}