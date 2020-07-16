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
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(CompareDefinitionType, match, options, aggregator, true);
            RunComparisonStep(EvaluateAccessModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateClassModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator);
            RunComparisonStep(EvaluateImplementedTypeChanges, match, options, aggregator, true);
            RunComparisonStep(EvaluateFieldChanges, match, options, aggregator);
            RunComparisonStep(EvaluatePropertyChanges, match, options, aggregator);
        }

        private static void CompareDefinitionType(
            ItemMatch<ITypeDefinition> match,
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

        private static string DetermineTypeChangeDescription(ITypeDefinition item)
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
            IChangeResultAggregator aggregator)
        {
            var change = AccessModifierChangeTable.CalculateChange(match);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            var newModifiers = match.NewItem.GetDeclaredAccessModifiers();
            var oldModifiers = match.OldItem.GetDeclaredAccessModifiers();

            if (string.IsNullOrWhiteSpace(oldModifiers))
            {
                // Modifiers have been added where there were previously none defined
                var suffix = string.Empty;

                if (newModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} access modifier" + suffix,
                    match.NewItem.FullName, null, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(newModifiers))
            {
                // All previous modifiers have been removed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} access modifier" + suffix,
                    match.NewItem.FullName, oldModifiers, null);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has changed the access modifier{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName, oldModifiers, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateClassModifierChanges(
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
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

            var newModifiers = match.NewItem.GetDeclaredModifiers();
            var oldModifiers = match.OldItem.GetDeclaredModifiers();

            if (string.IsNullOrWhiteSpace(oldModifiers))
            {
                // Modifiers have been added where there were previously none defined
                var suffix = string.Empty;

                if (newModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} modifier" + suffix,
                    match.NewItem.FullName, null, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(newModifiers))
            {
                // All previous modifiers have been removed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} modifier" + suffix,
                    match.NewItem.FullName, oldModifiers, null);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has changed the modifier{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName, oldModifiers, newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateGenericConstraints(ItemMatch<ITypeDefinition> match,
            IConstraintListDefinition oldConstraintList,
            IConstraintListDefinition newConstraintList,
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
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added {NewValue} generic type constraint" + suffix,
                    match.NewItem.FullName, null, newConstraintCount.ToString());

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into the constraints themselves
                return;
            }

            if (newConstraintCount == 0)
            {
                var suffix = string.Empty;

                if (oldConstraintCount != 1)
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed {OldValue} generic type constraint" + suffix,
                    match.NewItem.FullName, oldConstraintCount.ToString(), null);

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
            ItemMatch<ITypeDefinition> match,
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
                    "{DefinitionType} {Identifier} has removed {OldValue} generic type parameter" + suffix,
                    match.NewItem.FullName, typeParameterShift.ToString(), null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            if (typeParameterShift < 0)
            {
                // One or more generic type parameters have been removed
                var shift = Math.Abs(typeParameterShift);
                var suffix = shift == 1 ? "" : "s";
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added {NewValue} generic type parameter" + suffix,
                    match.NewItem.FullName, null, typeParameterShift.ToString());

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
            ItemMatch<ITypeDefinition> match,
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
            ItemMatch<ITypeDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
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
            IChangeResultAggregator aggregator)
        {
            var oldProperties = match.OldItem.Properties;
            var newProperties = match.NewItem.Properties;

            var results = _propertyProcessor.CalculateChanges(oldProperties, newProperties, options);

            aggregator.AddResults(results);
        }
    }
}