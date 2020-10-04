namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class MethodDefinitionTests
    {
        [Fact]
        public async Task AttributesReturnsDeclaredAttributes()
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(MethodDefinitionCode.MethodWithAttributes)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.Attributes.Should().HaveCount(2);
            sut.Attributes.First().Name.Should().Be("Stuff");
            sut.Attributes.First().Arguments.Should().BeEmpty();
            sut.Attributes.Skip(1).First().Name.Should().Be("Here");
            sut.Attributes.Skip(1).First().Arguments.Should().HaveCount(4);
        }

        [Fact]
        public async Task AttributesReturnsEmptyWhenNonDeclared()
        {
            var parentFullName = Guid.NewGuid().ToString();

            var declaringType = Substitute.For<IClassDefinition>();

            declaringType.FullName.Returns(parentFullName);

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(MethodDefinitionCode.ClassWithMethod)
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.Attributes.Should().BeEmpty();
        }

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
        [InlineData("", MethodModifiers.None)]
        [InlineData("new", MethodModifiers.New)]
        [InlineData("new static", MethodModifiers.NewStatic)]
        [InlineData("static new", MethodModifiers.NewStatic)]
        [InlineData("new virtual", MethodModifiers.NewVirtual)]
        [InlineData("virtual new", MethodModifiers.NewVirtual)]
        [InlineData("new abstract", MethodModifiers.NewAbstract)]
        [InlineData("abstract new", MethodModifiers.NewAbstract)]
        [InlineData("new abstract virtual", MethodModifiers.NewAbstractVirtual)]
        [InlineData("abstract new virtual", MethodModifiers.NewAbstractVirtual)]
        [InlineData("abstract virtual new", MethodModifiers.NewAbstractVirtual)]
        [InlineData("virtual abstract new", MethodModifiers.NewAbstractVirtual)]
        [InlineData("virtual new abstract", MethodModifiers.NewAbstractVirtual)]
        [InlineData("new virtual abstract", MethodModifiers.NewAbstractVirtual)]
        [InlineData("abstract", MethodModifiers.Abstract)]
        [InlineData("abstract override", MethodModifiers.AbstractOverride)]
        [InlineData("override abstract", MethodModifiers.AbstractOverride)]
        [InlineData("override", MethodModifiers.Override)]
        [InlineData("override sealed", MethodModifiers.SealedOverride)]
        [InlineData("sealed override", MethodModifiers.SealedOverride)]
        [InlineData("sealed", MethodModifiers.Sealed)]
        [InlineData("static", MethodModifiers.Static)]
        [InlineData("virtual", MethodModifiers.Virtual)]
        [InlineData("async new", MethodModifiers.AsyncNew)]
        [InlineData("async new static", MethodModifiers.AsyncNewStatic)]
        [InlineData("async static new", MethodModifiers.AsyncNewStatic)]
        [InlineData("async new virtual", MethodModifiers.AsyncNewVirtual)]
        [InlineData("async virtual new", MethodModifiers.AsyncNewVirtual)]
        [InlineData("async new abstract", MethodModifiers.AsyncNewAbstract)]
        [InlineData("async abstract new", MethodModifiers.AsyncNewAbstract)]
        [InlineData("async new abstract virtual", MethodModifiers.AsyncNewAbstractVirtual)]
        [InlineData("async abstract new virtual", MethodModifiers.AsyncNewAbstractVirtual)]
        [InlineData("async abstract virtual new", MethodModifiers.AsyncNewAbstractVirtual)]
        [InlineData("async virtual abstract new", MethodModifiers.AsyncNewAbstractVirtual)]
        [InlineData("async virtual new abstract", MethodModifiers.AsyncNewAbstractVirtual)]
        [InlineData("async new virtual abstract", MethodModifiers.AsyncNewAbstractVirtual)]
        [InlineData("async abstract", MethodModifiers.AsyncAbstract)]
        [InlineData("async abstract override", MethodModifiers.AsyncAbstractOverride)]
        [InlineData("async override abstract", MethodModifiers.AsyncAbstractOverride)]
        [InlineData("async override", MethodModifiers.AsyncOverride)]
        [InlineData("async override sealed", MethodModifiers.AsyncSealedOverride)]
        [InlineData("async sealed override", MethodModifiers.AsyncSealedOverride)]
        [InlineData("async sealed", MethodModifiers.AsyncSealed)]
        [InlineData("async static", MethodModifiers.AsyncStatic)]
        [InlineData("async virtual", MethodModifiers.AsyncVirtual)]
        public async Task ModifiersReturnsExpectedValue(string modifiers, MethodModifiers expected)
        {
            var declaringType = Substitute.For<IClassDefinition>();

            var node = await TestNode
                .FindNode<MethodDeclarationSyntax>(
                    MethodDefinitionCode.ClassWithMethod.Replace("public string", "public " + modifiers + " string"))
                .ConfigureAwait(false);

            var sut = new MethodDefinition(declaringType, node);

            sut.Modifiers.Should().Be(expected);
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