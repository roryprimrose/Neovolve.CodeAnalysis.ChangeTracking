namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeMatchEvaluator : MatchEvaluator<ITypeDefinition>
    {
        public override IMatchResults<ITypeDefinition> MatchItems(
            IEnumerable<ITypeDefinition> oldItems,
            IEnumerable<ITypeDefinition> newItems)
        {
            oldItems = oldItems ?? throw new ArgumentNullException(nameof(oldItems));
            newItems = newItems ?? throw new ArgumentNullException(nameof(newItems));

            IMatchResults<ITypeDefinition>? results = new MatchResults<ITypeDefinition>(oldItems, newItems);

            results = FindMatches(results, (x, y) => x.IsMatch(y));

            var currentItemsRemoved = results.ItemsRemoved;
            var currentItemsAdded = results.ItemsAdded;

            results = FindMatches(results, (x, y) => IsMovedType(x, y, currentItemsRemoved, currentItemsAdded));

            return results;
        }

        private static bool IsMovedType(
            ITypeDefinition oldType,
            ITypeDefinition newType,
            IEnumerable<ITypeDefinition> itemsRemoved,
            IEnumerable<ITypeDefinition> itemsAdded)
        {
            if (oldType.Name != newType.Name)
            {
                // The types don't have the same name
                return false;
            }

            if (oldType.GetType() != newType.GetType())
            {
                // The types don't have the same type
                return false;
            }

            // At this point the types should have different namespaces but we should double check just in case
            if (oldType.Namespace == newType.Namespace)
            {
                // It hasn't changed
                return false;
            }

            var possibleMatchesRemoved =
                itemsRemoved.Count(x => x.Name == oldType.Name);

            if (possibleMatchesRemoved > 1)
            {
                // There are multiple types with the same type and name in different namespaces
                // We therefore have to assume this isn't necessarily a change in namespace of a single class
                return false;
            }

            var possibleMatchesAdded =
                itemsAdded.Count(x => x.Name == oldType.Name);

            if (possibleMatchesAdded > 1)
            {
                // There are multiple types with the same type and name in different namespaces
                // We therefore have to assume this isn't necessarily a change in namespace of a single class
                return false;
            }

            // We have a type that looks like it has changed namespace
            return true;
        }
    }
}