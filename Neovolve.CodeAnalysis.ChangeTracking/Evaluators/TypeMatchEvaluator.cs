namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeMatchEvaluator : MatchEvaluator
    {
        protected override IMatchResults<T> BuildMatchResults<T>(List<ItemMatch<T>> matches, List<T> itemsRemoved,
            List<T> itemsAdded)
        {
            matches = matches ?? throw new ArgumentNullException(nameof(matches));
            itemsRemoved = itemsRemoved ?? throw new ArgumentNullException(nameof(itemsRemoved));
            itemsAdded = itemsAdded ?? throw new ArgumentNullException(nameof(itemsAdded));

            // We want to check if there is a rename on a type before we return the match results
            // The reason this is required is because the TypeComparer class will only evaluate items as matches when they exist in the same namespace
            // If a type has moved interface it will otherwise be identified as a remove + add rather than a namespace rename

            // Loop in reverse so that we can remove matched members as we go
            // Removing matched members as we find them means that we have less iterations of the inner loop for each subsequent old member
            // The set of old members and new members can also then be reported as not matches once all matches are removed
            // A for loop is required here because removing items would break a foreach iterator
            for (var oldIndex = itemsRemoved.Count - 1; oldIndex >= 0; oldIndex--)
            {
                var oldItem = itemsRemoved[oldIndex];

                if (oldItem == null)
                {
                    continue;
                }

                for (var newIndex = itemsAdded.Count - 1; newIndex >= 0; newIndex--)
                {
                    var newItem = itemsAdded[newIndex];

                    if (newItem == null)
                    {
                        continue;
                    }

                    var isMovedType = IsMovedType(oldItem, newItem, itemsRemoved, itemsAdded);

                    if (isMovedType == false)
                    {
                        continue;
                    }

                    // These items are the same type and name but have different namespaces
                    var match = new ItemMatch<T>(oldItem, newItem);

                    // Track the match
                    matches.Add(match);

                    // Remove the indices
                    itemsAdded.RemoveAt(newIndex);
                    itemsRemoved.RemoveAt(oldIndex);

                    break;
                }
            }

            return base.BuildMatchResults(matches, itemsRemoved, itemsAdded);
        }

        private static bool IsMovedType<T>(IItemDefinition oldItem, IItemDefinition newItem,
            IEnumerable<T> itemsRemoved,
            IEnumerable<T> itemsAdded)
        {
            if (!(oldItem is ITypeDefinition oldType))
            {
                // This is not a type element
                return false;
            }

            if (!(newItem is ITypeDefinition newType))
            {
                // This is not a type element
                return false;
            }

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

            var possibleMatchesRemoved = itemsRemoved.OfType<ITypeDefinition>()
                .Count(x => x.Name == oldType.Name && x.GetType() == oldType.GetType());

            if (possibleMatchesRemoved > 1)
            {
                // There are multiple types with the same type and name in different namespaces
                // We therefore have to assume this isn't necessarily a change in namespace of a single class
                return false;
            }

            var possibleMatchesAdded = itemsAdded.OfType<ITypeDefinition>()
                .Count(x => x.Name == oldType.Name && x.GetType() == oldType.GetType());

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