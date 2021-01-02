namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

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