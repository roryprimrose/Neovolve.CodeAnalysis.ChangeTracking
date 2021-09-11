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
                                                   && x != typeof(IBaseTypeDefinition)
                                                   && x != typeof(IBaseTypeDefinition<>)
                                                   && x != typeof(ITypeDefinition)
                                                   && x != typeof(IModifiersElement<>)
                                                   && x != typeof(IAccessModifiersElement<>)
                                                   && baseType.IsAssignableFrom(x));

            return definitionTypes.Select(x => new[] {x});
        }

        [Fact]
        public void FormatItemChangedMessageFormatsMessageWithProvidedArguments()
        {
            const string? messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, oldValue, newValue);

            var firstDefinition = Substitute.For<IClassDefinition>();
            var secondDefinition = Substitute.For<IClassDefinition>();
            var match = new ItemMatch<IClassDefinition>(firstDefinition, secondDefinition);

            Service<IIdentifierFormatter>().FormatIdentifier(match.NewItem, ItemFormatType.ItemChanged)
                .Returns(identifier);

            var actual = SUT.FormatMatch(match, ItemFormatType.ItemChanged, arguments);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }

        [Theory]
        [InlineData(ArgumentType.Ordinal, "Ordinal argument")]
        [InlineData(ArgumentType.Named, "Named argument")]
        public void FormatItemDeterminesDefinitionTypeBasedOnArgumentType(ArgumentType argumentType,
            string expected)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, null, null);

            var definition = Substitute.For<IArgumentDefinition>();

            definition.ArgumentType.Returns(argumentType);
            Service<IIdentifierFormatter>().FormatIdentifier(definition, ItemFormatType.ItemAdded).Returns(identifier);

            var actual = SUT.FormatItem(definition, ItemFormatType.ItemAdded, arguments);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Theory]
        [InlineData(typeof(IClassDefinition), "Class")]
        [InlineData(typeof(IInterfaceDefinition), "Interface")]
        [InlineData(typeof(IEnumDefinition), "Enum")]
        [InlineData(typeof(IEnumMemberDefinition), "Enum Member")]
        [InlineData(typeof(IStructDefinition), "Struct")]
        [InlineData(typeof(IConstraintListDefinition), "Generic constraint")]
        [InlineData(typeof(IFieldDefinition), "Field")]
        [InlineData(typeof(IConstructorDefinition), "Constructor")]
        [InlineData(typeof(IMethodDefinition), "Method")]
        [InlineData(typeof(IPropertyDefinition), "Property")]
        [InlineData(typeof(IPropertyAccessorDefinition), "Property accessor")]
        [InlineData(typeof(IParameterDefinition), "Parameter")]
        [InlineData(typeof(IAttributeDefinition), "Attribute")]
        [InlineData(typeof(IItemDefinition), "Element")]
        public void FormatItemDeterminesDefinitionTypeBasedOnDefinitionType(Type definitionType,
            string expected)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            Service<IIdentifierFormatter>().FormatIdentifier(definition, ItemFormatType.ItemAdded).Returns(identifier);

            var actual = SUT.FormatItem(definition, ItemFormatType.ItemAdded, arguments);

            actual.Should().Be($"{expected} {identifier}");
        }

        [Fact]
        public void FormatItemFormatsMessageWithProvidedArguments()
        {
            const string? messageFormat = "{DefinitionType} - {Identifier} - {OldValue} - {NewValue}";
            var identifier = Guid.NewGuid().ToString();
            var oldValue = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, oldValue, newValue);

            var definition = Substitute.For<IClassDefinition>();

            Service<IIdentifierFormatter>().FormatIdentifier(definition, ItemFormatType.ItemAdded).Returns(identifier);

            var actual = SUT.FormatItem(definition, ItemFormatType.ItemAdded, arguments);

            actual.Should().Be($"Class - {identifier} - {oldValue} - {newValue}");
        }

        [Theory]
        [MemberData(nameof(KnownDefinitionTypeDataSet))]
        public void FormatItemMapsKnownDefinitionTypes(Type definitionType)
        {
            const string? messageFormat = "{DefinitionType} {Identifier}";
            var identifier = Guid.NewGuid().ToString();
            var arguments = new FormatArguments(messageFormat, null, null);

            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            Service<IIdentifierFormatter>().FormatIdentifier(definition, ItemFormatType.ItemAdded).Returns(identifier);

            var actual = SUT.FormatItem(definition, ItemFormatType.ItemAdded, arguments);

            actual.Should().NotStartWith("Element ");
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