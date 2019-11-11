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
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>();
            var newNode = oldNode.JsonClone();

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldNode, newNode);

            actual.Should().NotBeNull();
            actual.NewNode.Should().Be(newNode);
            actual.OldNode.Should().Be(oldNode);
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereNameIsDifferent(string oldValue, string newValue)
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>().Set(x => x.Name = oldValue);
            var newNode = oldNode.JsonClone().Set(x => x.Name = newValue);

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldNode, newNode);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereNamespaceIsDifferent(string oldValue, string newValue)
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>().Set(x => x.Namespace = oldValue);
            var newNode = oldNode.JsonClone().Set(x => x.Namespace = newValue);

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldNode, newNode);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("Some", "Other")]
        [InlineData("Some", "some")]
        public void GetMatchReturnsNullWhereOwningTypeIsDifferent(string oldValue, string newValue)
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>().Set(x => x.OwningType = oldValue);
            var newNode = oldNode.JsonClone().Set(x => x.OwningType = newValue);

            var sut = new NodeMatcher();

            var actual = sut.GetMatch(oldNode, newNode);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetMatchThrowsExceptionWithNullNewNode()
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>();

            var sut = new NodeMatcher();

            Action action = () => sut.GetMatch(oldNode, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetMatchThrowsExceptionWithNullOldNode()
        {
            var newNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>();

            var sut = new NodeMatcher();

            Action action = () => sut.GetMatch(null, newNode);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(NodeDefinition), true)]
        [InlineData(typeof(PropertyDefinition), false)]
        [InlineData(typeof(AttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (NodeDefinition) Model.Create(type);

            var sut = new NodeMatcher();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new NodeMatcher();

            Action action = () => sut.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}