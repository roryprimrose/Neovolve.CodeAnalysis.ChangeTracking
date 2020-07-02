namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;

    public static class ChangeCalculatorFactory
    {
        public static IChangeCalculator BuildCalculator()
        {
            return BuildCalculator(null);
        }

        public static IChangeCalculator BuildCalculator(ILogger? logger)
        {
            var evaluator = new MatchEvaluator();
            var propertyComparer = new PropertyComparer();
            var propertyProcessor = new PropertyMatchProcessor(propertyComparer, evaluator, logger);
            var typeComparer = new TypeComparer(propertyProcessor);
            var processor = new TypeMatchProcessor(typeComparer, evaluator, logger);

            return new ChangeCalculator(processor, logger);
        }
    }
}