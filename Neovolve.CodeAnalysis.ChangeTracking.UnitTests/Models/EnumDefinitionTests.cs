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

        [Fact]
        public async Task MembersReturnsImplicitEnumValues()
        {
            var node = await TestNode.FindNode<EnumDeclarationSyntax>(EnumMembersWithImplicitValues)
                .ConfigureAwait(false);

            var sut = new EnumDefinition(node);

            sut.Members.Should().HaveCount(3);
            sut.Members.Single(x => x.Name == "First").Value.Should().Be("0");
            sut.Members.Single(x => x.Name == "Second").Value.Should().Be("1");
            sut.Members.Single(x => x.Name == "Third").Value.Should().Be("2");
        }
    }
}