namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class MemberDefinitionTests
    {
        [Fact]
        public async Task AccessModifierReturnsPrivateWhenEmptyModifierDefinedWithClassParent()
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifier.Should().Be(AccessModifier.Private);
        }

        [Fact]
        public async Task AccessModifierReturnsPublicWhenEmptyModifierDefinedWithInterfaceParent()
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(string.Empty);

            var declaringType = Substitute.For<IInterfaceDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifier.Should().Be(AccessModifier.Public);
        }

        [Theory]
        [InlineData("private", AccessModifier.Private)]
        [InlineData("internal", AccessModifier.Internal)]
        [InlineData("protected", AccessModifier.Protected)]
        [InlineData("private protected", AccessModifier.ProtectedPrivate)]
        [InlineData("protected private", AccessModifier.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifier.ProtectedInternal)]
        [InlineData("internal protected", AccessModifier.ProtectedInternal)]
        [InlineData("public", AccessModifier.Public)]
        public async Task AccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            AccessModifier expected)
        {
            var code = FieldDefinitionCode.BuildFieldWithModifiers(accessModifiers);

            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.AccessModifier.Should().Be(expected);
        }

        [Fact]
        public async Task DeclaringTypeReturnsParameterValue()
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(FieldDefinitionCode.SingleField)
                .ConfigureAwait(false);

            var sut = new Wrapper(declaringType, node);

            sut.DeclaringType.Should().Be(declaringType);
        }

        private class Wrapper : MemberDefinition
        {
            public Wrapper(ITypeDefinition declaringType, MemberDeclarationSyntax node) : base(node, declaringType)
            {
            }

            public override string FullName { get; } = string.Empty;
            public override string FullRawName { get; } = string.Empty;
            public override string Name { get; } = string.Empty;
            public override string RawName { get; } = string.Empty;
            public override string ReturnType { get; } = string.Empty;
        }
    }
}