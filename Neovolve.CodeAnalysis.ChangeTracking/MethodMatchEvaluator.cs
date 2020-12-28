namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MethodMatchEvaluator : MatchEvaluator<IMethodDefinition>
    {
        public override IMatchResults<IMethodDefinition> MatchItems(
            IEnumerable<IMethodDefinition> oldItems,
            IEnumerable<IMethodDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            var tracker = new MatchTracker(oldItems, newItems);

            // Find all the members that match by name, generic type parameter count and parameter set
            // This is finding direct matches on method signatures
            IdentifyMatches(tracker, (source, target) =>
                target.RawName == source.RawName
                && target.GenericTypeParameters.Count == source.GenericTypeParameters.Count
                && ParameterListMatches(source, target));

            // Find all members that match on name, generic type parameter count and parameter count
            // This is finding methods that have changed their parameter types
            IdentifyMatches(tracker, (source, target) =>
                target.RawName == source.RawName
                && target.GenericTypeParameters.Count == source.GenericTypeParameters.Count
                && target.Parameters.Count == source.Parameters.Count);

            // Find all members that match on name and parameter count but generic type parameter count has changed
            // This is finding methods that have changed their generic type parameters
            IdentifyMatches(tracker, (source, target) =>
                target.RawName == source.RawName
                && target.Parameters.Count == source.Parameters.Count);

            // Find members with the same name and generic type parameter count but different parameters
            // In this case it is a member that has changed the parameters
            IdentifyMatches(tracker, (source, target) =>
                target.RawName == source.RawName
                && target.GenericTypeParameters.Count == source.GenericTypeParameters.Count);

            // Find members with the same name where there is only one remaining match by name
            // In this case it is a member that has changed the parameters
            IdentifyMatches(tracker, (source, target) =>
                target.RawName == source.RawName);

            // Find members that have been renamed where there is only one where the parameter set is the same but with a different name
            IdentifyMatches(tracker, (source, target) =>
                target.GenericTypeParameters.Count == source.GenericTypeParameters.Count
                && ParameterListMatches(source, target));

            // At this point we have matched as many related methods between the old code and the new code
            return tracker.Results;
        }

        private static void IdentifyMatches(MatchTracker tracker,
            Func<IMethodDefinition, IMethodDefinition, bool> matcher)
        {
            // Loop in reverse so that the items in the loop can be removed safely by the tracker
            for (var index = tracker.OldItems.Count - 1; index >= 0; index--)
            {
                var oldMethod = tracker.OldItems[index];

                // Get the old methods that match the predicate
                var matchingOldMethods = tracker.OldItems.Count(x => matcher(oldMethod, x));

                if (matchingOldMethods > 1)
                {
                    // There is more than one old method matching the predicate
                    // We can't match old methods to new methods in this case because there are multiple that could match
                    continue;
                }

                // Get the new methods that also match the predicate
                var matchingMethods = tracker.NewItems.Where(x => matcher(oldMethod, x)).ToList();

                if (matchingMethods.Count != 1)
                {
                    // There are either no new methods matching the predicate or there are more than one
                    // In either case we can't match the old method to a new method because there are multiple that could match
                    continue;
                }

                var matchingMethod = matchingMethods[0];

                // There is only one old and new method that match the predicate
                // The assumption here is that these two methods are a match
                tracker.MatchFound(oldMethod, matchingMethod);
            }
        }

        private static bool ParameterListMatches(IMethodDefinition oldMethod, IMethodDefinition newMethod)
        {
            if (oldMethod.Parameters.Count != newMethod.Parameters.Count)
            {
                return false;
            }

            var oldParameters = oldMethod.Parameters.ToList();
            var newParameters = newMethod.Parameters.ToList();

            for (var index = 0; index < oldParameters.Count; index++)
            {
                var oldParameter = oldParameters[index];
                var newParameter = newParameters[index];

                if (oldParameter.Type != newParameter.Type)
                {
                    return false;
                }
            }

            return true;
        }

        private class MatchTracker
        {
            private readonly List<ItemMatch<IMethodDefinition>> _matches;
            private readonly List<IMethodDefinition> _newItems;
            private readonly List<IMethodDefinition> _oldItems;

            public MatchTracker(IEnumerable<IMethodDefinition> oldItems, IEnumerable<IMethodDefinition> newItems)
            {
                _oldItems = oldItems.ToList();
                _newItems = newItems.ToList();
                _matches = new List<ItemMatch<IMethodDefinition>>();
            }

            public void MatchFound(IMethodDefinition oldItem, IMethodDefinition newItem)
            {
                var match = new ItemMatch<IMethodDefinition>(oldItem, newItem);

                _matches.Add(match);
                _oldItems.Remove(oldItem);
                _newItems.Remove(newItem);
            }

            public IReadOnlyList<IMethodDefinition> NewItems => _newItems;

            public IReadOnlyList<IMethodDefinition> OldItems => _oldItems;

            public IMatchResults<IMethodDefinition> Results =>
                new MatchResults<IMethodDefinition>(_matches, _oldItems, _newItems);
        }
    }
}