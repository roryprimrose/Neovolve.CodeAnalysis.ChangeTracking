namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class BaseTypeDefinitionTests
    {
        [Fact]
        public async Task DeclaringTypeReturnsDeclaringClass()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.DeclaringType.Should().BeNull();

            var parentClass = sut.ChildClasses.Single();

            parentClass.DeclaringType.Should().Be(sut);

            var childClass = parentClass.ChildClasses.Single();

            childClass.DeclaringType.Should().Be(parentClass);
        }

        [Fact]
        public async Task DeclaringTypeReturnsDeclaringStruct()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInGrandparentStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.DeclaringType.Should().BeNull();

            var parentStruct = sut.ChildStructs.Single();

            parentStruct.DeclaringType.Should().Be(sut);

            var childStruct = parentStruct.ChildStructs.Single();

            childStruct.DeclaringType.Should().Be(parentStruct);
        }

        [Fact]
        public async Task DeclaringTypeReturnsNullWhenNoDeclaringTypeFound()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.DeclaringType.Should().BeNull();
        }

        [Fact]
        public async Task FullNameReturnsNameFromClassCombinedWithDeclaringTypeFullName()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.FullName.Should().Be("MyNamespace.MyGrandparentClass");

            var parentClass = sut.ChildClasses.Single();

            parentClass.FullName.Should().Be("MyNamespace.MyGrandparentClass+MyParentClass");

            var childClass = parentClass.ChildClasses.Single();

            childClass.FullName.Should().Be("MyNamespace.MyGrandparentClass+MyParentClass+MyClass");
        }

        [Fact]
        public async Task FullNameReturnsNameFromClassCombinedWithDeclaringTypeFullNameAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.FullName.Should().Be("MyNamespace.MyClass<TKey, TValue>");

            var child = sut.ChildClasses.Single();

            child.FullName.Should().Be("MyNamespace.MyClass<TKey, TValue>+MyChildClass");
        }

        [Fact]
        public async Task FullNameReturnsNameFromInterfaceCombinedWithDeclaringTypeFullNameAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.FullName.Should().Be("MyNamespace.MyInterface<TKey, TValue>");

            var child = sut.ChildInterfaces.Single();

            child.FullName.Should().Be("MyNamespace.MyInterface<TKey, TValue>+MyChildInterface");
        }

        [Fact]
        public async Task FullNameReturnsNameFromStructCombinedWithDeclaringTypeFullName()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInGrandparentStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.FullName.Should().Be("MyNamespace.MyGrandparentStruct");

            var parentStruct = sut.ChildStructs.Single();

            parentStruct.FullName.Should().Be("MyNamespace.MyGrandparentStruct+MyParentStruct");

            var childStruct = parentStruct.ChildStructs.Single();

            childStruct.FullName.Should().Be("MyNamespace.MyGrandparentStruct+MyParentStruct+MyStruct");
        }

        [Fact]
        public async Task FullNameReturnsNameFromStructCombinedWithDeclaringTypeFullNameAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.FullName.Should().Be("MyNamespace.MyStruct<TKey, TValue>");

            var child = sut.ChildStructs.Single();

            child.FullName.Should().Be("MyNamespace.MyStruct<TKey, TValue>+MyChildStruct");
        }

        [Fact]
        public async Task FullRawNameReturnsNameFromClassCombinedWithDeclaringTypeFullRawName()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.FullRawName.Should().Be("MyNamespace.MyGrandparentClass");

            var parentClass = sut.ChildClasses.Single();

            parentClass.FullRawName.Should().Be("MyNamespace.MyGrandparentClass+MyParentClass");

            var childClass = parentClass.ChildClasses.Single();

            childClass.FullRawName.Should().Be("MyNamespace.MyGrandparentClass+MyParentClass+MyClass");
        }

        [Fact]
        public async Task FullRawNameReturnsNameFromClassCombinedWithDeclaringTypeFullRawNameAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.FullRawName.Should().Be("MyNamespace.MyClass");

            var child = sut.ChildClasses.Single();

            child.FullRawName.Should().Be("MyNamespace.MyClass+MyChildClass");
        }

        [Fact]
        public async Task
            FullRawNameReturnsNameFromInterfaceCombinedWithDeclaringTypeFullRawNameAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.FullRawName.Should().Be("MyNamespace.MyInterface");

            var child = sut.ChildInterfaces.Single();

            child.FullRawName.Should().Be("MyNamespace.MyInterface+MyChildInterface");
        }

        [Fact]
        public async Task FullRawNameReturnsNameFromStructCombinedWithDeclaringTypeFullRawName()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInGrandparentStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.FullRawName.Should().Be("MyNamespace.MyGrandparentStruct");

            var parentStruct = sut.ChildStructs.Single();

            parentStruct.FullRawName.Should().Be("MyNamespace.MyGrandparentStruct+MyParentStruct");

            var childStruct = parentStruct.ChildStructs.Single();

            childStruct.FullRawName.Should().Be("MyNamespace.MyGrandparentStruct+MyParentStruct+MyStruct");
        }

        [Fact]
        public async Task FullRawNameReturnsNameFromStructCombinedWithDeclaringTypeFullRawNameAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.FullRawName.Should().Be("MyNamespace.MyStruct");

            var child = sut.ChildStructs.Single();

            child.FullRawName.Should().Be("MyNamespace.MyStruct+MyChildStruct");
        }

        [Fact]
        public async Task ImplementedTypesReturnsEmptyWhenNoImplementedTypesDeclared()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ImplementedTypes.Should().BeEmpty();
        }

        [Fact]
        public async Task ImplementedTypesReturnsMultipleImplementedType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassImplementsMultipleTypes)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ImplementedTypes.Should().HaveCount(2);
            sut.ImplementedTypes.First().Should().Be("MyBase");
            sut.ImplementedTypes.Skip(1).First().Should().Be("IEnumerable<string>");
        }

        [Fact]
        public async Task ImplementedTypesReturnsMultipleImplementedTypeWithGenericTypeDefinition()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceImplementsMultipleTypes)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.ImplementedTypes.Should().HaveCount(2);
            sut.ImplementedTypes.First().Should().Be("IDisposable");
            sut.ImplementedTypes.Skip(1).First().Should().Be("IEnumerable<T>");
        }

        [Fact]
        public async Task ImplementedTypesReturnsSingleImplementedType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassImplementsSingleType)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ImplementedTypes.Should().HaveCount(1);
            sut.ImplementedTypes.First().Should().Be("MyBase");
        }

        [Fact]
        public async Task IsVisibleReturnsFalseWhenParentTypeIsNotVisible()
        {
            var code = TypeDefinitionCode.StructInParentStruct.Replace("public struct MyParentStruct",
                "private struct MyParentStruct");

            var node = await TestNode.FindNode<StructDeclarationSyntax>(code).ConfigureAwait(false);

            var parent = new StructDefinition(node);
            var child = parent.ChildStructs.Single();

            parent.IsVisible.Should().BeFalse();
            child.IsVisible.Should().BeFalse();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", true)]
        [InlineData("private protected", true)]
        [InlineData("protected internal", true)]
        [InlineData("public", true)]
        public async Task IsVisibleReturnsValueBasedOnAccessModifiers(string accessModifiers, bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task NameReturnsMultipleGenericTypeDefinitions()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericTypes)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Name.Should().Be("IMyInterface<T, V>");
        }

        [Fact]
        public async Task NameReturnsNameFromClass()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyGrandparentClass");

            var parentClass = sut.ChildClasses.Single();

            parentClass.Name.Should().Be("MyParentClass");

            var childClass = parentClass.ChildClasses.Single();

            childClass.Name.Should().Be("MyClass");
        }

        [Fact]
        public async Task NameReturnsNameFromClassWithGenericTypeParameters()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithGenericType)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyClass<T>");
        }

        [Fact]
        public async Task NameReturnsNameFromClassWithParentsAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyClass<TKey, TValue>");

            var childClass = sut.ChildClasses.Single();

            childClass.Name.Should().Be("MyChildClass");
        }

        [Fact]
        public async Task NameReturnsNameFromInterfaceWithGenericTypeParameters()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithGenericType)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Name.Should().Be("IMyInterface<T>");
        }

        [Fact]
        public async Task NameReturnsNameFromInterfaceWithoutDeclaringType()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithoutParent)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Name.Should().Be("MyInterface");
        }

        [Fact]
        public async Task NameReturnsNameFromInterfaceWithParentsAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Name.Should().Be("MyInterface<TKey, TValue>");

            var child = sut.ChildInterfaces.Single();

            child.Name.Should().Be("MyChildInterface");
        }

        [Fact]
        public async Task NameReturnsNameFromStruct()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInGrandparentStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Name.Should().Be("MyGrandparentStruct");

            var parentStruct = sut.ChildStructs.Single();

            parentStruct.Name.Should().Be("MyParentStruct");

            var childStruct = parentStruct.ChildStructs.Single();

            childStruct.Name.Should().Be("MyStruct");
        }

        [Fact]
        public async Task NameReturnsNameFromStructWithGenericTypeParameters()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithGenericType)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Name.Should().Be("MyStruct<T>");
        }

        [Fact]
        public async Task NameReturnsNameFromStructWithParentsAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Name.Should().Be("MyStruct<TKey, TValue>");

            var childStruct = sut.ChildStructs.Single();

            childStruct.Name.Should().Be("MyChildStruct");
        }

        [Fact]
        public async Task NamespaceReturnsDeclarationWhenInComplexNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithComplexNamespace)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Namespace.Should().Be("MyNamespace.OtherNamespace.FinalNamespace");
        }

        [Fact]
        public async Task NamespaceReturnsDeclarationWhenInNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Namespace.Should().Be("MyNamespace");
        }

        [Fact]
        public async Task NamespaceReturnsDeclarationWhenInNestedNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithNestedNamespace)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Namespace.Should().Be("MyNamespace.ChildNamespace");
        }

        [Fact]
        public async Task NamespaceReturnsEmptyWhenNotInNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutNamespace)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Namespace.Should().BeEmpty();
        }

        [Fact]
        public async Task NamespaceReturnsSameValueOnChildClasses()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Namespace.Should().Be("MyNamespace");

            var parentClass = sut.ChildClasses.Single();

            parentClass.Namespace.Should().Be("MyNamespace");

            var childClass = parentClass.ChildClasses.Single();

            childClass.Namespace.Should().Be("MyNamespace");
        }

        [Fact]
        public async Task RawNameReturnsNameFromClass()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.RawName.Should().Be("MyGrandparentClass");

            var parentClass = sut.ChildClasses.Single();

            parentClass.RawName.Should().Be("MyParentClass");

            var childClass = parentClass.ChildClasses.Single();

            childClass.RawName.Should().Be("MyClass");
        }

        [Fact]
        public async Task RawNameReturnsNameFromClassWithoutGenericTypeParameters()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithGenericType)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.RawName.Should().Be("MyClass");
        }

        [Fact]
        public async Task RawNameReturnsNameFromClassWithParentsAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.RawName.Should().Be("MyClass");

            var childClass = sut.ChildClasses.Single();

            childClass.RawName.Should().Be("MyChildClass");
        }

        [Fact]
        public async Task RawNameReturnsNameFromInterfaceWithoutDeclaringType()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithoutParent)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.RawName.Should().Be("MyInterface");
        }

        [Fact]
        public async Task RawNameReturnsNameFromInterfaceWithoutGenericTypeParameters()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithGenericType)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.RawName.Should().Be("IMyInterface");
        }

        [Fact]
        public async Task RawNameReturnsNameFromInterfaceWithParentsAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.RawName.Should().Be("MyInterface");

            var child = sut.ChildInterfaces.Single();

            child.RawName.Should().Be("MyChildInterface");
        }

        [Fact]
        public async Task RawNameReturnsNameFromStruct()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInGrandparentStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.RawName.Should().Be("MyGrandparentStruct");

            var parentStruct = sut.ChildStructs.Single();

            parentStruct.RawName.Should().Be("MyParentStruct");

            var childStruct = parentStruct.ChildStructs.Single();

            childStruct.RawName.Should().Be("MyStruct");
        }

        [Fact]
        public async Task RawNameReturnsNameFromStructWithoutGenericTypeParameters()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithGenericType)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.RawName.Should().Be("MyStruct");
        }

        [Fact]
        public async Task RawNameReturnsNameFromStructWithParentsAndGenericTypeParameters()
        {
            var node = await TestNode
                .FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.RawName.Should().Be("MyStruct");

            var childStruct = sut.ChildStructs.Single();

            childStruct.RawName.Should().Be("MyChildStruct");
        }
    }
}