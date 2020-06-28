namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class InterfaceDefinitionTests
    {
        private const string EmptyInterface = @"
namespace MyNamespace 
{
    public interface MyInterface
    {
    }   
}
";

        [Fact]
        public async Task CanCreateFromDeclarationNode()
        {
            var node = await TestNode.FindNode<InterfaceDeclarationSyntax>(EmptyInterface)
                .ConfigureAwait(false);

            var sut = new InterfaceDefinition(node);

            sut.Name.Should().Be("MyInterface");
            sut.Namespace.Should().Be("MyNamespace");
            sut.ParentType.Should().BeNull();
            sut.ChildClasses.Should().BeEmpty();
        }
    }
}