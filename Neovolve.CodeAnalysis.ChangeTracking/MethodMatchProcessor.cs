//namespace Neovolve.CodeAnalysis.ChangeTracking
//{
//    using System;
//    using System.Collections.Generic;
//    using Microsoft.Extensions.Logging;
//    using Neovolve.CodeAnalysis.ChangeTracking.Models;

//    public class MethodMatchProcessor : IMethodMatchProcessor
//    {
//        private readonly IMethodComparer _comparer;

//        public MethodMatchProcessor(IMethodComparer comparer, IMatchEvaluator<IMethodDefinition> evaluator, ILogger? logger) : base(
//            evaluator, logger)
//        {
//            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
//        }

//        public IEnumerable<ComparisonResult> CalculateChanges(IEnumerable<IMethodDefinition> oldItems, IEnumerable<IMethodDefinition> newItems, ComparerOptions options)
//        {
//            return TODO_IMPLEMENT_ME;
//        }
//    }
//}