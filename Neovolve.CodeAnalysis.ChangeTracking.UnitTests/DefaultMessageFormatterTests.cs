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

    public class DefaultMessageFormatterTests
    {
        public static IEnumerable<object[]> KnownDefinitionTypeDataSet()
        {
            var baseType = typeof(IItemDefinition);
            var types = baseType.Assembly.GetTypes();
            var definitionTypes = types.Where(x => x.IsInterface 

                                                   // Ignore interfaces that are implemented by other definition interfaces
                                                   && x != baseType
                                                   && x != typeof(IElementDefinition)
                                                   && x != typeof(IGenericTypeElement)
                                                   && x != typeof(IMemberDefinition)
                                                   && x != typeof(ITypeDefinition)
                                                   && x != typeof(IModifiersElement<>)
                                                   && x != typeof(IAccessModifiersElement<>)
                                                   && baseType.IsAssignableFrom(x));

            return definitionTypes.Select(x => new[] {x});
        }

        [Theory]
        [InlineData(ArgumentType.Ordinal, "Ordinal argument")]
        [InlineData(ArgumentType.Named, "Named argument")]
        public void FormatItemAddedMessageDeterminesDefinitionTypeBasedOnArgumentType(ArgumentType argumentType,
            string expected)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
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
            const string? messageFormat = "{DefinitionType} {Identifier}";
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
            const string? messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, oldValue, newValue);

            var definition = Substitute.For<IClassDefinition>();

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemAddedMessage(definition, arguments!);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }

        [Theory]
        [MemberData(nameof(KnownDefinitionTypeDataSet))]
        public void FormatItemAddedMessageMapsKnownDefinitionTypes(Type definitionType)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemAddedMessage(definition, arguments!);

            actual.Should().NotStartWith("Element ");
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
            var arguments = Model.UsingModule<ConfigurationModule>().Create<FormatArguments>();

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
            const string? messageFormat = "{DefinitionType} {Identifier}";
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
            const string? messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
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
        [MemberData(nameof(KnownDefinitionTypeDataSet))]
        public void FormatItemChangedMessageMapsKnownDefinitionTypes(Type definitionType)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemRemovedMessage(definition, arguments!);

            actual.Should().NotStartWith("Element ");
        }

        [Theory]
        [InlineData(ArgumentType.Ordinal, "Ordinal argument")]
        [InlineData(ArgumentType.Named, "Named argument")]
        public void FormatItemRemovedMessageDeterminesDefinitionTypeBasedOnArgumentType(ArgumentType argumentType,
            string expected)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
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
            const string? messageFormat = "{DefinitionType} {Identifier}";
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
            const string? messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, oldValue, newValue);

            var definition = Substitute.For<IClassDefinition>();

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemRemovedMessage(definition, arguments!);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }

        [Theory]
        [MemberData(nameof(KnownDefinitionTypeDataSet))]
        public void FormatItemRemovedMessageMapsKnownDefinitionTypes(Type definitionType)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, identifier, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var sut = new DefaultMessageFormatter();

            var actual = sut.FormatItemRemovedMessage(definition, arguments!);

            actual.Should().NotStartWith("Element ");
        }
    }
}