namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class ChangeCalculator : IChangeCalculator
    {
        private readonly IList<IMemberComparer> _comparers;
        private readonly IMatchEvaluator _evaluator;
        private readonly ILogger _logger;

        public ChangeCalculator(IMatchEvaluator evaluator, IEnumerable<IMemberComparer> comparers, ILogger logger)
        {
            Ensure.Any.IsNotNull(evaluator, nameof(evaluator));
            Ensure.Any.IsNotNull(comparers, nameof(comparers));
            Ensure.Any.IsNotNull(logger, nameof(logger));

            _evaluator = evaluator;
            _comparers = comparers.FastToList();

            Ensure.Collection.HasItems(_comparers, nameof(_comparers));

            _logger = logger;
        }

        public ChangeType CalculateChange(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes)
        {
            Ensure.Any.IsNotNull(oldNodes, nameof(oldNodes));
            Ensure.Any.IsNotNull(newNodes, nameof(newNodes));

            var results = _evaluator.CompareNodes(oldNodes, newNodes);

            if (results == null)
            {
                _logger.LogInformation("No member matches found, assuming no code change.");

                return ChangeType.None;
            }

            // Check if there are any old members that didn't match where they were publicly visible - breaking change
            if (results.OldMembersNotMatched.Any(x => x.IsPublic))
            {
                _logger.LogInformation(
                    "Found old public members that do not match new members. This indicates a breaking change.");

                return ChangeType.Breaking;
            }

            var changeType = ChangeType.None;

            // Check all the matches for a breaking change or feature added
            foreach (var match in results.Matches)
            {
                var comparer = _comparers.FirstOrDefault(x => x.IsSupported(match.OldMember));

                if (comparer == null)
                {
                    // There is no comparer that supports this member type
                    var message = string.Format(CultureInfo.CurrentCulture, "No {0} found that supports {1}",
                        nameof(IMemberComparer), match.OldMember.GetType().FullName);

                    throw new InvalidOperationException(message);
                }

                var matchChange = comparer.Compare(match.OldMember, match.NewMember);

                if (matchChange == ChangeType.Breaking)
                {
                    _logger.LogInformation("Found member match on {0} where {1} identified a breaking change.",
                        match.OldMember, comparer.GetType().Name);

                    // We can't get a worse result so no point continuing
                    return ChangeType.Breaking;
                }

                if (matchChange > changeType)
                {
                    _logger.LogInformation("Found member match on {0} where {1} identified a {2} change.",
                        match.OldMember, comparer.GetType().Name,
                        matchChange.ToString().ToLower(CultureInfo.CurrentCulture));

                    // This should be an increase from None to Feature
                    changeType = matchChange;
                }
            }

            if (changeType > ChangeType.None)
            {
                _logger.LogInformation("Calculated overall change type as {0}.",
                    changeType.ToString().ToLower(CultureInfo.CurrentCulture));

                // From here on we can't find any more breaking changes
                // This really should be just a feature change at this point
                Debug.Assert(changeType == ChangeType.Feature);

                return changeType;
            }

            // Check all the new members that didn't match where they are publicly visible - feature change
            if (results.NewMembersNotMatched.Any(x => x.IsPublic))
            {
                _logger.LogInformation(
                    "Found new public members that do not match old members. This indicates a new feature.");

                // New members added. At this point nothing has been found to be removed or altered to produce a breaking change
                return ChangeType.Feature;
            }

            // No change identified
            _logger.LogInformation("No changes identified between the old and new members.");

            return ChangeType.None;
        }
    }
}