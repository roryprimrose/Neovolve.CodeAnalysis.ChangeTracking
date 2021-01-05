﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
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
            var attributeComparer = new AttributeComparer();
            var attributeMatcher = new AttributeMatchEvaluator();
            var attributeProcessor = new AttributeMatchProcessor(attributeMatcher, attributeComparer, logger);

            var accessModifierChangeTable = new AccessModifiersChangeTable();
            var accessModifiersComparer = new AccessModifiersComparer(accessModifierChangeTable);

            var fieldComparer = new FieldComparer(accessModifiersComparer, attributeProcessor);
            var fieldMatcher = new FieldMatchEvaluator();
            var fieldProcessor = new FieldMatchProcessor(fieldMatcher, fieldComparer, logger);

            var propertyAccessorAccessModifiersChangeTable = new PropertyAccessorAccessModifiersChangeTable();
            var propertyAccessorAccessModifiersComparer = new PropertyAccessorAccessModifiersComparer(propertyAccessorAccessModifiersChangeTable);
            var propertyAccessorComparer = new PropertyAccessorComparer(propertyAccessorAccessModifiersComparer, attributeProcessor);
            var propertyAccessorMatcher = new PropertyAccessorMatchEvaluator();
            var propertyAccessorProcessor =
                new PropertyAccessorMatchProcessor(propertyAccessorMatcher, propertyAccessorComparer, logger);
            var propertyComparer = new PropertyComparer(accessModifiersComparer, propertyAccessorProcessor, attributeProcessor);
            var propertyMatcher = new PropertyMatchEvaluator();
            var propertyProcessor = new PropertyMatchProcessor(propertyMatcher, propertyComparer, logger);

            var classModifiersChangeTable = new ClassModifiersChangeTable();
            var classModifiersComparer = new ClassModifiersComparer(classModifiersChangeTable);
            var classComparer = new ClassComparer(
                accessModifiersComparer,
                classModifiersComparer,
                fieldProcessor,
                propertyProcessor,
                attributeProcessor);

            var interfaceComparer = new InterfaceComparer(
                accessModifiersComparer,
                fieldProcessor,
                propertyProcessor,
                attributeProcessor);

            var structModifiersChangeTable = new StructModifiersChangeTable();
            var structModifiersComparer = new StructModifiersComparer(structModifiersChangeTable);
            var structComparer = new StructComparer(
                accessModifiersComparer,
                structModifiersComparer,
                fieldProcessor,
                propertyProcessor,
                attributeProcessor);

            var aggregateTypeComparer = new AggregateTypeComparer(classComparer, interfaceComparer, structComparer);

            var typeMatchEvaluator = new TypeMatchEvaluator();
            var typeProcessor = new TypeMatchProcessor(typeMatchEvaluator, aggregateTypeComparer, logger);

            return new ChangeCalculator(typeProcessor, logger);
        }
    }
}