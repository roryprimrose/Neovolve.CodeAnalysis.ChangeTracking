﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class TypeDefinitionTests
    {
        [Fact]
        public async Task AttributesReturnsEmptyWhenNoAttributesDeclared()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Attributes.Should().BeEmpty();
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributes()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributes)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Attributes.Should().HaveCount(2);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnMultipleLists()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInMultipleLists)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Attributes.Should().HaveCount(4);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
            sut.Attributes.Skip(2).First().Name.Should().Be("Third");
            sut.Attributes.Skip(3).First().Name.Should().Be("Fourth");
        }

        [Fact]
        public async Task AttributesReturnsMultipleAttributesOnSingleList()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleAttributesInSingleList)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Attributes.Should().HaveCount(2);

            sut.Attributes.First().Name.Should().Be("First");
            sut.Attributes.Skip(1).First().Name.Should().Be("Second");
        }

        [Fact]
        public async Task AttributesReturnsSingleAttribute()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithSingleAttribute)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Attributes.Should().HaveCount(1);

            sut.Attributes.First().Name.Should().Be("MyAttribute");
        }

        [Fact]
        public async Task ChildClassesIncludesHierarchyOfClassesAndInterfaces()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInParentClassAndInterface)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyGrandparentClass");
            sut.FullName.Should().Be("MyNamespace.MyGrandparentClass");
            sut.DeclaringType.Should().BeNull();

            var myInterface = sut.ChildInterfaces.Single();

            myInterface.Name.Should().Be("IMyInterface");
            myInterface.FullName.Should().Be("MyNamespace.MyGrandparentClass+IMyInterface");
            myInterface.DeclaringType.Should().Be(sut);

            var parent = myInterface.ChildClasses.Single();

            parent.Name.Should().Be("MyParentClass");
            parent.FullName.Should().Be("MyNamespace.MyGrandparentClass+IMyInterface+MyParentClass");
            parent.DeclaringType.Should().Be(myInterface);

            var myClass = parent.ChildClasses.Single();

            myClass.Name.Should().Be("MyClass");
            myClass.FullName.Should().Be("MyNamespace.MyGrandparentClass+IMyInterface+MyParentClass+MyClass");
            myClass.DeclaringType.Should().Be(parent);

            var myChildInterface = myClass.ChildInterfaces.Single();

            myChildInterface.Name.Should().Be("IChildInterface");
            myChildInterface.FullName.Should().Be(
                "MyNamespace.MyGrandparentClass+IMyInterface+MyParentClass+MyClass+IChildInterface");
            myChildInterface.DeclaringType.Should().Be(myClass);
        }

        [Fact]
        public async Task ChildClassesReturnsEmptyWhenNoDeclarationsFound()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildClasses.Should().BeEmpty();
        }

        [Fact]
        public async Task ChildClassesReturnsMultipleClassDefinitions()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.MultipleChildClasses)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildClasses.Should().HaveCount(2);

            var firstClass = sut.ChildClasses.First();
            var secondClass = sut.ChildClasses.Skip(1).First();

            firstClass.Name.Should().Be("FirstChild");
            firstClass.FullName.Should().Be("MyNamespace.MyClass+FirstChild");
            firstClass.DeclaringType.Should().Be(sut);

            secondClass.Name.Should().Be("SecondChild");
            secondClass.FullName.Should().Be("MyNamespace.MyClass+SecondChild");
            secondClass.DeclaringType.Should().Be(sut);
        }

        [Fact]
        public async Task ChildClassesReturnsSingleClassDefinition()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInParentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyParentClass");
            sut.FullName.Should().Be("MyNamespace.MyParentClass");
            sut.ChildClasses.Should().HaveCount(1);

            var childClass = sut.ChildClasses.First();

            childClass.Name.Should().Be("MyClass");
            childClass.FullName.Should().Be("MyNamespace.MyParentClass+MyClass");
            childClass.DeclaringType.Should().Be(sut);
        }

        [Fact]
        public async Task ChildInterfacesReturnsMultipleInterfaceDefinitions()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.MultipleChildInterfaces)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildInterfaces.Should().HaveCount(2);

            var firstInterface = sut.ChildInterfaces.First();
            var secondInterface = sut.ChildInterfaces.Skip(1).First();

            firstInterface.Name.Should().Be("FirstChild");
            firstInterface.FullName.Should().Be("MyNamespace.MyClass+FirstChild");
            firstInterface.DeclaringType.Should().Be(sut);

            secondInterface.Name.Should().Be("SecondChild");
            secondInterface.FullName.Should().Be("MyNamespace.MyClass+SecondChild");
            secondInterface.DeclaringType.Should().Be(sut);
        }

        [Fact]
        public async Task ChildTypesReturnsMultipleTypeDefinitions()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.MultipleChildTypes)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildTypes.Should().HaveCount(4);

            sut.ChildTypes.OfType<ClassDefinition>().Should().HaveCount(2);
            sut.ChildTypes.OfType<InterfaceDefinition>().Should().HaveCount(2);
        }

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
        public async Task DeclaringTypeReturnsNullWhenNoDeclaringTypeFound()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.DeclaringType.Should().BeNull();
        }

        [Fact]
        public async Task FullNameReturnsNameCombinedWithDeclaringTypeFullName()
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
        public async Task GenericTypeParametersReturnsEmptyWhenNotGenericType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.GenericTypeParameters.Should().BeEmpty();
        }

        [Fact]
        public async Task GenericTypeParametersReturnsNameOfSingleGenericType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithGenericType)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.GenericTypeParameters.Should().HaveCount(1);
            sut.GenericTypeParameters.Single().Should().Be("T");
        }

        [Fact]
        public async Task GenericTypeParametersReturnsNamesOfMultipleGenericTypes()
        {
            var node = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMultipleGenericTypes)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.GenericTypeParameters.Should().HaveCount(2);
            sut.GenericTypeParameters.First().Should().Be("T");
            sut.GenericTypeParameters.Skip(1).First().Should().Be("V");
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

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", true)]
        [InlineData("private protected", true)]
        [InlineData("protected internal", true)]
        [InlineData("public", true)]
        public async Task IsVisibleReturnsValueBasedOnScope(string scope, bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(scope);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task LocationReturnsEmptyFilePathWhenNodeLacksSourceInformation()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Location.FilePath.Should().BeEmpty();
        }

        [Fact]
        public async Task LocationReturnsFileContentLocation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(4);
        }

        [Fact]
        public async Task LocationReturnsFilePathWhenNodeIncludesSourceInformation()
        {
            var filePath = Guid.NewGuid().ToString();

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent, filePath)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Location.FilePath.Should().Be(filePath);
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
        public async Task NameReturnsNameFromClassWithGenericType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithGenericType)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyClass<T>");
        }

        [Fact]
        public async Task NameReturnsNameFromInterfaceWithGenericType()
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
        public async Task NamespaceReturnsDeclarationWhenInNamespace()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Namespace.Should().Be("MyNamespace");
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
        public async Task PropertiesReturnsDeclaredPropertiesOnClass()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithProperties)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Properties.Should().HaveCount(2);

            var first = sut.Properties.First();

            first.Name.Should().Be("First");
            first.IsVisible.Should().BeTrue();
            first.ReturnType.Should().Be("string");

            var second = sut.Properties.Skip(1).First();

            second.Name.Should().Be("Second");
            second.IsVisible.Should().BeTrue();
            second.ReturnType.Should().Be("DateTimeOffset");
        }

        [Fact]
        public async Task PropertiesReturnsDeclaredPropertiesOnInterface()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithProperties)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Properties.Should().HaveCount(2);

            var first = sut.Properties.First();

            first.Name.Should().Be("First");
            first.IsVisible.Should().BeTrue();
            first.ReturnType.Should().Be("string");

            var second = sut.Properties.Skip(1).First();

            second.Name.Should().Be("Second");
            second.IsVisible.Should().BeTrue();
            second.ReturnType.Should().Be("DateTimeOffset");
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullDeclaringType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassDefinition(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Usage",
            "CA1806:Do not ignore method results",
            Justification = "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullNodeWithDeclaringType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var parentType = new ClassDefinition(node);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassDefinition(parentType, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}