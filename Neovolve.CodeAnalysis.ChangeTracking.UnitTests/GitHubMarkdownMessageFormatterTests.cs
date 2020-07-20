namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class GitHubMarkdownMessageFormatterTests
    {
        [Fact]
        public void FormatItemChangedMessageFormatsMessageWithMarkup()
        {
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var match = new ItemMatch<IPropertyDefinition>(new TestPropertyDefinition(), new TestPropertyDefinition());
            var arguments = new FormatArguments("{DefinitionType} {Identifier} {OldValue} {NewValue}", identifier,
                oldValue, newValue);

            var sut = new GitHubMarkdownMessageFormatter();

            var actual = sut.FormatItemChangedMessage(match, arguments);

            actual.Should().Be($"Property `{identifier}` `{oldValue}` `{newValue}`");
        }
    }
}