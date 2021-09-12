namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class GitHubMarkdownIdentifierFormatterTests
    {
        [Fact]
        public void FormatItemQuotesAllIdentifierParts()
        {
            var argumentName = Guid.NewGuid().ToString();
            var attributeName = Guid.NewGuid().ToString();
            var elementFullName = Guid.NewGuid().ToString();

            var argument = Substitute.For<IArgumentDefinition>();
            var attribute = Substitute.For<IAttributeDefinition>();
            var element = Substitute.For<IInterfaceDefinition>();

            argument.ArgumentType.Returns(ArgumentType.Named);
            argument.ParameterName.Returns(argumentName);
            argument.DeclaringAttribute.Returns(attribute);
            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var sut = new GitHubMarkdownIdentifierFormatter();

            var actual = sut.FormatIdentifier(argument, ItemFormatType.None);

            actual.Should().Be($"Named argument `{argumentName}` in attribute `{attributeName}` on interface `{elementFullName}`");
        }
    }
}