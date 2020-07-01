namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;
    using Xunit.Abstractions;

    public class PropertyComparerTests
    {
        private readonly ITestOutputHelper _output;

        public PropertyComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        //[Theory]
        //[InlineData(false, false, SemVerChangeType.None)]
        //[InlineData(true, true, SemVerChangeType.None)]
        //[InlineData(true, false, SemVerChangeType.Breaking)]
        //[InlineData(false, true, SemVerChangeType.Feature)]
        //public void CompareReturnsBaseResultWhenPropertyAccessorsHaveSameVisibility(
        //    bool oldValue,
        //    bool newValue,
        //    SemVerChangeType expected)
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>()
        //        .Create<OldPropertyDefinition>()
        //        .Set(x => x.IsVisible = oldValue);
        //    var newMember = oldMember.JsonClone().Set(x => x.IsVisible = newValue);
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(expected);
        //}

        //[Fact]
        //public void CompareReturnsBreakingWhenFeatureAlsoIndicated()
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>()
        //        .Create<OldPropertyDefinition>()
        //        .Set(x => { x.CanWrite = false; });
        //    var newMember = oldMember.JsonClone()
        //        .Set(x =>
        //        {
        //            x.ReturnType = Guid.NewGuid().ToString(); // Breaking
        //            x.CanWrite = true; // Feature
        //        });
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //}

        //[Fact]
        //public void CompareReturnsFeatureWhenBreakingChangeOnAccessorsAndPropertyNowVisible()
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>()
        //        .Create<OldPropertyDefinition>()
        //        .Set(x => { x.IsVisible = false; });
        //    var newMember = oldMember.JsonClone()
        //        .Set(x =>
        //        {
        //            x.IsVisible = true;
        //            x.CanWrite = false;
        //        });
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        //}

        //[Fact]
        //public void CompareReturnsNoneWhenAccessorLessVisibleButPropertiesNotPublic()
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>()
        //        .Create<OldPropertyDefinition>()
        //        .Set(x =>
        //        {
        //            x.IsVisible = false;
        //            x.CanWrite = true;
        //        });
        //    var newMember = oldMember.JsonClone().Set(x => { x.CanWrite = false; });
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(SemVerChangeType.None);
        //}

        //[Fact]
        //public void CompareReturnsNoneWhenNodesMatch()
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>().Create<OldPropertyDefinition>();
        //    var newMember = oldMember.JsonClone();
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(SemVerChangeType.None);
        //}

        //[Theory]
        //[InlineData(false, false, SemVerChangeType.None)]
        //[InlineData(true, true, SemVerChangeType.None)]
        //[InlineData(false, true, SemVerChangeType.Feature)]
        //[InlineData(true, false, SemVerChangeType.Breaking)]
        //public void CompareReturnsResultOnChangesToGetAccessorVisibility(
        //    bool oldValue,
        //    bool newValue,
        //    SemVerChangeType expected)
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>()
        //        .Create<OldPropertyDefinition>()
        //        .Set(x => x.CanRead = oldValue);
        //    var newMember = oldMember.JsonClone().Set(x => x.CanRead = newValue);
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(expected);
        //}

        //[Theory]
        //[InlineData(false, false, SemVerChangeType.None)]
        //[InlineData(true, true, SemVerChangeType.None)]
        //[InlineData(false, true, SemVerChangeType.Feature)]
        //[InlineData(true, false, SemVerChangeType.Breaking)]
        //public void CompareReturnsResultOnChangesToSetAccessorVisibility(
        //    bool oldValue,
        //    bool newValue,
        //    SemVerChangeType expected)
        //{
        //    var oldMember = Model.UsingModule<ConfigurationModule>()
        //        .Create<OldPropertyDefinition>()
        //        .Set(x => x.CanWrite = oldValue);
        //    var newMember = oldMember.JsonClone().Set(x => x.CanWrite = newValue);
        //    var match = new DefinitionMatch(oldMember, newMember);

        //    var sut = new PropertyComparer();

        //    var actual = sut.Compare(match);

        //    _output.WriteLine(actual.Message);

        //    actual.ChangeType.Should().Be(expected);
        //}

        //[Fact]
        //public void CompareThrowsExceptionWithNullMatch()
        //{
        //    var sut = new PropertyComparer();

        //    Action action = () => sut.Compare(null!);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Theory]
        //[InlineData(typeof(OldPropertyDefinition), true)]
        //[InlineData(typeof(OldMemberDefinition), false)]
        //[InlineData(typeof(OldAttributeDefinition), false)]
        //public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        //{
        //    var definition = (OldMemberDefinition) Model.UsingModule<ConfigurationModule>().Create(type);

        //    var sut = new PropertyComparer();

        //    var actual = sut.IsSupported(definition);

        //    actual.Should().Be(expected);
        //}

        //[Fact]
        //public void IsSupportedThrowsExceptionWithNullNode()
        //{
        //    var sut = new PropertyComparer();

        //    Action action = () => sut.IsSupported(null!);

        //    action.Should().Throw<ArgumentNullException>();
        //}
    }
}