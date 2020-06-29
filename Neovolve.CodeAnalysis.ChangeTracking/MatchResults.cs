using System.Collections.Generic;
using System.Collections.ObjectModel;
using EnsureThat;

namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class MatchResults
    {
        public MatchResults(IEnumerable<MemberMatch> matches, IEnumerable<OldMemberDefinition> oldMembersNotMatched,
            IEnumerable<OldMemberDefinition> newMembersNotMatched)
        {
            Ensure.Any.IsNotNull(matches, nameof(matches));
            Ensure.Any.IsNotNull(oldMembersNotMatched, nameof(oldMembersNotMatched));
            Ensure.Any.IsNotNull(newMembersNotMatched, nameof(newMembersNotMatched));

            Matches = new ReadOnlyCollection<MemberMatch>(matches.FastToList());
            OldMembersNotMatched = new ReadOnlyCollection<OldMemberDefinition>(oldMembersNotMatched.FastToList());
            NewMembersNotMatched = new ReadOnlyCollection<OldMemberDefinition>(newMembersNotMatched.FastToList());
        }

        public IReadOnlyCollection<OldMemberDefinition> OldMembersNotMatched { get; }
        public IReadOnlyCollection<OldMemberDefinition> NewMembersNotMatched { get; }
        public IReadOnlyCollection<MemberMatch> Matches { get; }
    }
}