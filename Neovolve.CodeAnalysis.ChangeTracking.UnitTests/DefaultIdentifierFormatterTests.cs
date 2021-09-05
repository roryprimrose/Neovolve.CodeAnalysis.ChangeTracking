namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultIdentifierFormatterTests
    {
        [Fact]
        public void FormatIdentifierReturnsFormattedValueForAttributeDefinition()
        {
            var attributeName = Guid.NewGuid().ToString();
            var elementFullName = Guid.NewGuid().ToString();

            var attribute = Substitute.For<IAttributeDefinition>();
            var element = Substitute.For<IElementDefinition>();

            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var sut = new DefaultIdentifierFormatter();

            var actual = sut.FormatIdentifier(attribute, ItemFormatType.None);

            actual.Should().Be(attributeName + " on " + elementFullName);
        }

        [Fact]
        public void FormatIdentifierReturnsFormattedValueForNamedArgumentDefinition()
        {
            var argumentName = Guid.NewGuid().ToString();
            var attributeName = Guid.NewGuid().ToString();
            var elementFullName = Guid.NewGuid().ToString();

            var argument = Substitute.For<IArgumentDefinition>();
            var attribute = Substitute.For<IAttributeDefinition>();
            var element = Substitute.For<IElementDefinition>();

            argument.ArgumentType.Returns(ArgumentType.Named);
            argument.ParameterName.Returns(argumentName);
            argument.DeclaringAttribute.Returns(attribute);
            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var sut = new DefaultIdentifierFormatter();

            var actual = sut.FormatIdentifier(argument, ItemFormatType.None);

            actual.Should().Be(argumentName + " in " + attributeName + " on " + elementFullName);
        }

        [Theory]
        [InlineData(null, "1")]
        [InlineData(0, "1")]
        [InlineData(1, "2")]
        [InlineData(3, "4")]
        public void FormatIdentifierReturnsFormattedValueForOrdinalArgumentDefinition(int? argumentIndex,
            string expectedPrefix)
        {
            var attributeName = Guid.NewGuid().ToString();
            var elementFullName = Guid.NewGuid().ToString();

            var argument = Substitute.For<IArgumentDefinition>();
            var attribute = Substitute.For<IAttributeDefinition>();
            var element = Substitute.For<IElementDefinition>();

            argument.ArgumentType.Returns(ArgumentType.Ordinal);
            argument.OrdinalIndex.Returns(argumentIndex);
            argument.DeclaringAttribute.Returns(attribute);
            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var sut = new DefaultIdentifierFormatter();

            var actual = sut.FormatIdentifier(argument, ItemFormatType.None);

            actual.Should().Be(expectedPrefix + " in " + attributeName + " on " + elementFullName);
        }

        [Fact]
        public void FormatIdentifierReturnsFormattedValueForParameter()
        {
            var parameterName = Guid.NewGuid().ToString();
            var parameterType = Guid.NewGuid().ToString();
            var memberFullName = Guid.NewGuid().ToString();

            var parameter = Substitute.For<IParameterDefinition>();
            var member = Substitute.For<IMemberDefinition>();

            parameter.Type.Returns(parameterType);
            parameter.Name.Returns(parameterName);
            parameter.DeclaringMember.Returns(member);
            member.FullName.Returns(memberFullName);

            var sut = new DefaultIdentifierFormatter();

            var actual = sut.FormatIdentifier(parameter, ItemFormatType.None);

            actual.Should().Be(parameterType + " " + parameterName + " in " + memberFullName);
        }

        [Fact]
        public void FormatIdentifierReturnsFullNameForElementDefinition()
        {
            var expected = Guid.NewGuid().ToString();

            var definition = Substitute.For<IElementDefinition>();

            definition.FullName.Returns(expected);

            var sut = new DefaultIdentifierFormatter();

            var actual = sut.FormatIdentifier(definition, ItemFormatType.None);

            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatIdentifierReturnsNameForItemDefinition()
        {
            var expected = Guid.NewGuid().ToString();

            var definition = Substitute.For<IItemDefinition>();

            definition.Name.Returns(expected);

            var sut = new DefaultIdentifierFormatter();

            var actual = sut.FormatIdentifier(definition, ItemFormatType.None);

            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatIdentifierThrowsExceptionWithNullDefinition()
        {
            var sut = new DefaultIdentifierFormatter();

            Action action = () => sut.FormatIdentifier(null!, ItemFormatType.ItemAdded);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}