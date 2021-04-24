namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
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
        public async Task AccessModifierReturnsPrivateForNestedClassWithoutAccessModifier()
        {
            var code = TypeDefinitionCode.MultipleChildTypes.Replace("public class FirstClass", "class FirstClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var child = sut.ChildClasses.Single(x => x.Name == "FirstClass");

            child.AccessModifiers.Should().Be(AccessModifiers.Private);
        }

        [Fact]
        public async Task AccessModifierReturnsPrivateForNestedStructWithoutAccessModifier()
        {
            var code = TypeDefinitionCode.MultipleChildTypes.Replace("public struct FirstStruct", "struct FirstStruct");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var child = sut.ChildStructs.Single(x => x.Name == "FirstStruct");

            child.AccessModifiers.Should().Be(AccessModifiers.Private);
        }

        [Theory]
        [InlineData("", AccessModifiers.Internal)]
        [InlineData("private", AccessModifiers.Private)]
        [InlineData("internal", AccessModifiers.Internal)]
        [InlineData("protected", AccessModifiers.Protected)]
        [InlineData("private protected", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected private", AccessModifiers.ProtectedPrivate)]
        [InlineData("protected internal", AccessModifiers.ProtectedInternal)]
        [InlineData("internal protected", AccessModifiers.ProtectedInternal)]
        [InlineData("public", AccessModifiers.Public)]
        public async Task AccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            AccessModifiers expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.AccessModifiers.Should().Be(expected);
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
        public async Task ChildStructsIncludesHierarchyOfStructsAndInterfaces()
        {
            var node = await TestNode
                .FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInParentStructAndInterface)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Name.Should().Be("MyGrandparentStruct");
            sut.FullName.Should().Be("MyNamespace.MyGrandparentStruct");
            sut.DeclaringType.Should().BeNull();

            var myInterface = sut.ChildInterfaces.Single();

            myInterface.Name.Should().Be("IMyInterface");
            myInterface.FullName.Should().Be("MyNamespace.MyGrandparentStruct+IMyInterface");
            myInterface.DeclaringType.Should().Be(sut);

            var parent = myInterface.ChildStructs.Single();

            parent.Name.Should().Be("MyParentStruct");
            parent.FullName.Should().Be("MyNamespace.MyGrandparentStruct+IMyInterface+MyParentStruct");
            parent.DeclaringType.Should().Be(myInterface);

            var myStruct = parent.ChildStructs.Single();

            myStruct.Name.Should().Be("MyStruct");
            myStruct.FullName.Should().Be("MyNamespace.MyGrandparentStruct+IMyInterface+MyParentStruct+MyStruct");
            myStruct.DeclaringType.Should().Be(parent);

            var myChildInterface = myStruct.ChildInterfaces.Single();

            myChildInterface.Name.Should().Be("IChildInterface");
            myChildInterface.FullName.Should().Be(
                "MyNamespace.MyGrandparentStruct+IMyInterface+MyParentStruct+MyStruct+IChildInterface");
            myChildInterface.DeclaringType.Should().Be(myStruct);
        }

        [Fact]
        public async Task ChildStructsReturnsEmptyWhenNoDeclarationsFound()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithoutParent)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.ChildStructs.Should().BeEmpty();
        }

        [Fact]
        public async Task ChildStructsReturnsMultipleStructDefinitions()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.MultipleChildStructs)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.ChildStructs.Should().HaveCount(2);

            var firstStruct = sut.ChildStructs.First();
            var secondStruct = sut.ChildStructs.Skip(1).First();

            firstStruct.Name.Should().Be("FirstChild");
            firstStruct.FullName.Should().Be("MyNamespace.MyStruct+FirstChild");
            firstStruct.DeclaringType.Should().Be(sut);

            secondStruct.Name.Should().Be("SecondChild");
            secondStruct.FullName.Should().Be("MyNamespace.MyStruct+SecondChild");
            secondStruct.DeclaringType.Should().Be(sut);
        }

        [Fact]
        public async Task ChildStructsReturnsSingleStructDefinition()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructInParentStruct)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Name.Should().Be("MyParentStruct");
            sut.FullName.Should().Be("MyNamespace.MyParentStruct");
            sut.ChildStructs.Should().HaveCount(1);

            var childStruct = sut.ChildStructs.First();

            childStruct.Name.Should().Be("MyStruct");
            childStruct.FullName.Should().Be("MyNamespace.MyParentStruct+MyStruct");
            childStruct.DeclaringType.Should().Be(sut);
        }

        [Fact]
        public async Task ChildTypesReturnsMultipleTypeDefinitions()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.MultipleChildTypes)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildTypes.Should().HaveCount(6);

            sut.ChildTypes.OfType<ClassDefinition>().Should().HaveCount(2);
            sut.ChildTypes.OfType<InterfaceDefinition>().Should().HaveCount(2);
            sut.ChildTypes.OfType<StructDefinition>().Should().HaveCount(2);
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
        public async Task GenericConstraintsReturnsDeclaredConstraints()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithMultipleGenericConstraints)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.GenericConstraints.Should().HaveCount(2);
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
        public async Task IsVisibleReturnsValueBasedOnAccessModifiers(string accessModifiers, bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task MergePartialTypeThrowsExceptionWithNullPartialType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            Action action = () => sut.MergePartialType(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task MergePartialTypeThrowsExceptionWhenPartialTypeIsDifferent()
        {
            var classNode = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var classDefinition = new ClassDefinition(classNode);

            var interfaceNode = await TestNode.FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMethod).ConfigureAwait(false);

            var interfaceDefinition = new InterfaceDefinition(interfaceNode);

            Action action = () => classDefinition.MergePartialType(interfaceDefinition);

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData("MyClass", "MyOtherClass")]
        [InlineData("MyNamespace", "MyOtherNamespace")]
        public async Task MergePartialTypeThrowsExceptionWhenPartialTypeIsDifferentPartialType(string find, string replace)
        {
            var classNode = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var classDefinition = new ClassDefinition(classNode);

            var otherNode = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent.Replace(find, replace)).ConfigureAwait(false);

            var otherDefinition = new ClassDefinition(otherNode);

            Action action = () => classDefinition.MergePartialType(otherDefinition);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task MergePartialTypeMergesAttributes()
        {
            var firstClass = TypeDefinitionCode.ClassWithAttribute.Replace("class", "partial class");
            var secondClass = TypeDefinitionCode.ClassWithMultipleAttributes.Replace("class", "partial class");

            var firstNode = await TestNode.FindNode<ClassDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<ClassDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new ClassDefinition(firstNode);
            var secondDefinition = new ClassDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.Attributes.Count.Should().Be(3);
            firstDefinition.Attributes.Should().Contain(x => x.Name == "JsonPropertyName");
            firstDefinition.Attributes.Should().Contain(secondDefinition.Attributes);
        }

        [Fact]
        public async Task MergePartialTypeMergesMethods()
        {
            var firstClass = TypeDefinitionCode.ClassWithMethod.Replace("class", "partial class");
            var secondClass = firstClass.Replace("GetValue", "GetOtherValue");

            var firstNode = await TestNode.FindNode<ClassDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<ClassDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new ClassDefinition(firstNode);
            var secondDefinition = new ClassDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.Methods.Count.Should().Be(2);
            firstDefinition.Methods.Should().Contain(x => x.Name == "GetValue");
            firstDefinition.Methods.Should().Contain(secondDefinition.Methods);
            firstDefinition.Methods.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
        }

        [Fact]
        public async Task MergePartialTypeMergesProperties()
        {
            var firstClass = TypeDefinitionCode.ClassWithProperties.Replace("class", "partial class");
            var secondClass = firstClass.Replace("First", "Third").Replace("Second", "Fourth");

            var firstNode = await TestNode.FindNode<ClassDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<ClassDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new ClassDefinition(firstNode);
            var secondDefinition = new ClassDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.Properties.Count.Should().Be(4);
            firstDefinition.Properties.Should().Contain(x => x.Name == "First");
            firstDefinition.Properties.Should().Contain(x => x.Name == "Second");
            firstDefinition.Properties.Should().Contain(secondDefinition.Properties);
            firstDefinition.Properties.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
        }

        [Fact]
        public async Task MergePartialTypeMergesChildClasses()
        {
            var firstClass = TypeDefinitionCode.MultipleChildClasses.Replace("class", "partial class");
            var secondClass = firstClass.Replace("First", "Third").Replace("Second", "Fourth");

            var firstNode = await TestNode.FindNode<ClassDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<ClassDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new ClassDefinition(firstNode);
            var secondDefinition = new ClassDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.ChildClasses.Count.Should().Be(4);
            firstDefinition.ChildClasses.Should().Contain(x => x.Name == "FirstChild");
            firstDefinition.ChildClasses.Should().Contain(x => x.Name == "SecondChild");
            firstDefinition.ChildClasses.Should().Contain(secondDefinition.ChildClasses);
            firstDefinition.ChildClasses.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
        }

        [Fact]
        public async Task MergePartialTypeMergesChildInterfaces()
        {
            var firstClass = TypeDefinitionCode.MultipleChildInterfaces.Replace("class", "partial class");
            var secondClass = firstClass.Replace("First", "Third").Replace("Second", "Fourth");

            var firstNode = await TestNode.FindNode<ClassDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<ClassDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new ClassDefinition(firstNode);
            var secondDefinition = new ClassDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.ChildInterfaces.Count.Should().Be(4);
            firstDefinition.ChildInterfaces.Should().Contain(x => x.Name == "FirstChild");
            firstDefinition.ChildInterfaces.Should().Contain(x => x.Name == "SecondChild");
            firstDefinition.ChildInterfaces.Should().Contain(secondDefinition.ChildInterfaces);
            firstDefinition.ChildInterfaces.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
        }

        [Fact]
        public async Task MergePartialTypeMergesChildStructs()
        {
            var firstClass = TypeDefinitionCode.MultipleChildStructs.Replace("class", "partial class");
            var secondClass = firstClass.Replace("First", "Third").Replace("Second", "Fourth");

            var firstNode = await TestNode.FindNode<StructDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<StructDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new StructDefinition(firstNode);
            var secondDefinition = new StructDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.ChildStructs.Count.Should().Be(4);
            firstDefinition.ChildStructs.Should().Contain(x => x.Name == "FirstChild");
            firstDefinition.ChildStructs.Should().Contain(x => x.Name == "SecondChild");
            firstDefinition.ChildStructs.Should().Contain(secondDefinition.ChildStructs);
            firstDefinition.ChildStructs.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
        }

        [Fact]
        public async Task MergePartialTypeMergesChildTypes()
        {
            var firstClass = TypeDefinitionCode.MultipleChildTypes.Replace("class", "partial class");
            var secondClass = firstClass.Replace("First", "Third").Replace("Second", "Fourth");

            var firstNode = await TestNode.FindNode<ClassDeclarationSyntax>(firstClass)
                .ConfigureAwait(false);
            var secondNode = await TestNode.FindNode<ClassDeclarationSyntax>(secondClass)
                .ConfigureAwait(false);

            var firstDefinition = new ClassDefinition(firstNode);
            var secondDefinition = new ClassDefinition(secondNode);

            firstDefinition.MergePartialType(secondDefinition);

            firstDefinition.ChildTypes.Count.Should().Be(12);
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstClass");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondClass");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstInterface");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondInterface");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstStruct");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondStruct");
            firstDefinition.ChildTypes.Should().Contain(secondDefinition.ChildTypes);
            firstDefinition.ChildTypes.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
        }

        [Fact]
        public async Task MethodsOnClassReturnsDeclaredMethod()
        {
            const string? code = TypeDefinitionCode.ClassWithMethod;

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var method = sut.Methods.Single();

            method.Name.Should().Be("GetValue");
            method.Parameters.Count.Should().Be(3);
        }

        [Fact]
        public async Task MethodsOnInterfaceReturnsDeclaredMethod()
        {
            const string? code = TypeDefinitionCode.InterfaceWithMethod;

            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            var method = sut.Methods.Single();

            method.Name.Should().Be("GetValue");
            method.Parameters.Count.Should().Be(3);
        }

        [Fact]
        public async Task MethodsOnStructReturnsDeclaredMethod()
        {
            const string? code = TypeDefinitionCode.StructWithMethod;

            var node = await TestNode.FindNode<StructDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new StructDefinition(node);

            var method = sut.Methods.Single();

            method.Name.Should().Be("GetValue");
            method.Parameters.Count.Should().Be(3);
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
        public async Task PropertiesReturnsDeclaredPropertiesOnClass()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithProperties)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Properties.Should().HaveCount(2);

            var first = sut.Properties.First();

            first.Name.Should().Be("First");

            var second = sut.Properties.Skip(1).First();

            second.Name.Should().Be("Second");
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

            var second = sut.Properties.Skip(1).First();

            second.Name.Should().Be("Second");
        }

        [Fact]
        public async Task PropertiesReturnsDeclaredPropertiesOnStruct()
        {
            var node = await TestNode.FindNode<StructDeclarationSyntax>(TypeDefinitionCode.StructWithProperties)
                .ConfigureAwait(false);

            var sut = new StructDefinition(node);

            sut.Properties.Should().HaveCount(2);

            var first = sut.Properties.First();

            first.Name.Should().Be("First");

            var second = sut.Properties.Skip(1).First();

            second.Name.Should().Be("Second");
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