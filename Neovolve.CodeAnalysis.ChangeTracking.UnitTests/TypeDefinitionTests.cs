namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class TypeDefinitionTests
    {
        [Fact]
        public async Task ChildClassesIncludesHierarchyOfClassesAndInterfaces()
        {
            var node = await TestNode
                .FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInParentClassAndInterface)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyGrandparentClass");
            sut.ParentType.Should().BeNull();

            var myInterface = sut.ChildInterfaces.Single();

            myInterface.Name.Should().Be("MyGrandparentClass+IMyInterface");
            myInterface.ParentType.Should().Be(sut);

            var parent = myInterface.ChildClasses.Single();

            parent.Name.Should().Be("MyGrandparentClass+IMyInterface+MyParentClass");
            parent.ParentType.Should().Be(myInterface);

            var myClass = parent.ChildClasses.Single();

            myClass.Name.Should().Be("MyGrandparentClass+IMyInterface+MyParentClass+MyClass");
            myClass.ParentType.Should().Be(parent);

            var myChildInterface = myClass.ChildInterfaces.Single();

            myChildInterface.Name.Should().Be("MyGrandparentClass+IMyInterface+MyParentClass+MyClass+IChildInterface");
            myChildInterface.ParentType.Should().Be(myClass);
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

            firstClass.Name.Should().Be("MyClass+FirstChild");
            firstClass.ParentType.Should().Be(sut);

            secondClass.Name.Should().Be("MyClass+SecondChild");
            secondClass.ParentType.Should().Be(sut);
        }

        [Fact]
        public async Task ChildClassesReturnsSingleClassDefinition()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInParentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyParentClass");
            sut.ChildClasses.Should().HaveCount(1);

            var childClass = sut.ChildClasses.First();

            childClass.Name.Should().Be("MyParentClass+MyClass");
            childClass.ParentType.Should().Be(sut);
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

            firstInterface.Name.Should().Be("MyClass+FirstChild");
            firstInterface.ParentType.Should().Be(sut);

            secondInterface.Name.Should().Be("MyClass+SecondChild");
            secondInterface.ParentType.Should().Be(sut);
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
        [ClassData(typeof(GrandparentHierarchyIsVisibleDataSet))]
        public async Task IsVisibleReturnsValueBasedOnParentType(string grandparentScope, string parentScope,
            string scope, bool expected)
        {
            var code = TypeDefinitionCode.BuildHierarchyWithScope(grandparentScope, parentScope, scope);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            // Find the deepest class
            var child = sut.ChildClasses.Single().ChildClasses.Single();

            child.IsVisible.Should().Be(expected);
        }

        [Theory]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("protected", true)]
        [InlineData("private protected", true)]
        [InlineData("protected internal", true)]
        [InlineData("public", true)]
        public async Task IsVisibleReturnsValueBasedOnScope(string scope, bool expected)
        {
            var code = TypeDefinitionCode.BuildClassWithScope(scope);

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task LocationReturnsEmptyWhenNodeLacksSourceInformation()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Location.FilePath.Should().BeEmpty();
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
            sut.Location.LineIndex.Should().Be(3);
            sut.Location.CharacterIndex.Should().Be(4);
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
        public async Task NameReturnsNameFromClassWithGenericType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithGenericType)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyClass<T>");
        }

        [Fact]
        public async Task NameReturnsNameFromClassWithoutParentType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyClass");
        }

        [Fact]
        public async Task NameReturnsNameFromClassWithParentTypes()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.Name.Should().Be("MyGrandparentClass");

            var parentClass = sut.ChildClasses.Single();

            parentClass.Name.Should().Be("MyGrandparentClass+MyParentClass");

            var childClass = parentClass.ChildClasses.Single();

            childClass.Name.Should().Be("MyGrandparentClass+MyParentClass+MyClass");
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
        public async Task NameReturnsNameFromInterfaceWithoutParentType()
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
        public async Task ParentTypeReturnsNullWhenNoParentTypeFound()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ParentType.Should().BeNull();
        }

        [Fact]
        public async Task ParentTypeReturnsParentClass()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassInGrandparentClass)
                .ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            sut.ParentType.Should().BeNull();

            var parentClass = sut.ChildClasses.Single();

            parentClass.ParentType.Should().Be(sut);

            var childClass = parentClass.ChildClasses.Single();

            childClass.ParentType.Should().Be(parentClass);
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassDefinition(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullNodeWithParentType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            var parentType = new ClassDefinition(node);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassDefinition(parentType, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification =
            "The constructor is the target of the test")]
        public async Task ThrowsExceptionWhenCreatedWithNullParentType()
        {
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(TypeDefinitionCode.ClassWithoutParent)
                .ConfigureAwait(false);

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ClassDefinition(null!, node);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}