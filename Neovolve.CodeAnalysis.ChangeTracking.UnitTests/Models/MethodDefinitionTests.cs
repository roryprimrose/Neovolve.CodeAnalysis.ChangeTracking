namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class MethodDefinitionTests
    {
        [Theory]
        [InlineData(MethodDefinitionCode.ClassWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.InterfaceWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.StructWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithMethodInGenericType, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithGenericMethod, "GetValue<T>")]
        [InlineData(MethodDefinitionCode.ClassWithExplicitInterfaceMethod, "IDisposable.Dispose")]
        public async Task FullNameReturnsNameFromMethodAndDeclaringType(string code, string expected)
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.FullName.Should().Be(declaringType.FullName + "." + expected);
        }

        [Theory]
        [InlineData(MethodDefinitionCode.ClassWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.InterfaceWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.StructWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithMethodInGenericType, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithGenericMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithExplicitInterfaceMethod, "IDisposable.Dispose")]
        public async Task FullRawNameReturnsNameFromMethodAndDeclaringType(string code, string expected)
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullRawName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.FullRawName.Should().Be(declaringType.FullRawName + "." + expected);
        }

        [Theory]
        [InlineData(MethodDefinitionCode.ClassWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.InterfaceWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.StructWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithMethodInGenericType, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithGenericMethod, "GetValue<T>")]
        [InlineData(MethodDefinitionCode.ClassWithExplicitInterfaceMethod, "IDisposable.Dispose")]
        public async Task NameReturnsNameFromMethod(string code, string expected)
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.Name.Should().Be(expected);
        }

        [Theory]
        [InlineData(MethodDefinitionCode.ClassWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.InterfaceWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.StructWithMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithMethodInGenericType, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithGenericMethod, "GetValue")]
        [InlineData(MethodDefinitionCode.ClassWithExplicitInterfaceMethod, "IDisposable.Dispose")]
        public async Task RawNameReturnsNameFromMethod(string code, string expected)
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.RawName.Should().Be(expected);
        }

        [Theory]
        [InlineData(MethodDefinitionCode.ClassWithMethod, "string")]
        [InlineData(MethodDefinitionCode.InterfaceWithMethod, "string")]
        [InlineData(MethodDefinitionCode.StructWithMethod, "string")]
        [InlineData(MethodDefinitionCode.ClassWithMethodInGenericType, "T")]
        [InlineData(MethodDefinitionCode.ClassWithGenericMethod, "T")]
        [InlineData(MethodDefinitionCode.ClassWithExplicitInterfaceMethod, "void")]
        [InlineData(MethodDefinitionCode.ClassWithTaskMethod, "Task<string>")]
        [InlineData(MethodDefinitionCode.ClassWithVoidMethod, "void")]
        public async Task ReturnTypeReturnsDeclaredType(string code, string expected)
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.ReturnType.Should().Be(expected);
        }
    }
}