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

    public class Evaluator : IEvaluator
    {
        private readonly IList<IMemberMatcher> _matchers;
        private readonly IList<IMemberComparer> _comparers;
        private readonly ILogger _logger;
        private readonly IScanner _scanner;

        public Evaluator(IScanner scanner, IEnumerable<IMemberMatcher> matchers, IEnumerable<IMemberComparer> comparers, ILogger logger)
        {
            Ensure.Any.IsNotNull(scanner, nameof(scanner));
            Ensure.Any.IsNotNull(matchers, nameof(matchers));
            Ensure.Any.IsNotNull(comparers, nameof(comparers));
            Ensure.Any.IsNotNull(logger, nameof(logger));

            _scanner = scanner;
            _matchers = matchers.FastToList();
            
            Ensure.Collection.HasItems(_matchers, nameof(_matchers));

            _comparers = comparers.FastToList();

            Ensure.Collection.HasItems(_comparers, nameof(_comparers));

            _logger = logger;
        }

        public ChangeType CompareNodes(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes)
        {
            Ensure.Any.IsNotNull(oldNodes, nameof(oldNodes));
            Ensure.Any.IsNotNull(newNodes, nameof(newNodes));

            var matches = new List<MemberMatch>();

            var oldMembers = _scanner.FindDefinitions(oldNodes).FastToList();
            var newMembers = _scanner.FindDefinitions(newNodes).FastToList();

            for (var oldIndex = oldMembers.Count - 1; oldIndex >= 0; oldIndex--)
            {
                var oldMember = oldMembers[oldIndex];
                var matcher = _matchers.FirstOrDefault(x => x.IsSupported(oldMember));

                if (matcher == null)
                {
                    // There is no matcher that supports this member type
                    var message = string.Format(CultureInfo.CurrentCulture, "No {0} found that supports {1}", nameof(IMemberMatcher), oldMember.GetType().FullName);

                    throw new InvalidOperationException(message);
                }

                MemberMatch match = null;

                for (var newIndex = newMembers.Count - 1; newIndex >= 0; newIndex--)
                {
                    var newMember = newMembers[newIndex];

                    match = DetermineMatch(oldMember, newMember, matcher);

                    if (match != null)
                    {
                        // Track the match
                        matches.Add(match);

                        // Remove the indices
                        oldMembers.RemoveAt(oldIndex);
                        newMembers.RemoveAt(newIndex);

                        break;
                    }
                }

                if (match != null)
                {
                    break;
                }
            }

            // Check if there are any old members that didn't match where they were publicly visible - breaking change
            if (oldMembers.Any(x => x.IsPublic))
            {
                return ChangeType.Breaking;
            }

            var changeType = ChangeType.None;

            // Check all the matches for a breaking change or feature added
            foreach (var match in matches)
            {
                var comparer = _comparers.FirstOrDefault(x => x.IsSupported(match.OldMember));

                if (comparer == null)
                {
                    // There is no comparer that supports this member type
                    var message = string.Format(CultureInfo.CurrentCulture, "No {0} found that supports {1}", nameof(IMemberComparer), match.OldMember.GetType().FullName);

                    throw new InvalidOperationException(message);
                }

                var matchChange = comparer.Compare(match.OldMember, match.NewMember);
                
                if (matchChange == ChangeType.Breaking)
                {
                    // We can't get a worse result so no point continuing
                    return ChangeType.Breaking;
                }

                if (matchChange > changeType)
                {
                    // This should be an increase from None to Feature
                    changeType = matchChange;
                }
            }

            if (changeType > ChangeType.None)
            {
                // From here on we can't find any more breaking changes
                // This really should be just a feature change at this point
                Debug.Assert(changeType == ChangeType.Feature);

                return changeType;
            }

            // Check all the new members that didn't match where they are publicly visible - feature change
            if (newMembers.Any(x => x.IsPublic))
            {
                // New members added. At this point nothing has been found to be removed or altered to produce a breaking change
                return ChangeType.Feature;
            }

            // otherwise there is no change
            return ChangeType.None;
        }

        private MemberMatch DetermineMatch(MemberDefinition oldMember, MemberDefinition newMember, IMemberMatcher matcher)
        {
            if (oldMember.GetType() != newMember.GetType())
            {
                // The member types are different
                return null;
            }

            var match = matcher.GetMatch(oldMember, newMember);

            return null;
        }
    }
}