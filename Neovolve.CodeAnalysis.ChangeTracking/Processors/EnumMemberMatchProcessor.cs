﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Processors
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class EnumMemberMatchProcessor : ElementMatchProcessor<IEnumMemberDefinition>, IEnumMemberMatchProcessor
    {
        public EnumMemberMatchProcessor(
            IEnumMemberEvaluator evaluator,
            IEnumMemberComparer comparer,
            ILogger? logger) : base(evaluator, comparer, logger)
        {
        }
    }
}