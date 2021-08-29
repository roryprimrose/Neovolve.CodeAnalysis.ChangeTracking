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
        public async Task ChildEnumsReturnsEmptyWhenNoDeclarationsFound()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildEnums.Should().BeEmpty();
        }

        [Fact]
        public async Task ChildEnumsReturnsMultipleEnumDefinitions()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.MultipleChildEnums)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ChildEnums.Should().HaveCount(2);

            var firstEnum = sut.ChildEnums.First();
            var secondEnum = sut.ChildEnums.Skip(1).First();

            firstEnum.Name.Should().Be("FirstChild");
            firstEnum.FullName.Should().Be("MyNamespace.MyClass+FirstChild");
            firstEnum.DeclaringType.Should().Be(sut);

            secondEnum.Name.Should().Be("SecondChild");
            secondEnum.FullName.Should().Be("MyNamespace.MyClass+SecondChild");
            secondEnum.DeclaringType.Should().Be(sut);
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

            sut.ChildTypes.Should().HaveCount(8);

            sut.ChildTypes.OfType<ClassDefinition>().Should().HaveCount(2);
            sut.ChildTypes.OfType<InterfaceDefinition>().Should().HaveCount(2);
            sut.ChildTypes.OfType<StructDefinition>().Should().HaveCount(2);
            sut.ChildTypes.OfType<EnumDefinition>().Should().HaveCount(2);
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
        public async Task IsVisibleReturnsFalseWhenDeclaringTypeIsVisibleReturnsFalse()
        {
            var code = TypeDefinitionCode.MultipleChildTypes.Replace("public class MyClass", "class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var child = sut.ChildClasses.Single(x => x.Name == "FirstClass");

            child.IsVisible.Should().BeFalse();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", true)]
        [InlineData("private protected", true)]
        [InlineData("protected private", true)]
        [InlineData("protected internal", true)]
        [InlineData("internal protected", true)]
        [InlineData("public", true)]
        public async Task IsVisibleReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(accessModifiers);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsVisible.Should().Be(expected);
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

            firstDefinition.ChildTypes.Count.Should().Be(16);
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstClass");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondClass");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "ThirdClass");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FourthClass");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstInterface");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondInterface");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "ThirdInterface");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FourthInterface");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstStruct");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondStruct");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "ThirdStruct");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FourthStruct");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FirstEnum");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "SecondEnum");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "ThirdEnum");
            firstDefinition.ChildTypes.Should().Contain(x => x.Name == "FourthEnum");
            firstDefinition.ChildTypes.Should().Contain(secondDefinition.ChildTypes);
            firstDefinition.ChildTypes.All(x => x.DeclaringType == firstDefinition).Should().BeTrue();
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
        public async Task MergePartialTypeThrowsExceptionWhenPartialTypeIsDifferent()
        {
            var classNode = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var classDefinition = new ClassDefinition(classNode);

            var interfaceNode = await TestNode
                .FindNode<InterfaceDeclarationSyntax>(TypeDefinitionCode.InterfaceWithMethod).ConfigureAwait(false);

            var interfaceDefinition = new InterfaceDefinition(interfaceNode);

            Action action = () => classDefinition.MergePartialType(interfaceDefinition);

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData("MyClass", "MyOtherClass")]
        [InlineData("MyNamespace", "MyOtherNamespace")]
        public async Task MergePartialTypeThrowsExceptionWhenPartialTypeIsDifferentPartialType(string find,
            string replace)
        {
            var classNode = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var classDefinition = new ClassDefinition(classNode);

            var otherNode = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent.Replace(find, replace))
                .ConfigureAwait(false);

            var otherDefinition = new ClassDefinition(otherNode);

            Action action = () => classDefinition.MergePartialType(otherDefinition);

            action.Should().Throw<InvalidOperationException>();
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