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

            var attributeComparer = new AttributeComparer();
            var attributeMatcher = new AttributeMatchEvaluator();
            var attributeProcessor = new AttributeMatchProcessor(attributeComparer, attributeMatcher, logger);

            var fieldComparer = new FieldComparer(attributeProcessor);
            var fieldMatcher = new FieldMatchEvaluator();
            var fieldProcessor = new FieldMatchProcessor(fieldComparer, fieldMatcher, logger);

            var propertyAccessorComparer = new PropertyAccessorComparer(attributeProcessor);
            var propertyAccessorProcessor =
                new PropertyAccessorMatchProcessor(propertyAccessorComparer, evaluator, logger);
            var propertyComparer = new PropertyComparer(propertyAccessorProcessor, attributeProcessor);
            var propertyProcessor = new PropertyMatchProcessor(propertyComparer, evaluator, logger);

            var typeComparer = new TypeComparer(fieldProcessor, propertyProcessor, attributeProcessor);
            var typeMatchEvaluator = new TypeMatchEvaluator();
            var typeProcessor = new TypeMatchProcessor(typeComparer, typeMatchEvaluator, logger);

            return new ChangeCalculator(typeProcessor, logger);
        }
    }
}