namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AttributeComparer : IAttributeComparer
    {
        public IEnumerable<ComparisonResult> CompareMatch(ItemMatch<IAttributeDefinition> match,
            ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            if (match.OldItem.Arguments.Count == 0
                && match.NewItem.Arguments.Count == 0)
            {
                // The attributes do not have any arguments
                // These attributes match each other
                return Array.Empty<ComparisonResult>();
            }

            var aggregator = new ChangeResultAggregator();

            EvaluateArgumentDefinitionChanges(match, options, aggregator);

            return aggregator.Results;
        }

        private static bool EvaluateArgumentCount(ItemMatch<IAttributeDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldArguments = match.OldItem.Arguments;
            var oldArgumentCount = oldArguments.Count;
            var newArguments = match.NewItem.Arguments;
            var newArgumentCount = newArguments.Count;

            var argumentShift = oldArgumentCount - newArgumentCount;

            if (argumentShift == 0)
            {
                return false;
            }

            // One or more arguments have been added or removed
            var changeLabel = argumentShift > 0 ? "removed" : "added";
            var shiftAmount = Math.Abs(argumentShift);
            var suffix = shiftAmount == 1 ? "" : "s";
            var args = new FormatArguments(
                $"has {changeLabel} {shiftAmount} argument{suffix}", null, null);

            var oldNamedArguments = oldArguments.Where(x => x.ArgumentType == ArgumentType.Named).ToList();
            var newNamedArguments = newArguments.Where(x => x.ArgumentType == ArgumentType.Named).ToList();

            if (shiftAmount == 1
                && oldNamedArguments.Count != newNamedArguments.Count)
            {
                // Attempt to be more specific about the argument change
                // In this case there is a named argument that has been 
                var oldNames = oldNamedArguments.Select(x => x.ParameterName).ToList();
                var newNames = newNamedArguments.Select(x => x.ParameterName).ToList();

                var union = oldNames.Union(newNames);
                var intersection = oldNames.Intersect(newNames);
                var differences = union.Except(intersection).ToList();

                if (differences.Count == 1)
                {
                    var parameterName = differences[0];

                    // We have a change to just one named argument so we can make the change message more specific
                    if (argumentShift > 0)
                    {
                        // Old named argument has been removed
                        // Find the old named argument
                        var oldArgument = oldNamedArguments.First(x => x.ParameterName == parameterName);

                        aggregator.AddElementRemovedResult(SemVerChangeType.Breaking, oldArgument,
                            options.MessageFormatter);

                        return true;
                    }

                    // New named argument has been added
                    // Find the new named argument
                    var newArgument = newNamedArguments.First(x => x.ParameterName == parameterName);

                    aggregator.AddElementAddedResult(SemVerChangeType.Breaking, newArgument,
                        options.MessageFormatter);

                    return true;
                }
            }

            aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

            // No need to look into how the attribute has changed
            return true;
        }

        private static void EvaluateArgumentDefinitionChanges(
            ItemMatch<IAttributeDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            if (EvaluateArgumentCount(match, options, aggregator))
            {
                return;
            }

            if (EvaluateOrdinalArgumentCount(match, options, aggregator))
            {
                return;
            }

            // No need to check for a difference in named argument counts because at this point we have the same number of overall arguments and the same number of ordinal arguments
            // The number of named arguments must be the same

            // At this point we have the same number of ordinal and named arguments
            if (EvaluateOrdinalArgument(match, options, aggregator))
            {
                return;
            }

            EvaluateNamedArgument(match, options, aggregator);
        }

        private static void EvaluateNamedArgument(ItemMatch<IAttributeDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldArguments = match.OldItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Named).ToList();
            var newArguments = match.NewItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Named).ToList();

            // At this point we have the same number of named arguments
            // Assuming there is a change, it can either be
            // All parameter names match but a value has changed, or
            // There is a different in parameter names which can only mean at least one has changed (old removed, new added)
            // The later will just be recorded as the old named parameter has been removed
            for (var index = 0; index < oldArguments.Count; index++)
            {
                var oldArgument = oldArguments[index];
                var newArgument = newArguments.FirstOrDefault(x => x.ParameterName == oldArgument.ParameterName);

                if (newArgument == null)
                {
                    // This argument has been removed
                    var args = new FormatArguments(
                        "has been removed", null, null);

                    var message = options.MessageFormatter.FormatItem(oldArgument, ItemFormatType.ItemRemoved, args);

                    var result = new ComparisonResult(SemVerChangeType.Breaking, oldArgument, null, message);

                    aggregator.AddResult(result);

                    return;
                }

                if (oldArgument.Value != newArgument.Value)
                {
                    // There is a match on the parameter names but the value has changed
                    var args = new FormatArguments(
                        "has changed value from {OldValue} to {NewValue}", oldArgument.Value, newArgument.Value);

                    // Create a match for the arguments
                    var argumentMatch = new ItemMatch<IArgumentDefinition>(oldArgument, newArgument);

                    aggregator.AddElementChangedResult(SemVerChangeType.Breaking, argumentMatch,
                        options.MessageFormatter,
                        args);

                    return;
                }
            }
        }

        private static bool EvaluateOrdinalArgument(ItemMatch<IAttributeDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldArguments = match.OldItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Ordinal).ToList();
            var newArguments = match.NewItem.Arguments.Where(x => x.ArgumentType == ArgumentType.Ordinal).ToList();

            for (var index = 0; index < oldArguments.Count; index++)
            {
                var oldArgument = oldArguments[index];
                var newArgument = newArguments[index];

                if (oldArgument.Value != newArgument.Value)
                {
                    var args = new FormatArguments(
                        "has changed value from {OldValue} to {NewValue}", oldArgument.Value, newArgument.Value);

                    // Create a match for the arguments
                    var argumentMatch = new ItemMatch<IArgumentDefinition>(oldArgument, newArgument);

                    aggregator.AddElementChangedResult(SemVerChangeType.Breaking, argumentMatch,
                        options.MessageFormatter,
                        args);

                    return true;
                }
            }

            return false;
        }

        private static bool EvaluateOrdinalArgumentCount(ItemMatch<IAttributeDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldArguments = match.OldItem.Arguments.Count(x => x.ArgumentType == ArgumentType.Ordinal);
            var newArguments = match.NewItem.Arguments.Count(x => x.ArgumentType == ArgumentType.Ordinal);

            var argumentShift = oldArguments - newArguments;

            if (argumentShift != 0)
            {
                var changeLabel = argumentShift > 0 ? "removed" : "added";
                var shiftAmount = Math.Abs(argumentShift);

                // One or more arguments have been added or removed
                var suffix = shiftAmount == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"has {changeLabel} {shiftAmount} ordinal argument{suffix}", null, null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                return true;
            }

            return false;
        }
    }
}