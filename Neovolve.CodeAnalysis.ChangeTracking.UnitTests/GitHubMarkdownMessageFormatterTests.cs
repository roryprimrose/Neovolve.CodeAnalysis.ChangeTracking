namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class GitHubMarkdownMessageFormatterTests : Tests<GitHubMarkdownMessageFormatter>
    {
        [Fact]
        public void FormatItemChangedMessageFormatsMessageWithMarkup()
        {
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var match = new ItemMatch<IPropertyDefinition>(new TestPropertyDefinition(), new TestPropertyDefinition());
            var arguments = new FormatArguments("{DefinitionType} {Identifier} {OldValue} {NewValue}",
                oldValue, newValue);

            Service<IIdentifierFormatter>().FormatIdentifier(match.NewItem, ItemFormatType.ItemChanged).Returns(identifier);

            var actual = SUT.FormatMatch(match, ItemFormatType.ItemChanged, arguments);

            actual.Should().Be($"Property `{identifier}` `{oldValue}` `{newValue}`");
        }
    }
}