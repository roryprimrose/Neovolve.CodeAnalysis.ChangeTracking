namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultMessageFormatterTests
    {
        [Theory]
        [InlineData(ArgumentType.Ordinal, "Ordinal argument")]
        [InlineData(ArgumentType.Named, "Named argument")]
        public void FormatItemAddedMessageDeterminesDefinitionTypeBasedOnArgumentType(ArgumentType argumentType,
            string expected)
        {
            var messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = Substitute.For<IArgumentDefinition>();

            definition.ArgumentType.Returns(argumentType);

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemAddedMessage(definition, arguments!);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Theory]
        [InlineData(typeof(IClassDefinition), "Class")]
        [InlineData(typeof(IInterfaceDefinition), "Interface")]
        [InlineData(typeof(IConstraintListDefinition), "Generic constraint")]
        [InlineData(typeof(IPropertyDefinition), "Property")]
        [InlineData(typeof(IPropertyAccessorDefinition), "Property accessor")]
        [InlineData(typeof(IFieldDefinition), "Field")]
        [InlineData(typeof(IAttributeDefinition), "Attribute")]
        [InlineData(typeof(IItemDefinition), "Element")]
        public void FormatItemAddedMessageDeterminesDefinitionTypeBasedOnDefinitionType(Type definitionType,
            string expected)
        {
            var messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemAddedMessage(definition, arguments!);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Fact]
        public void FormatItemAddedMessageFormatsMessageWithProvidedArguments()
        {
            var messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, oldValue, newValue);

            var definition = Substitute.For<IClassDefinition>();

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemAddedMessage(definition, arguments!);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }

        [Fact]
        public void FormatItemAddedMessageThrowsExceptionWithNullArguments()
        {
            var definition = Substitute.For<IItemDefinition>();

            var sut = new DefaultMessageFormatter();

            Action action = () => sut.FormatItemAddedMessage(definition, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FormatItemAddedMessageThrowsExceptionWithNullDefinition()
        {
            var arguments = Model.Create<FormatArguments>();

            var sut = new DefaultMessageFormatter();

            Action action = () => sut.FormatItemAddedMessage(null!, arguments);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(ArgumentType.Ordinal, "Ordinal argument")]
        [InlineData(ArgumentType.Named, "Named argument")]
        public void FormatItemChangedMessageDeterminesDefinitionTypeBasedOnArgumentType(ArgumentType argumentType,
            string expected)
        {
            var messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var firstDefinition = Substitute.For<IArgumentDefinition>();
            var secondDefinition = Substitute.For<IArgumentDefinition>();
            var match = new ItemMatch<IArgumentDefinition>(firstDefinition, secondDefinition);

            firstDefinition.ArgumentType.Returns(argumentType);
            secondDefinition.ArgumentType.Returns(argumentType);

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemChangedMessage(match, arguments!);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Fact]
        public void FormatItemChangedMessageFormatsMessageWithProvidedArguments()
        {
            var messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, oldValue, newValue);

            var firstDefinition = Substitute.For<IClassDefinition>();
            var secondDefinition = Substitute.For<IClassDefinition>();
            var match = new ItemMatch<IClassDefinition>(firstDefinition, secondDefinition);

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemChangedMessage(match, arguments!);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }

        [Theory]
        [InlineData(ArgumentType.Ordinal, "Ordinal argument")]
        [InlineData(ArgumentType.Named, "Named argument")]
        public void FormatItemRemovedMessageDeterminesDefinitionTypeBasedOnArgumentType(ArgumentType argumentType,
            string expected)
        {
            var messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = Substitute.For<IArgumentDefinition>();

            definition.ArgumentType.Returns(argumentType);

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemRemovedMessage(definition, arguments!);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Theory]
        [InlineData(typeof(IClassDefinition), "Class")]
        [InlineData(typeof(IInterfaceDefinition), "Interface")]
        [InlineData(typeof(IConstraintListDefinition), "Generic constraint")]
        [InlineData(typeof(IPropertyDefinition), "Property")]
        [InlineData(typeof(IPropertyAccessorDefinition), "Property accessor")]
        [InlineData(typeof(IFieldDefinition), "Field")]
        [InlineData(typeof(IAttributeDefinition), "Attribute")]
        [InlineData(typeof(IItemDefinition), "Element")]
        public void FormatItemRemovedMessageDeterminesDefinitionTypeBasedOnDefinitionType(Type definitionType,
            string expected)
        {
            var messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemRemovedMessage(definition, arguments!);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Fact]
        public void FormatItemRemovedMessageFormatsMessageWithProvidedArguments()
        {
            var messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, oldValue, newValue);

            var definition = Substitute.For<IClassDefinition>();

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemRemovedMessage(definition, arguments!);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }
    }
}