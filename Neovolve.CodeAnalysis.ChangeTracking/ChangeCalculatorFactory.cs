namespace Neovolve.CodeAnalysis.ChangeTracking
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
            var attributeMatcher = new AttributeEvaluator();
            var attributeProcessor = new AttributeMatchProcessor(attributeMatcher, attributeComparer, logger);

            var accessModifierChangeTable = new AccessModifiersChangeTable();
            var accessModifiersComparer = new AccessModifiersComparer(accessModifierChangeTable);
            var memberModifiersChangeTable = new PropertyModifiersChangeTable();
            var memberModifiersComparer = new PropertyModifiersComparer(memberModifiersChangeTable);

            var genericTypeElementComparer = new GenericTypeElementComparer();

            var fieldModifiersChangeTable = new FieldModifiersChangeTable();
            var fieldModifiersComparer = new FieldModifiersComparer(fieldModifiersChangeTable);
            var fieldComparer = new FieldComparer(accessModifiersComparer, fieldModifiersComparer, attributeProcessor);
            var fieldMatcher = new FieldEvaluator();
            var fieldProcessor = new FieldMatchProcessor(fieldMatcher, fieldComparer, logger);

            var propertyAccessorAccessModifiersChangeTable = new PropertyAccessorAccessModifiersChangeTable();
            var propertyAccessorAccessModifiersComparer = new PropertyAccessorAccessModifiersComparer(propertyAccessorAccessModifiersChangeTable);
            var propertyAccessorComparer = new PropertyAccessorComparer(propertyAccessorAccessModifiersComparer, attributeProcessor);
            var propertyAccessorEvaluator = new PropertyAccessorEvaluator();
            var propertyAccessorProcessor =
                new PropertyAccessorMatchProcessor(propertyAccessorEvaluator, propertyAccessorComparer, logger);
            var propertyComparer = new PropertyComparer(accessModifiersComparer, memberModifiersComparer, propertyAccessorProcessor, attributeProcessor);
            var propertyMatcher = new PropertyEvaluator();
            var propertyProcessor = new PropertyMatchProcessor(propertyMatcher, propertyComparer, logger);

            var methodEvaluator = new MethodEvaluator();
            var methodModifiersChangeTable = new MethodModifiersChangeTable();
            var methodModifiersComparer = new MethodModifiersComparer(methodModifiersChangeTable);
            var parameterModifiersChangeTable = new ParameterModifiersChangeTable();
            var parameterModifiersComparer = new ParameterModifiersComparer(parameterModifiersChangeTable);
            var parameterComparer = new ParameterComparer(parameterModifiersComparer, attributeProcessor);
            var methodComparer = new MethodComparer(accessModifiersComparer, methodModifiersComparer, genericTypeElementComparer, parameterComparer, attributeProcessor);
            var methodProcessor = new MethodMatchProcessor(methodEvaluator, methodComparer, logger);

            var classModifiersChangeTable = new ClassModifiersChangeTable();
            var classModifiersComparer = new ClassModifiersComparer(classModifiersChangeTable);
            var classComparer = new ClassComparer(
                accessModifiersComparer,
                classModifiersComparer,
                genericTypeElementComparer,
                fieldProcessor,
                propertyProcessor,
                methodProcessor, attributeProcessor);

            var interfaceComparer = new InterfaceComparer(
                accessModifiersComparer,
                genericTypeElementComparer,
                propertyProcessor,
                methodProcessor,
                attributeProcessor);

            var structModifiersChangeTable = new StructModifiersChangeTable();
            var structModifiersComparer = new StructModifiersComparer(structModifiersChangeTable);
            var structComparer = new StructComparer(
                accessModifiersComparer,
                structModifiersComparer,
                genericTypeElementComparer,
                fieldProcessor,
                propertyProcessor,
                methodProcessor,
                attributeProcessor);

            var aggregateTypeComparer = new AggregateTypeComparer(classComparer, interfaceComparer, structComparer);

            var typeEvaluator = new TypeEvaluator();
            var typeProcessor = new TypeMatchProcessor(typeEvaluator, aggregateTypeComparer, logger);

            return new ChangeCalculator(typeProcessor, logger);
        }
    }
}