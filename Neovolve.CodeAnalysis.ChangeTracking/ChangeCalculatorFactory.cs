namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    public static class ChangeCalculatorFactory
    {
        public static IChangeCalculator BuildCalculator()
        {
            return BuildCalculator(null);
        }

        public static IChangeCalculator BuildCalculator(ILogger? logger)
        {
            var matchers = new List<IMemberMatcher>
            {
                new MemberMatcher()
            };
            var evaluator = new MatchEvaluator(matchers, logger);
            var comparers = new List<MemberComparer>
            {
                new MemberComparer(),
                new PropertyComparer()
            };

            return new ChangeCalculator(evaluator, comparers, logger);
        }
    }
}