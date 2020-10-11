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

            var matches = new List<ItemMatch<IMethodDefinition>>();
            var oldMethods = oldItems.ToList();
            var newMethods = newItems.ToList();

            // Loop in reverse so that the items in the loop can be removed safely
            for (var index = oldMethods.Count - 1; index >= 0; index--)
            {
                var oldMethod = oldMethods[index];

                // Get the new methods that have the same name
                var matchingMethods = newMethods.Where(x => x.RawName == oldMethod.RawName).ToList();

                if (matchingMethods.Count == 0)
                {
                    // This method has been removed
                    continue;
                }

                if (matchingMethods.Count == 1)
                {
                    // This method may have changed but it is assumed to be directly related to the new method
                    var match = new ItemMatch<IMethodDefinition>(oldMethod, matchingMethods[0]);

                    oldMethods.RemoveAt(index);
                    newMethods.Remove(matchingMethods[0]);

                    matches.Add(match);

                    continue;
                }

                // There are more than one method that matches the name of the old method
                // The only reliable match will be to attempt to match on the parameters
                // First start with an exact match on the parameter types
                var matchByParameterTypes = FindMethodMatchingParameterTypes(oldMethod, matchingMethods);

                if (matchByParameterTypes != null)
                {
                    var match = new ItemMatch<IMethodDefinition>(oldMethod, matchByParameterTypes);

                    oldMethods.RemoveAt(index);
                    newMethods.Remove(matchByParameterTypes);

                    matches.Add(match);

                    continue;
                }

                // Then match on the same number of parameters if there is a single match
                var matchByParameterCounts = FindMethodMatchingParameterCount(oldMethod, matchingMethods);

                if (matchByParameterCounts != null)
                {
                    var match = new ItemMatch<IMethodDefinition>(oldMethod, matchByParameterCounts);

                    oldMethods.RemoveAt(index);
                    newMethods.Remove(matchByParameterCounts);

                    matches.Add(match);
                }
            }

            return new MatchResults<IMethodDefinition>(matches, oldMethods, newMethods);
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

        private static IMethodDefinition? FindMethodMatchingParameterCount(
            IMethodDefinition oldMethod,
            IEnumerable<IMethodDefinition> newMethods)
        {
            var methodsWithSameCount = newMethods.Where(x => x.Parameters.Count == oldMethod.Parameters.Count).ToList();

            if (methodsWithSameCount.Count == 1)
            {
                // There is only one method with the same number of parameters
                // The assumption here is that it is the same method but the parameter types have been changed
                return methodsWithSameCount[0];
            }

            // There are either no methods with the same parameter count or there are multiple methods with the same parameter count
            // We can't reliable determine a match between the old method and the new methods
            return null;
        }

        private static IMethodDefinition? FindMethodMatchingParameterTypes(
            IMethodDefinition oldMethod,
            IEnumerable<IMethodDefinition> newMethods)
        {
            // SingleOrDefault is more technically correct here but there should only ever be one method matching by name and exact parameter list
            // FirstOrDefault allows for an early exit on the loop whereas SingleOrDefault forces a full enumeration
            return newMethods.FirstOrDefault(x => ParameterListMatches(oldMethod, x));
        }
    }
}