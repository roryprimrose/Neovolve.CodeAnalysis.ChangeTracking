namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class ChangeCalculator : IChangeCalculator
    {
        private readonly IList<IMemberComparer> _comparers;
        private readonly IMatchEvaluator _evaluator;
        private readonly ILogger? _logger;

        public ChangeCalculator(IMatchEvaluator evaluator, IEnumerable<IMemberComparer> comparers, ILogger? logger)
        {
            Ensure.Any.IsNotNull(evaluator, nameof(evaluator));
            Ensure.Any.IsNotNull(comparers, nameof(comparers));

            _evaluator = evaluator;
            _comparers = comparers.FastToList();

            Ensure.Collection.HasItems(_comparers, nameof(_comparers));

            _logger = logger;
        }

        public ChangeCalculatorResult CalculateChanges(IEnumerable<SyntaxNode> oldNodes,
            IEnumerable<SyntaxNode> newNodes)
        {
            Ensure.Any.IsNotNull(oldNodes, nameof(oldNodes));
            Ensure.Any.IsNotNull(newNodes, nameof(newNodes));

            var matchingNodes = _evaluator.CompareNodes(oldNodes, newNodes);

            var results = new ChangeCalculatorResult();

            // Record any public members that have been added
            foreach (var memberAdded in matchingNodes.NewMembersNotMatched.Where(x => x.IsPublic))
            {
                var memberType = memberAdded.MemberType.ToString().ToLower(CultureInfo.CurrentCulture);

                _logger?.LogInformation(
                    "Found new public {0} {1} that does not match any old public {2} (feature)",
                    memberType, memberAdded.ToString(false),
                    memberType);

                var memberRemovedResult = ComparisonResult.MemberAdded(memberAdded);

                results.Add(memberRemovedResult);
            }

            // Record any public members that have been removed
            foreach (var memberRemoved in matchingNodes.OldMembersNotMatched.Where(x => x.IsPublic))
            {
                var memberType = memberRemoved.MemberType.ToString().ToLower(CultureInfo.CurrentCulture);

                _logger?.LogInformation(
                    "Found old public {0} {1} that does not match any new public {2} (breaking)",
                    memberType, memberRemoved.ToString(false),
                    memberType);

                var memberRemovedResult = ComparisonResult.MemberRemoved(memberRemoved);

                results.Add(memberRemovedResult);
            }

            // Check all the matches for a breaking change or feature added
            foreach (var match in matchingNodes.Matches)
            {
                var comparer = _comparers.FirstOrDefault(x => x.IsSupported(match.OldMember));

                if (comparer == null)
                {
                    // There is no comparer that supports this member type
                    var message = string.Format(CultureInfo.CurrentCulture, "No {0} found that supports {1}",
                        nameof(IMemberComparer), match.OldMember.GetType().FullName);

                    throw new InvalidOperationException(message);
                }

                var result = comparer.Compare(match);

                if (result.ChangeType == SemVerChangeType.None)
                {
                    _logger?.LogDebug(result.Message);

                    // Don't add comparison results to the outcome where it looks like there is no change
                    continue;
                }

                _logger?.LogInformation(result.Message);

                results.Add(result);

                if (result.ChangeType == SemVerChangeType.Breaking)
                {
                    _logger?.LogInformation("Identified a potential breaking change in {0} {1}.",
                        match.OldMember.MemberType.ToString().ToLower(CultureInfo.CurrentCulture),
                        match.OldMember.ToString(false));
                }
                else if (result.ChangeType == SemVerChangeType.Feature)
                {
                    _logger?.LogInformation("Identified a potential {0} change in {1} {2}.",
                        result.ToString().ToLower(CultureInfo.CurrentCulture),
                        match.OldMember.MemberType.ToString().ToLower(CultureInfo.CurrentCulture),
                        match.OldMember.ToString(false));
                }
            }

            _logger?.LogInformation("Calculated overall result as a {0} change.",
                results.ChangeType.ToString().ToLower(CultureInfo.CurrentCulture));

            return results;
        }
    }
}