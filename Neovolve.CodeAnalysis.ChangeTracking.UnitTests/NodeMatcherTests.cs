namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class NodeMatcherTests
    {
        [Fact]
        public void GetMatchReturnsMatcherWhenNodesHaveSameIdentifiers()
        {
            var oldDefinition = Model.Create<NodeDefinition>();
            var newDefinition = Model.Create<NodeDefinition>()
                .Set(x =>
                {
                    x.Namespace = oldDefinition.Namespace;
                    x.Name = oldDefinition.Name;
                    x.OwningType = oldDefinition.OwningType;
                });

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldDefinition, newDefinition);

            actual.Should().NotBeNull();
            actual.NewDefinition.Should().Be(newDefinition);
            actual.OldDefinition.Should().Be(oldDefinition);
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereNameIsDifferent(string oldValue, string newValue)
        {
            var oldDefinition = Model.Create<NodeDefinition>().Set(x => x.Name = oldValue);
            var newDefinition = Model.Create<NodeDefinition>()
                .Set(x =>
                {
                    x.Namespace = oldDefinition.Namespace;
                    x.Name = newValue;
                    x.OwningType = oldDefinition.OwningType;
                });

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldDefinition, newDefinition);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereNamespaceIsDifferent(string oldValue, string newValue)
        {
            var oldDefinition = Model.Create<NodeDefinition>().Set(x => x.Namespace = oldValue);
            var newDefinition = Model.Create<NodeDefinition>()
                .Set(x =>
                {
                    x.Namespace = newValue;
                    x.Name = oldDefinition.Name;
                    x.OwningType = oldDefinition.OwningType;
                });

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldDefinition, newDefinition);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereOwningTypeIsDifferent(string oldValue, string newValue)
        {
            var oldDefinition = Model.Create<NodeDefinition>().Set(x => x.OwningType = oldValue);
            var newDefinition = Model.Create<NodeDefinition>()
                .Set(x =>
                {
                    x.Namespace = oldDefinition.Namespace;
                    x.Name = oldDefinition.Name;
                    x.OwningType = newValue;
                });

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldDefinition, newDefinition);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetMatchThrowsExceptionWithNullNewDefinition()
        {
            var oldDefinition = Model.Create<NodeDefinition>();

            var sut = new NodeMatcher();

            Action action = () => sut.GetMatch(oldDefinition, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetMatchThrowsExceptionWithNullOldDefinition()
        {
            var newDefinition = Model.Create<NodeDefinition>();

            var sut = new NodeMatcher();

            Action action = () => sut.GetMatch(null, newDefinition);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}