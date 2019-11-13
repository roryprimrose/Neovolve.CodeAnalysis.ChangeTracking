namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class MatchEvaluator : IMatchEvaluator
    {
        private readonly ILogger _logger;
        private readonly IList<IMemberMatcher> _matchers;
        private readonly INodeScanner _nodeScanner;

        public MatchEvaluator(INodeScanner nodeScanner, IEnumerable<IMemberMatcher> matchers, ILogger logger)
        {
            Ensure.Any.IsNotNull(nodeScanner, nameof(nodeScanner));
            Ensure.Any.IsNotNull(matchers, nameof(matchers));

            _nodeScanner = nodeScanner;
            _matchers = matchers.FastToList();

            Ensure.Collection.HasItems(_matchers, nameof(_matchers));

            _logger = logger;
        }

        public MatchResults CompareNodes(IEnumerable<SyntaxNode> oldNodes, IEnumerable<SyntaxNode> newNodes)
        {
            Ensure.Any.IsNotNull(oldNodes, nameof(oldNodes));
            Ensure.Any.IsNotNull(newNodes, nameof(newNodes));

            var matches = new List<MemberMatch>();

            var oldMembers = _nodeScanner.FindDefinitions(oldNodes).FastToList();
            var newMembers = _nodeScanner.FindDefinitions(newNodes).FastToList();

            // Loop in reverse so that we can remove matched members as we go
            // Removing matched members as we find them means that we have less iterations of the inner loop for each subsequent old member
            // The set of old members and new members can also then be reported as not matches once all matches are removed
            // A for loop is required here because removing items would break a foreach iterator
            for (var oldIndex = oldMembers.Count - 1; oldIndex >= 0; oldIndex--)
            {
                var oldMember = oldMembers[oldIndex];
                var matcher = _matchers.FirstOrDefault(x => x.IsSupported(oldMember));

                if (matcher == null)
                {
                    // There is no matcher that supports this member type
                    var message = string.Format(CultureInfo.CurrentCulture, "No {0} found that supports {1}",
                        nameof(IMemberMatcher), oldMember.GetType().FullName);

                    throw new InvalidOperationException(message);
                }

                for (var newIndex = newMembers.Count - 1; newIndex >= 0; newIndex--)
                {
                    var newMember = newMembers[newIndex];

                    if (matcher.IsSupported(newMember) == false)
                    {
                        // The matcher that supports the old node doesn't support the new node
                        // They cannot match so continue searching other new nodes
                        continue;
                    }

                    var match = matcher.GetMatch(oldMember, newMember);

                    if (match != null)
                    {
                        _logger?.LogDebug("Found match on " + match.OldMember);

                        // Track the match
                        matches.Add(match);

                        // Remove the indices
                        oldMembers.RemoveAt(oldIndex);
                        newMembers.RemoveAt(newIndex);

                        break;
                    }
                }
            }

            return new MatchResults(matches, oldMembers, newMembers);
        }
    }
}