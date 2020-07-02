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
            var attributeProcessor = new AttributeMatchProcessor(attributeComparer, evaluator, logger);
            var fieldComparer = new FieldComparer(attributeProcessor);
            var fieldProcessor = new FieldMatchProcessor(fieldComparer, evaluator, logger);
            var propertyComparer = new PropertyComparer(attributeProcessor);
            var propertyProcessor = new PropertyMatchProcessor(propertyComparer, evaluator, logger);
            var typeComparer = new TypeComparer(fieldProcessor, propertyProcessor, attributeProcessor);
            var processor = new TypeMatchProcessor(typeComparer, evaluator, logger);

            return new ChangeCalculator(processor, logger);
        }
    }
}