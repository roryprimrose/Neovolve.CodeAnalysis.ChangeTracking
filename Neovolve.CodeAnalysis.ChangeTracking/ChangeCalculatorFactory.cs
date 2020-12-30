﻿namespace Neovolve.CodeAnalysis.ChangeTracking
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
            var attributeComparer = new AttributeComparer();
            var attributeMatcher = new AttributeMatchEvaluator();
            var attributeProcessor = new AttributeMatchProcessor(attributeMatcher, attributeComparer, logger);

            var fieldComparer = new FieldComparer(attributeProcessor);
            var fieldMatcher = new FieldMatchEvaluator();
            var fieldProcessor = new FieldMatchProcessor(fieldMatcher, fieldComparer, logger);

            var propertyAccessorComparer = new PropertyAccessorComparer(attributeProcessor);
            var propertyAccessorMatcher = new PropertyAccessorMatchEvaluator();
            var propertyAccessorProcessor =
                new PropertyAccessorMatchProcessor(propertyAccessorMatcher, propertyAccessorComparer, logger);
            var propertyComparer = new PropertyComparer(propertyAccessorProcessor, attributeProcessor);
            var propertyMatcher = new PropertyMatchEvaluator();
            var propertyProcessor = new PropertyMatchProcessor(propertyMatcher, propertyComparer, logger);

            var typeComparer = new TypeComparer(fieldProcessor, propertyProcessor, attributeProcessor);
            var typeMatchEvaluator = new TypeMatchEvaluator();
            var typeProcessor = new TypeMatchProcessor(typeMatchEvaluator, typeComparer, logger);

            return new ChangeCalculator(typeProcessor, logger);
        }
    }
}