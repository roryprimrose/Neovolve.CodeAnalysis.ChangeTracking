namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class TypeComparer<T> : ElementComparer<T>, ITypeComparer<T> where T : ITypeDefinition
    {
        private readonly IFieldMatchProcessor _fieldProcessor;
        private readonly IPropertyMatchProcessor _propertyProcessor;
        private readonly IAccessModifiersComparer _accessModifiersComparer;
        private readonly IMethodMatchProcessor _methodProcessor;

        public TypeComparer(IAccessModifiersComparer accessModifiersComparer,
            IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IMethodMatchProcessor methodProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _accessModifiersComparer = accessModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(accessModifiersComparer));
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
        
        private static void EvaluateGenericConstraints(ItemMatch<T> match,
            IConstraintListDefinition? oldConstraintList,
            IConstraintListDefinition? newConstraintList,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldConstraintCount = oldConstraintList?.Constraints.Count ?? 0;
            var newConstraintCount = newConstraintList?.Constraints.Count ?? 0;

            if (oldConstraintCount == 0
                && newConstraintCount == 0)
            {
                // There are no generic constraints defined on either type
                return;
            }

            if (oldConstraintCount == 0)
            {
                var suffix = string.Empty;

                if (newConstraintCount != 1)
                {
                    // There is more than one modifiers
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has added {newConstraintCount} generic type constraint{suffix}",
                    match.NewItem.FullName, null, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into the constraints themselves
                return;
            }

            if (newConstraintCount == 0)
            {
                var suffix = string.Empty;

                if (oldConstraintCount != 1)
                {
                    // There is more than one modifiers
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has removed {oldConstraintCount} generic type constraint{suffix}",
                    match.NewItem.FullName, null, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into the constraints themselves
                return;
            }

            // Find the old constraints that have been removed
            var removedConstraints = oldConstraintList!.Constraints.Except(newConstraintList!.Constraints);

            foreach (var constraint in removedConstraints)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} generic type constraint",
                    match.NewItem.FullName, constraint, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }

            // Find the new constraints that have been added
            var addedConstraints = newConstraintList!.Constraints.Except(oldConstraintList!.Constraints);

            foreach (var constraint in addedConstraints)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} generic type constraint",
                    match.NewItem.FullName, null, constraint);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateGenericTypeDefinitionChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldTypeParameters = match.OldItem.GenericTypeParameters.FastToList();
            var newTypeParameters = match.NewItem.GenericTypeParameters.FastToList();

            var typeParameterShift = oldTypeParameters.Count - newTypeParameters.Count;

            if (typeParameterShift > 0)
            {
                // One or more generic type parameters have been removed
                var suffix = typeParameterShift == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has removed {typeParameterShift} generic type parameter{suffix}",
                    match.NewItem.FullName, null, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            if (typeParameterShift < 0)
            {
                // One or more generic type parameters have been added
                var shift = Math.Abs(typeParameterShift);
                var suffix = shift == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has added {shift} generic type parameter{suffix}",
                    match.NewItem.FullName, null, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            // We have the same number of generic types, evaluate the constraints
            for (var index = 0; index < oldTypeParameters.Count; index++)
            {
                var oldName = oldTypeParameters[index];
                var newName = newTypeParameters[index];

                var oldConstraints = match.OldItem.GenericConstraints.FirstOrDefault(x => x.Name == oldName);
                var newConstraints = match.NewItem.GenericConstraints.FirstOrDefault(x => x.Name == newName);

                EvaluateGenericConstraints(match, oldConstraints, newConstraints, options, aggregator);
            }
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