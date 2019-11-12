using System.Collections.Generic;
using System.Collections.ObjectModel;
using EnsureThat;

namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class MatchResults
    {
        public MatchResults(IEnumerable<MemberMatch> matches, IEnumerable<MemberDefinition> oldMembersNotMatched,
            IEnumerable<MemberDefinition> newMembersNotMatched)
        {
            Ensure.Any.IsNotNull(matches, nameof(matches));
            Ensure.Any.IsNotNull(oldMembersNotMatched, nameof(oldMembersNotMatched));
            Ensure.Any.IsNotNull(newMembersNotMatched, nameof(newMembersNotMatched));

            Matches = new ReadOnlyCollection<MemberMatch>(matches.FastToList());
            OldMembersNotMatched = new ReadOnlyCollection<MemberDefinition>(oldMembersNotMatched.FastToList());
            NewMembersNotMatched = new ReadOnlyCollection<MemberDefinition>(newMembersNotMatched.FastToList());
        }

        public IReadOnlyCollection<MemberDefinition> OldMembersNotMatched { get; }
        public IReadOnlyCollection<MemberDefinition> NewMembersNotMatched { get; }
        public IReadOnlyCollection<MemberMatch> Matches { get; }
    }
}