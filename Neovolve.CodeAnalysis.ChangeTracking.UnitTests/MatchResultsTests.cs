// ReSharper disable ObjectCreationAsStatement

namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    public class MatchResultsTests
    {
        //[Fact]
        //[SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        //public void ThrowsExceptionWhenCreatedWithNullMatches()
        //{
        //    var oldMembersNotMatched = Model.UsingModule<ConfigurationModule>().Create<IEnumerable<OldMemberDefinition>>();
        //    var newMembersNotMatched = Model.UsingModule<ConfigurationModule>().Create<IEnumerable<OldMemberDefinition>>();

        //    Action action = () => new MatchResults(null!, oldMembersNotMatched, newMembersNotMatched);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //[SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        //public void ThrowsExceptionWhenCreatedWithNullOldMembersNotMatched()
        //{
        //    var matches = Model.UsingModule<ConfigurationModule>().Create<IEnumerable<DefinitionMatch>>();
        //    var newMembersNotMatched = Model.UsingModule<ConfigurationModule>().Create<IEnumerable<OldMemberDefinition>>();

        //    Action action = () => new MatchResults(matches, null!, newMembersNotMatched);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //[SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        //public void ThrowsExceptionWhenCreatedWithNullNewMembersNotMatched()
        //{
        //    var matches = Model.UsingModule<ConfigurationModule>().Create<IEnumerable<DefinitionMatch>>();
        //    var oldMembersNotMatched = Model.UsingModule<ConfigurationModule>().Create<IEnumerable<OldMemberDefinition>>();

        //    Action action = () => new MatchResults(matches, oldMembersNotMatched, null!);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //public void CanCreateWithRequiredValues()
        //{
        //    var matches = Model.UsingModule<ConfigurationModule>().Create<IList<DefinitionMatch>>();
        //    var oldMembersNotMatched = Model.UsingModule<ConfigurationModule>().Create<IList<OldMemberDefinition>>();
        //    var newMembersNotMatched = Model.UsingModule<ConfigurationModule>().Create<IList<OldMemberDefinition>>();

        //    var sut = new MatchResults(matches, oldMembersNotMatched, newMembersNotMatched);

        //    sut.MatchingItems.Should().BeEquivalentTo(matches);
        //    sut.ItemsRemoved.Should().BeEquivalentTo(oldMembersNotMatched);
        //    sut.ItemsAdded.Should().BeEquivalentTo(newMembersNotMatched);
        //}
    }
}