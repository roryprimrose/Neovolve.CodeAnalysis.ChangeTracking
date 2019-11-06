namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class NodeMatchTests
    {
        [Fact]
        public void CanCreateWithNullNewDefinitions()
        {
            var oldDefinition = Model.Create<NodeDefinition>();

            var sut = new NodeMatch(oldDefinition, null);

            sut.OldDefinition.Should().Be(oldDefinition);
            sut.NewDefinition.Should().BeNull();
        }

        [Fact]
        public void CanCreateWithNullOldDefinitions()
        {
            var newDefinition = Model.Create<NodeDefinition>();

            var sut = new NodeMatch(null, newDefinition);

            sut.OldDefinition.Should().BeNull();
            sut.NewDefinition.Should().Be(newDefinition);
        }

        [Fact]
        public void CanCreateWithTwoDefinitions()
        {
            var oldDefinition = Model.Create<NodeDefinition>();
            var newDefinition = Model.Create<NodeDefinition>();

            var sut = new NodeMatch(oldDefinition, newDefinition);

            sut.OldDefinition.Should().Be(oldDefinition);
            sut.NewDefinition.Should().Be(newDefinition);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithTwoNullDefinitions()
        {
            Action action = () => new NodeMatch(null, null);

            action.Should().Throw<ArgumentException>();
        }
    }
}