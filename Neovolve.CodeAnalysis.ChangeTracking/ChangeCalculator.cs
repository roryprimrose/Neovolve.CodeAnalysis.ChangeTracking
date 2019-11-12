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
            var oldPublicMember = results.OldMembersNotMatched.FirstOrDefault(x => x.IsPublic);

            if (oldPublicMember != null)
            {
                var memberType = oldPublicMember.MemberType.ToLower(CultureInfo.CurrentCulture);

                _logger.LogInformation(
                    "Found old public {0} {1} that does not match any new public {2}. This indicates a breaking change.",
                    memberType, oldPublicMember.ToString(false),
                    memberType);

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

                var matchChange = comparer.Compare(match);

                if (matchChange == ChangeType.Breaking)
                {
                    _logger.LogInformation("Identified a potential breaking change in {0} {1}.",
                        match.OldMember.MemberType.ToLower(CultureInfo.CurrentCulture),
                        match.OldMember.ToString(false));

                    // We can't get a worse result so no point continuing
                    return ChangeType.Breaking;
                }

                if (matchChange > changeType)
                {
                    _logger.LogInformation("Identified a potential {0} change in {1} {2}.",
                        matchChange.ToString().ToLower(CultureInfo.CurrentCulture),
                        match.OldMember.MemberType.ToLower(CultureInfo.CurrentCulture),
                        match.OldMember.ToString(false));

                    // This should be an increase from None to Feature
                    changeType = matchChange;
                }
            }

            if (changeType > ChangeType.None)
            {
                _logger.LogInformation("Calculated overall result as a {0} change.",
                    changeType.ToString().ToLower(CultureInfo.CurrentCulture));

                // From here on we can't find any more breaking changes
                // This really should be just a feature change at this point
                Debug.Assert(changeType == ChangeType.Feature);

                return changeType;
            }

            // Check all the new members that didn't match where they are publicly visible - feature change
            var newPublicMember = results.NewMembersNotMatched.FirstOrDefault(x => x.IsPublic);

            if (newPublicMember != null)
            {
                var memberType = newPublicMember.MemberType.ToLower(CultureInfo.CurrentCulture);

                _logger.LogInformation(
                    "Found new public {0} {1} that does not match any old public {2}. This indicates a new feature.",
                    memberType, newPublicMember.ToString(false), memberType);

                // New members added. At this point nothing has been found to be removed or altered to produce a breaking change
                return ChangeType.Feature;
            }

            // No change identified
            _logger.LogInformation("No changes identified between the old and new members.");

            return ChangeType.None;
        }
    }
}