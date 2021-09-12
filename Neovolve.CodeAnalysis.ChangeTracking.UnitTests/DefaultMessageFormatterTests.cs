namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultMessageFormatterTests : Tests<DefaultMessageFormatter>
    {
        [Theory]
        [InlineData("type reference", "Type reference")]
        [InlineData("Type reference", "Type reference")]
        public void FormatItemChangedMessageFormatsMessageWithProvidedArguments(string prefix, string expectedPrefix)
        {
            const string? messageFormat = "{Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, oldValue, newValue);

            var firstDefinition = Substitute.For<IClassDefinition>();
            var secondDefinition = Substitute.For<IClassDefinition>();
            var match = new ItemMatch<IClassDefinition>(firstDefinition, secondDefinition);

            Service<IIdentifierFormatter>().FormatIdentifier(match.NewItem, ItemFormatType.ItemChanged)
                .Returns(prefix + " " + identifier);

            var actual = SUT.FormatMatch(match, ItemFormatType.ItemChanged, arguments);

            actual.Should().Be($"{expectedPrefix} {identifier} - {oldValue} - {newValue}");
        }

        [Theory]
        [InlineData("type reference", "Type reference")]
        [InlineData("Type reference", "Type reference")]
        public void FormatItemFormatsMessageWithProvidedArguments(string prefix, string expectedPrefix)
        {
            const string? messageFormat = "{Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, oldValue, newValue);

            var definition = Substitute.For<IClassDefinition>();

            Service<IIdentifierFormatter>().FormatIdentifier(definition, ItemFormatType.ItemAdded).Returns(prefix + " " + identifier);

            var actual = SUT.FormatItem(definition, ItemFormatType.ItemAdded, arguments);

            actual.Should().Be($"{expectedPrefix} {identifier} - {oldValue} - {newValue}");
        }

        [Fact]
        public void FormatItemThrowsExceptionWithNullArguments()
        {
            var definition = Substitute.For<IItemDefinition>();

            Action action = () => SUT.FormatItem(definition, ItemFormatType.ItemAdded, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FormatItemThrowsExceptionWithNullDefinition()
        {
            var arguments = Model.Create<FormatArguments>();

            Action action = () => SUT.FormatItem(null!, ItemFormatType.ItemChanged, arguments);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullIdentifyFormatter()
        {
            Action action = () => new DefaultMessageFormatter(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}