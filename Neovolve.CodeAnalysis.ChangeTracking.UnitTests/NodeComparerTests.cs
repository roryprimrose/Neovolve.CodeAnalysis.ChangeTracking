namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class NodeComparerTests
    {
        [Fact]
        public void CompareReturnsFeatureWhenReturnTypeChangedWithPropertyChangedToPublic()
        {
            var oldNode = Model.UsingModule<CompilerModule>()
                .Create<NodeDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.ReturnType = "string";
                });
            var newNode = oldNode.JsonClone()
                .Set(x =>
                {
                    x.IsPublic = true; // Feature
                    x.ReturnType = "DateTimeOffset"; // Breaking
                });

            var sut = new NodeComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(ChangeType.Feature);
        }

        [Fact]
        public void CompareReturnsNoneWhenNodesMatch()
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>();
            var newNode = oldNode.JsonClone();

            var sut = new NodeComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(ChangeType.None);
        }

        [Fact]
        public void CompareReturnsNoneWhenReturnTypeChangedWithPropertyNotPublic()
        {
            var oldNode = Model.UsingModule<CompilerModule>()
                .Create<NodeDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.ReturnType = "string";
                });
            var newNode = oldNode.JsonClone().Set(x => { x.ReturnType = "DateTimeOffset"; });

            var sut = new NodeComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(ChangeType.None);
        }

        [Theory]
        [InlineData(false, false, ChangeType.None)]
        [InlineData(true, true, ChangeType.None)]
        [InlineData(true, false, ChangeType.Breaking)]
        [InlineData(false, true, ChangeType.Feature)]
        public void CompareReturnsResultBasedOnIsPublic(bool oldValue, bool newValue, ChangeType expected)
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>().Set(x => x.IsPublic = oldValue);
            var newNode = oldNode.JsonClone().Set(x => x.IsPublic = newValue);

            var sut = new NodeComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("string", "string", ChangeType.None)]
        [InlineData("string", "DateTimeOffset", ChangeType.Breaking)]
        public void CompareReturnsResultBasedOnReturnType(string oldValue, string newValue, ChangeType expected)
        {
            var oldNode = Model.UsingModule<CompilerModule>()
                .Create<NodeDefinition>()
                .Set(x => x.ReturnType = oldValue);
            var newNode = oldNode.JsonClone().Set(x => x.ReturnType = newValue);

            var sut = new NodeComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CompareThrowsExceptionWithNullNewNode()
        {
            var oldNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>();

            var sut = new NodeComparer();

            Action action = () => sut.Compare(oldNode, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullOldNode()
        {
            var newNode = Model.UsingModule<CompilerModule>().Create<NodeDefinition>();

            var sut = new NodeComparer();

            Action action = () => sut.Compare(null, newNode);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(NodeDefinition), true)]
        [InlineData(typeof(PropertyDefinition), false)]
        [InlineData(typeof(AttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (NodeDefinition) Model.Create(type);

            var sut = new NodeComparer();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new NodeComparer();

            Action action = () => sut.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}