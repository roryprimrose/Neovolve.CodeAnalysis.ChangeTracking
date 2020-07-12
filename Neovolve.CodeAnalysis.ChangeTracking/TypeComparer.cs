namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeComparer : ElementComparer<ITypeDefinition>, ITypeComparer
    {
        private readonly IFieldMatchProcessor _fieldProcessor;
        private readonly IPropertyMatchProcessor _propertyProcessor;

        public TypeComparer(
            IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _propertyProcessor = propertyProcessor ?? throw new ArgumentNullException(nameof(propertyProcessor));
            _fieldProcessor = fieldProcessor ?? throw new ArgumentNullException(nameof(fieldProcessor));
        }

        protected override void EvaluateMatch(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(CompareDefinitionType, match, options, aggregator, true);
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateModifierChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateImplementedTypeChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateFieldChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyChanges, match, options, aggregator);
        }

        private static void CompareDefinitionType(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            // Check for a change in type
            if (match.OldItem.GetType() != match.NewItem.GetType())
            {
                var newType = DetermineTypeName(match.NewItem);

                var result = ComparisonResult.ItemChanged(
                    SemVerChangeType.Breaking,
                    match,
                    $"{match.OldItem.Description} has changed to {newType}");

                aggregator.AddResult(result);
            }
        }

        private static string DetermineTypeName(ITypeDefinition item)
        {
            if (item is IClassDefinition)
            {
                return "class";
            }

            if (item is IInterfaceDefinition)
            {
                return "interface";
            }

            throw new NotSupportedException("Unknown type provided");
        }

        private static void EvaluateAccessModifierChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var change = AccessModifierChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (match.OldItem.AccessModifier == AccessModifier.None)
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the access modifiers {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else if (match.NewItem.AccessModifier == AccessModifier.None)
            {
                // All previous modifiers have been removed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the access modifiers {match.OldItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed access modifiers from {match.OldItem.AccessModifier} to {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
        }

        private static void EvaluateGenericConstraints(
            ItemMatch<ITypeDefinition> match,
            IConstraintListDefinition oldConstraintList,
            IConstraintListDefinition newConstraintList,
            ChangeResultAggregator aggregator)
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
                var message = $"{match.NewItem.Description} has added {newConstraintCount} generic type constraint";

                if (newConstraintCount != 1)
                {
                    message += "s";
                }

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);

                // No need to look into the constraints themselves
                return;
            }

            if (newConstraintCount == 0)
            {
                var message = $"{match.NewItem.Description} has removed {oldConstraintCount} generic type constraint";

                if (oldConstraintCount != 1)
                {
                    message += "s";
                }

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);

                // No need to look into the constraints themselves
                return;
            }

            // Find the old constraints that have been removed
            var removedConstraints = oldConstraintList!.Constraints.Except(newConstraintList!.Constraints);

            foreach (var constraint in removedConstraints)
            {
                var message = $"{match.NewItem.Description} has removed the generic type constraint {constraint}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);

                aggregator.AddResult(result);
            }

            // Find the new constraints that have been added
            var addedConstraints = newConstraintList!.Constraints.Except(oldConstraintList!.Constraints);

            foreach (var constraint in addedConstraints)
            {
                var message = $"{match.NewItem.Description} has added the generic type constraint {constraint}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);
            }
        }

        private static void EvaluateGenericTypeDefinitionChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var oldTypeParameters = match.OldItem.GenericTypeParameters.ToList();
            var newTypeParameters = match.NewItem.GenericTypeParameters.ToList();

            var typeParameterShift = oldTypeParameters.Count - newTypeParameters.Count;

            if (typeParameterShift > 0)
            {
                // One or more generic type parameters have been removed
                var suffix = typeParameterShift == 1 ? "" : "s";
                var message =
                    $"{match.NewItem.Description} has removed {typeParameterShift} generic type parameter{suffix}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);

                // No need to look into how the generic type has changed
                return;
            }

            if (typeParameterShift < 0)
            {
                // One or more generic type parameters have been removed
                var shift = Math.Abs(typeParameterShift);
                var suffix = shift == 1 ? "" : "s";
                var message = $"{match.NewItem.Description} has added {shift} generic type parameter{suffix}";
                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);

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

                EvaluateGenericConstraints(match, oldConstraints, newConstraints, aggregator);
            }
        }

        private static void EvaluateImplementedTypeChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            // Find the old types that have been removed
            var removedTypes = match.OldItem.ImplementedTypes.Except(match.NewItem.ImplementedTypes);

            foreach (var removedType in removedTypes)
            {
                var message = $"{match.NewItem.Description} has removed the implemented type {removedType}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);
            }

            // Find the new types that have been added
            var addedTypes = match.NewItem.ImplementedTypes.Except(match.OldItem.ImplementedTypes);

            foreach (var addedType in addedTypes)
            {
                var message = $"{match.NewItem.Description} has added the implemented type {addedType}";

                var result = ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                aggregator.AddResult(result);
            }
        }

        private static void EvaluateModifierChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var oldItem = match.OldItem as IClassDefinition;

            if (oldItem == null)
            {
                // This is not a class
                return;
            }

            var newItem = (IClassDefinition)match.NewItem;
            var classMatch = new ItemMatch<IClassDefinition>(oldItem, newItem);

            var change = ClassModifierChangeTable.CalculateChange(classMatch);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            if (match.OldItem.AccessModifier == AccessModifier.None)
            {
                // Modifiers have been added where there were previously none defined
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has added the modifiers {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else if (match.NewItem.AccessModifier == AccessModifier.None)
            {
                // All previous modifiers have been removed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has removed the modifiers {match.OldItem.AccessModifier}");

                aggregator.AddResult(result);
            }
            else
            {
                // Modifiers have been changed
                var result = ComparisonResult.ItemChanged(
                    change,
                    match,
                    $"{match.NewItem.Description} has changed modifiers from {match.OldItem.AccessModifier} to {match.NewItem.AccessModifier}");

                aggregator.AddResult(result);
            }
        }
        private void EvaluateFieldChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var oldClass = match.OldItem as IClassDefinition;

            if (oldClass == null)
            {
                return;
            }

            var newClass = (IClassDefinition)match.NewItem;

            var changes = _fieldProcessor.CalculateChanges(oldClass.Fields, newClass.Fields, options);

            aggregator.AddResults(changes);
        }

        private void EvaluatePropertyChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            ChangeResultAggregator aggregator)
        {
            var oldProperties = match.OldItem.Properties;
            var newProperties = match.NewItem.Properties;

            var results = _propertyProcessor.CalculateChanges(oldProperties, newProperties, options);

            aggregator.AddResults(results);
        }
    }
}