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
        [Theory]
        [InlineData("Identifier", "Identifier")]
        [InlineData("identifier", "Identifier")]
        public void FormatItemChangedMessageFormatsMessageWithMarkup(string prefix, string expectedPrefix)
        {
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var match = new ItemMatch<IPropertyDefinition>(new TestPropertyDefinition(), new TestPropertyDefinition());
            var arguments = new FormatArguments("{Identifier} {OldValue} {NewValue}",
                oldValue, newValue);

            Service<IIdentifierFormatter>().FormatIdentifier(match.NewItem, ItemFormatType.ItemChanged).Returns(prefix + " " + identifier);

            var actual = SUT.FormatMatch(match, ItemFormatType.ItemChanged, arguments);

            actual.Should().Be($"{expectedPrefix} {identifier} `{oldValue}` `{newValue}`");
        }
    }
}