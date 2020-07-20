namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class ItemMatchTests
    {
        [Fact]
        public void PropertiesReturnConstructorValues()
        {
            var oldItem = new TestPropertyDefinition();
            var newItem = new TestPropertyDefinition();

            var sut = new ItemMatch<IPropertyDefinition>(oldItem, newItem);

            sut.OldItem.Should().Be(oldItem);
            sut.NewItem.Should().Be(newItem);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNewItem()
        {
            var oldItem = new TestPropertyDefinition();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ItemMatch<IPropertyDefinition>(oldItem, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOldItem()
        {
            var newItem = new TestPropertyDefinition();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ItemMatch<IPropertyDefinition>(null!, newItem);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}