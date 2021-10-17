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
                    $"has {changeLabel} {shiftAmount} generic type parameter{suffix}",
                    null,
                    null);

                if (shiftAmount == 1)
                {
                    // Attempt to find the exact named generic type added or removed
                    var union = oldTypeParameters.Union(newTypeParameters);
                    var intersection = oldTypeParameters.Intersect(newTypeParameters);
                    var differences = union.Except(intersection).ToList();

                    if (differences.Count == 1)
                    {
                        var genericTypeNameChanged = differences[0];

                        // We have a change to just one named argument so we can make the change message more specific
                        if (typeParameterShift > 0)
                        {
                            // A generic type parameter has been removed
                            args = new FormatArguments($"has removed the {MessagePart.OldValue} generic type parameter",
                                genericTypeNameChanged, null);
                        }
                        else
                        {
                            // A generic type parameter has been added
                            args = new FormatArguments($"has added the {MessagePart.NewValue} generic type parameter",
                                null,
                                genericTypeNameChanged);
                        }
                    }
                }

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else
            {
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

            return aggregator.Results;
        }

        private static void EvaluateGenericConstraints(
            ItemMatch<IGenericTypeElement> match,
            IConstraintListDefinition? oldConstraintList,
            IConstraintListDefinition? newConstraintList,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldConstraints = oldConstraintList?.Constraints ?? new List<string>();
            var newConstraints = newConstraintList?.Constraints ?? new List<string>();

            if (oldConstraints.Count == 0
                && newConstraints.Count == 0)
            {
                // There are no generic constraints defined on either type
                return;
            }

            // Calculate the constraints added and removed
            var removedConstraints = oldConstraints.Except(newConstraints).ToList();
            var addedConstraints = newConstraints.Except(oldConstraints).ToList();

            var constraintShift = removedConstraints.Count + addedConstraints.Count;

            if (constraintShift == 0)
            {
                // The constraints match
                return;
            }

            if (addedConstraints.Count > 0)
            {
                var suffix = string.Empty;

                if (addedConstraints.Count != 1)
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"has added {addedConstraints.Count} generic type constraint{suffix}",
                    null,
                    null);

                if (addedConstraints.Count == 1)
                {
                    // A generic type parameter has been added
                    args = new FormatArguments($"has added the {MessagePart.NewValue} generic type constraint",
                        null,
                        addedConstraints[0]);
                }

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }

            if (removedConstraints.Count > 0)
            {
                var suffix = string.Empty;

                if (removedConstraints.Count != 1)
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"has removed {removedConstraints.Count} generic type constraint{suffix}",
                    null,
                    null);

                if (removedConstraints.Count == 1)
                {
                    // A generic type parameter has been added
                    args = new FormatArguments($"has removed the {MessagePart.OldValue} generic type constraint",
                        removedConstraints[0],
                        null);
                }

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }
        }
    }
}