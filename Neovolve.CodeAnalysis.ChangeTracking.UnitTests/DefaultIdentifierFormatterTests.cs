namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultIdentifierFormatterTests : Tests<DefaultIdentifierFormatter>
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
        
        [Theory]
        [InlineData(typeof(IClassDefinition), "Class")]
        [InlineData(typeof(IInterfaceDefinition), "Interface")]
        [InlineData(typeof(IEnumDefinition), "Enum")]
        [InlineData(typeof(IEnumMemberDefinition), "Enum member")]
        [InlineData(typeof(IStructDefinition), "Struct")]
        [InlineData(typeof(IConstraintListDefinition), "Generic constraint")]
        [InlineData(typeof(IFieldDefinition), "Field")]
        [InlineData(typeof(IConstructorDefinition), "Constructor")]
        [InlineData(typeof(IMethodDefinition), "Method")]
        [InlineData(typeof(IPropertyDefinition), "Property")]
        [InlineData(typeof(IPropertyAccessorDefinition), "Property accessor")]
        [InlineData(typeof(IParameterDefinition), "Parameter")]
        [InlineData(typeof(IAttributeDefinition), "Attribute")]
        [InlineData(typeof(IElementDefinition), "Element")]
        [InlineData(typeof(IItemDefinition), "Element")]
        public void FormatIdentifierDeterminesDefinitionTypeBasedOnDefinitionType(Type definitionType,
            string expected)
        {
            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var actual = SUT.FormatIdentifier(definition, ItemFormatType.ItemAdded);

            actual.Should().StartWith($"{expected}");
        }

        [Fact]
        public void FormatIdentifierDeterminesDefinitionTypeBasedOnNamedArgument()
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

            var actual = SUT.FormatIdentifier(argument, ItemFormatType.ItemAdded);

            actual.Should()
                .Be($"Named argument {argumentName} in attribute {attributeName} on interface {elementFullName}");
        }

        [Fact]
        public void FormatIdentifierDeterminesDefinitionTypeBasedOnOrdinalArgument()
        {
            var argumentName = Guid.NewGuid().ToString();
            var attributeName = Guid.NewGuid().ToString();
            var elementFullName = Guid.NewGuid().ToString();

            var argument = Substitute.For<IArgumentDefinition>();
            var attribute = Substitute.For<IAttributeDefinition>();
            var element = Substitute.For<IInterfaceDefinition>();

            argument.ArgumentType.Returns(ArgumentType.Ordinal);
            argument.DeclaringAttribute.Returns(attribute);
            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var actual = SUT.FormatIdentifier(argument, ItemFormatType.ItemAdded);

            actual.Should().Be($"Ordinal argument 1 in attribute {attributeName} on interface {elementFullName}");
        }

        [Fact]
        public void FormatIdentifierReturnsFormattedValueForAttributeDefinition()
        {
            var attributeName = Guid.NewGuid().ToString();
            var elementFullName = Guid.NewGuid().ToString();

            var attribute = Substitute.For<IAttributeDefinition>();
            var element = Substitute.For<IClassDefinition>();

            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var actual = SUT.FormatIdentifier(attribute, ItemFormatType.None);

            actual.Should().Be("Attribute " + attributeName + " on class " + elementFullName);
        }

        [Fact]
        public void FormatIdentifierReturnsFormattedValueForNamedArgumentDefinition()
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

            var actual = SUT.FormatIdentifier(argument, ItemFormatType.None);

            actual.Should().Be("Named argument " + argumentName + " in attribute " + attributeName + " on interface "
                               + elementFullName);
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
            var element = Substitute.For<IStructDefinition>();

            argument.ArgumentType.Returns(ArgumentType.Ordinal);
            argument.OrdinalIndex.Returns(argumentIndex);
            argument.DeclaringAttribute.Returns(attribute);
            attribute.Name.Returns(attributeName);
            attribute.DeclaringElement.Returns(element);
            element.FullName.Returns(elementFullName);

            var actual = SUT.FormatIdentifier(argument, ItemFormatType.None);

            actual.Should().Be("Ordinal argument " + expectedPrefix + " in attribute " + attributeName + " on struct "
                               + elementFullName);
        }

        [Fact]
        public void FormatIdentifierReturnsFormattedValueForParameter()
        {
            var parameterName = Guid.NewGuid().ToString();
            var parameterType = Guid.NewGuid().ToString();
            var memberFullName = Guid.NewGuid().ToString();

            var parameter = Substitute.For<IParameterDefinition>();
            var member = Substitute.For<IMethodDefinition>();

            parameter.Type.Returns(parameterType);
            parameter.Name.Returns(parameterName);
            parameter.DeclaringMember.Returns(member);
            member.FullName.Returns(memberFullName);

            var actual = SUT.FormatIdentifier(parameter, ItemFormatType.None);

            actual.Should().Be("Parameter " + parameterType + " " + parameterName + " in method " + memberFullName);
        }

        [Fact]
        public void FormatIdentifierReturnsFullNameForElementDefinition()
        {
            var expected = Guid.NewGuid().ToString();

            var definition = Substitute.For<IElementDefinition>();

            definition.FullName.Returns(expected);

            var actual = SUT.FormatIdentifier(definition, ItemFormatType.None);

            actual.Should().Be("Element " + expected);
        }

        [Fact]
        public void FormatIdentifierReturnsNameForItemDefinition()
        {
            var expected = Guid.NewGuid().ToString();

            var definition = Substitute.For<IItemDefinition>();

            definition.Name.Returns(expected);

            var actual = SUT.FormatIdentifier(definition, ItemFormatType.None);

            actual.Should().Be("Element " + expected);
        }

        [Fact]
        public void FormatIdentifierThrowsExceptionWithNullDefinition()
        {
            Action action = () => SUT.FormatIdentifier(null!, ItemFormatType.ItemAdded);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(KnownDefinitionTypeDataSet))]
        public void FormatItemMapsKnownDefinitionTypes(Type definitionType)
        {
            var definition = (IItemDefinition) Substitute.For(new[] {definitionType}, Array.Empty<object>());

            var actual = SUT.FormatIdentifier(definition, ItemFormatType.ItemAdded);

            actual.Should().NotStartWith("Element ");
        }
    }
}