namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class TypeComparer<T> : ElementComparer<T>, ITypeComparer<T> where T : ITypeDefinition
    {
        private readonly IGenericTypeElementComparer _genericTypeElementComparer;
        private readonly IFieldMatchProcessor _fieldProcessor;
        private readonly IPropertyMatchProcessor _propertyProcessor;
        private readonly IAccessModifiersComparer _accessModifiersComparer;
        private readonly IMethodMatchProcessor _methodProcessor;

        public TypeComparer(IAccessModifiersComparer accessModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IFieldMatchProcessor fieldProcessor,
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
            _fieldProcessor = fieldProcessor ?? throw new ArgumentNullException(nameof(fieldProcessor));
        }

        protected override void EvaluateMatch(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(CompareDefinitionType, match, options, aggregator, true);
            RunComparisonStep(CompareNamespace, match, options, aggregator, true);
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator);
            RunComparisonStep(EvaluateImplementedTypeChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateFieldChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyChanges, match, options, aggregator);
            RunComparisonStep(EvaluateMethodChanges, match, options, aggregator);
        }

        private static void CompareDefinitionType(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            // Check for a change in type
            if (match.OldItem.GetType() != match.NewItem.GetType())
            {
                var newType = DetermineTypeChangeDescription(match.NewItem);

                var args = new FormatArguments("{DefinitionType} {Identifier} has changed to {NewValue}",
                    match.OldItem.FullName, null, newType);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
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

        private static string DetermineTypeChangeDescription(T item)
        {
            if (item is IClassDefinition)
            {
                return "class";
            }

            if (item is IStructDefinition)
            {
                return "struct";
            }

            if (item is IInterfaceDefinition)
            {
                return "interface";
            }

            throw new NotSupportedException("Unknown type provided");
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

        private void EvaluateFieldChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldClass = match.OldItem as IClassDefinition;

            if (oldClass == null)
            {
                return;
            }

            var newClass = (IClassDefinition) match.NewItem;

            var changes = _fieldProcessor.CalculateChanges(oldClass.Fields, newClass.Fields, options);

            aggregator.AddResults(changes);
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