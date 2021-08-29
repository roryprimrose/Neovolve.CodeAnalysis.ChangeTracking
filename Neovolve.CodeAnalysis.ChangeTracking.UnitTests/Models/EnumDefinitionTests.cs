namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class EnumDefinitionTests
    {
        private const string EnumMembersWithExplicitValues = @"
namespace MyNamespace 
{
    public enum MyEnum
    {
        First = 123,
        Second = 234,
        Third = 345
    }   
}
";

        private const string EnumMembersWithFlagsValues = @"
namespace MyNamespace 
{
    [Flags]
    public enum MyEnum
    {
        First = 1,
        Second = 2,
        Third = 4,
        All = First | Second | Third
    }   
}
";

        private const string EnumMembersWithImplicitValues = @"
namespace MyNamespace 
{
    public enum MyEnum
    {
        First,
        Second,
        Third
    }   
}
";

        [Fact]
        public async Task AccessModifierReturnsPrivateForNestedEnumWithoutAccessModifier()
        {
            var code = TypeDefinitionCode.MultipleChildTypes.Replace("public enum FirstEnum", "enum FirstEnum");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var child = sut.ChildEnums.Single(x => x.Name == "FirstEnum");

            child.AccessModifiers.Should().Be(EnumAccessModifiers.Private);
        }

        [Theory]
        [InlineData("", EnumAccessModifiers.Internal)]
        [InlineData("private", EnumAccessModifiers.Private)]
        [InlineData("internal", EnumAccessModifiers.Internal)]
        [InlineData("protected", EnumAccessModifiers.Protected)]
        [InlineData("public", EnumAccessModifiers.Public)]
        public async Task AccessModifierReturnsValueBasedOnAccessModifiers(
            string accessModifiers,
            EnumAccessModifiers expected)
        {
            var code = EnumMembersWithImplicitValues.Replace("public enum MyEnum", accessModifiers + " enum MyEnum");

            var node = await TestNode.FindNode<EnumDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.AccessModifiers.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData("byte")]
        [InlineData("sbyte")]
        [InlineData("short")]
        [InlineData("ushort")]
        [InlineData("int")]
        [InlineData("uint")]
        [InlineData("long")]
        [InlineData("ulong")]
        public async Task ImplementedTypesReturnsDeclaredValue(string baseType)
        {
            var code = EnumMembersWithImplicitValues.Replace("MyEnum", "MyEnum : " + baseType);

            var node = await TestNode.FindNode<EnumDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.ImplementedTypes.Should().HaveCount(1);
            sut.ImplementedTypes.First().Should().Be(baseType);
        }

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var node = await TestNode.FindNode<EnumDeclarationSyntax>(EnumMembersWithExplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.Name.Should().Be("MyEnum");
            sut.Namespace.Should().Be("MyNamespace");
            sut.DeclaringType.Should().BeNull();
        }

        [Fact]
        public async Task CanCreateFromDeclarationNodeAndDeclaringType()
        {
            var declaringType = new TestClassDefinition();

            var node = await TestNode.FindNode<EnumDeclarationSyntax>(EnumMembersWithExplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumDefinition(declaringType, node);

            sut.Name.Should().Be("MyEnum");
            sut.Namespace.Should().Be("MyNamespace");
            sut.DeclaringType.Should().Be(declaringType);
        }

        [Fact]
        public async Task IsVisibleReturnsFalseWhenDeclaringTypeIsNotVisible()
        {
            var code = TypeDefinitionCode.MultipleChildTypes.Replace("public class MyClass", "internal class MyClass");
            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var childType = sut.ChildEnums.First(x => x.Name == "FirstEnum");

            childType.IsVisible.Should().BeFalse();
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
        public async Task IsVisibleReturnsValueBasedOnAccessModifiersOnDeclaringType(
            string accessModifiers,
            bool expected)
        {
            var code = TypeDefinitionCode.MultipleChildTypes.Replace("public class MyClass",
                accessModifiers + " class MyClass");

            var node = await TestNode.FindNode<ClassDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new ClassDefinition(node);

            var childType = sut.ChildEnums.First(x => x.Name == "FirstEnum");

            childType.IsVisible.Should().Be(expected);
        }

        [Fact]
        public async Task MembersReturnsEmptyValuesForImplicitEnumValues()
        {
            var node = await TestNode.FindNode<EnumDeclarationSyntax>(EnumMembersWithImplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.Members.Should().HaveCount(3);
            sut.Members.Single(x => x.Name == "First").Value.Should().BeEmpty();
            sut.Members.Single(x => x.Name == "Second").Value.Should().BeEmpty();
            sut.Members.Single(x => x.Name == "Third").Value.Should().BeEmpty();
        }

        [Fact]
        public async Task MembersReturnsExplicitEnumValues()
        {
            var node = await TestNode.FindNode<EnumDeclarationSyntax>(EnumMembersWithExplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.Members.Should().HaveCount(3);
            sut.Members.Single(x => x.Name == "First").Value.Should().Be("123");
            sut.Members.Single(x => x.Name == "Second").Value.Should().Be("234");
            sut.Members.Single(x => x.Name == "Third").Value.Should().Be("345");
        }

        [Fact]
        public async Task MembersReturnsFlagsEnumValues()
        {
            var node = await TestNode.FindNode<EnumDeclarationSyntax>(EnumMembersWithFlagsValues)
                .ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.Members.Should().HaveCount(4);
            sut.Members.Single(x => x.Name == "First").Value.Should().Be("1");
            sut.Members.Single(x => x.Name == "Second").Value.Should().Be("2");
            sut.Members.Single(x => x.Name == "Third").Value.Should().Be("4");
            sut.Members.Single(x => x.Name == "All").Value.Should().Be("First | Second | Third");
        }
    }
}