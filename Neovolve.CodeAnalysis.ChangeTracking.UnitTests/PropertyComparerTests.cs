namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class PropertyComparerTests
    {
        [Fact]
        public void CompareReturnsBreakingWhenFeatureAlsoIndicated()
        {
            var oldNode = Model.Create<PropertyDefinition>()
                .Set(x =>
                {
                    x.IsPublic = false;
                    x.ReturnType = "string";
                    x.CanRead = true;
                    x.CanWrite = true;
                });
            var newNode = Model.Create<PropertyDefinition>()
                .Set(x => x.ReturnType = "DateTimeOffset")
                .Set(x =>
                {
                    x.IsPublic = true; // Feature
                    x.ReturnType = oldNode.ReturnType;
                    x.CanRead = true;
                    x.CanWrite = false; // Breaking
                });

            var sut = new PropertyComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(ChangeType.Breaking);
        }

        [Theory]
        [InlineData(false, false, ChangeType.None)]
        [InlineData(true, true, ChangeType.None)]
        [InlineData(true, false, ChangeType.Breaking)]
        [InlineData(false, true, ChangeType.Feature)]
        public void CompareReturnsBaseResultWhenPropertyAccessorsHaveSameVisibility(bool oldValue, bool newValue, ChangeType expected)
        {
            var oldNode = Model.Create<PropertyDefinition>().Set(x =>
            {
                x.IsPublic = oldValue;
                x.CanRead = true;
                x.CanWrite = true;
            });
            var newNode = Model.Create<PropertyDefinition>()
                .Set(x =>
                {
                    x.IsPublic = newValue;
                    x.ReturnType = oldNode.ReturnType;
                    x.CanRead = true;
                    x.CanWrite = true;
                });

            var sut = new PropertyComparer();

            var actual = sut.Compare(oldNode, newNode);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CompareReturnsNoneWhenChangedAccessorsExistAgainstHiddenProperties()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public void CompareReturnsFeatureWhenBreakingChangeOnAccessorsAndPropertyNowVisible()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public void CompareReturnsResultOnChangesToGetAccessorVisibility()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public void CompareReturnsResultOnChangesToSetAccessorVisibility()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullNewNode()
        {
            var oldNode = Model.Create<PropertyDefinition>();

            var sut = new PropertyComparer();

            Action action = () => sut.Compare(oldNode, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullOldNode()
        {
            var newNode = Model.Create<PropertyDefinition>();

            var sut = new PropertyComparer();

            Action action = () => sut.Compare(null, newNode);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(PropertyDefinition), true)]
        [InlineData(typeof(NodeDefinition), false)]
        [InlineData(typeof(AttributeDefinition), false)]
        public void IsSupportedReturnsTrueForExactTypeMatch(Type type, bool expected)
        {
            var definition = (NodeDefinition) Model.Create(type);

            var sut = new PropertyComparer();

            var actual = sut.IsSupported(definition);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new PropertyComparer();

            Action action = () => sut.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}