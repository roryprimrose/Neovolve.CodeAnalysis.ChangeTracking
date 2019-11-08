namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class NodeMatchTests
    {
        [Fact]
        public void CanCreateWithTwoNodes()
        {
            var oldNode = Model.Create<NodeDefinition>();
            var newNode = Model.Create<NodeDefinition>();

            var sut = new NodeMatch(oldNode, newNode);

            sut.OldNode.Should().Be(oldNode);
            sut.NewNode.Should().Be(newNode);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNewNode()
        {
            var oldNode = Model.Create<NodeDefinition>();

            Action action = () => new NodeMatch(oldNode, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOldNode()
        {
            var newNode = Model.Create<NodeDefinition>();

            Action action = () => new NodeMatch(null, newNode);

            action.Should().Throw<ArgumentException>();
        }
    }
}