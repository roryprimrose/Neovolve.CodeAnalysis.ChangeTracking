namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class TypeComparer<T> : ElementComparer<T>, ITypeComparer<T> where T : ITypeDefinition
    {
        private readonly IGenericTypeElementComparer _genericTypeElementComparer;
        private readonly IPropertyMatchProcessor _propertyProcessor;
        private readonly IAccessModifiersComparer _accessModifiersComparer;
        private readonly IMethodMatchProcessor _methodProcessor;

        public TypeComparer(IAccessModifiersComparer accessModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IPropertyMatchProcessor propertyProcessor,
            IMethodMatchProcessor methodProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _accessModifiersComparer = accessModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(accessModifiersComparer));
            _genericTypeElementComparer = genericTypeElementComparer
                                          ?? throw new ArgumentNullException(nameof(genericTypeElementComparer));
            _propertyProcessor = propertyProcessor ?? throw new ArgumentNullException(nameof(propertyProcessor));
            _methodProcessor = methodProcessor ?? throw new ArgumentNullException(nameof(methodProcessor));
        }

        protected override void EvaluateMatch(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));
            
            RunComparisonStep(CompareNamespace, match, options, aggregator, true);
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateImplementedTypeChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluatePropertyChanges, match, options, aggregator);
            RunComparisonStep(EvaluateMethodChanges, match, options, aggregator);
        }
        
        private static void CompareNamespace(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            // Check for a change in type
            if (match.OldItem.Namespace != match.NewItem.Namespace)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has changed namespace from {OldValue} to {NewValue}",
                    match.OldItem.FullName, match.OldItem.Namespace, match.NewItem.Namespace);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }
        
        private void EvaluateAccessModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IAccessModifiersElement<AccessModifiers>>(match.OldItem, match.NewItem);

            var results = _accessModifiersComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
        }
        
        private void EvaluateGenericTypeDefinitionChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IGenericTypeElement>(match.OldItem, match.NewItem);

            var results = _genericTypeElementComparer.CompareItems(convertedMatch, options);

            aggregator.AddResults(results);
        }

        private static void EvaluateImplementedTypeChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            // Find the old types that have been removed
            var removedTypes = match.OldItem.ImplementedTypes.Except(match.NewItem.ImplementedTypes);

            foreach (var removedType in removedTypes)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the implemented type {OldValue}",
                    match.NewItem.FullName, removedType, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }

            // Find the new types that have been added
            var addedTypes = match.NewItem.ImplementedTypes.Except(match.OldItem.ImplementedTypes);

            foreach (var addedType in addedTypes)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the implemented type {NewValue}",
                    match.NewItem.FullName, null, addedType);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }
        
        private void EvaluatePropertyChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldProperties = match.OldItem.Properties;
            var newProperties = match.NewItem.Properties;

            var results = _propertyProcessor.CalculateChanges(oldProperties, newProperties, options);

            aggregator.AddResults(results);
        }

        private void EvaluateMethodChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldMethods = match.OldItem.Methods;
            var newMethods = match.NewItem.Methods;

            var results = _methodProcessor.CalculateChanges(oldMethods, newMethods, options);

            aggregator.AddResults(results);
        }
    }
}