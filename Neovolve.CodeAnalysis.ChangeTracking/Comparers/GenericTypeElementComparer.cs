namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class GenericTypeElementComparer : IGenericTypeElementComparer
    {
        public IEnumerable<ComparisonResult> CompareMatch(ItemMatch<IGenericTypeElement> match, ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            var oldTypeParameters = match.OldItem.GenericTypeParameters.FastToList();
            var newTypeParameters = match.NewItem.GenericTypeParameters.FastToList();

            if (oldTypeParameters.Count == 0
                && newTypeParameters.Count == 0)
            {
                return Array.Empty<ComparisonResult>();
            }

            var typeParameterShift = oldTypeParameters.Count - newTypeParameters.Count;

            var aggregator = new ChangeResultAggregator();

            if (typeParameterShift != 0)
            {
                var changeLabel = typeParameterShift > 0 ? "removed" : "added";
                var shiftAmount = Math.Abs(typeParameterShift);

                // One or more generic type parameters have been removed
                var suffix = shiftAmount == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has {changeLabel} {shiftAmount} generic type parameter{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return aggregator.Results;
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

            return aggregator.Results;
        }

        private static void EvaluateGenericConstraints(
            ItemMatch<IGenericTypeElement> match,
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
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has added {newConstraintCount} generic type constraint{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

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
                    $"{{DefinitionType}} {{Identifier}} has removed {oldConstraintCount} generic type constraint{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);

                // No need to look into the constraints themselves
                return;
            }

            // Find the old constraints that have been removed
            var removedConstraints = oldConstraintList!.Constraints.Except(newConstraintList!.Constraints);

            foreach (var constraint in removedConstraints)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} generic type constraint",
                    match.NewItem.FullName,
                    constraint,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }

            // Find the new constraints that have been added
            var addedConstraints = newConstraintList!.Constraints.Except(oldConstraintList!.Constraints);

            foreach (var constraint in addedConstraints)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} generic type constraint",
                    match.NewItem.FullName,
                    null,
                    constraint);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }
    }
}