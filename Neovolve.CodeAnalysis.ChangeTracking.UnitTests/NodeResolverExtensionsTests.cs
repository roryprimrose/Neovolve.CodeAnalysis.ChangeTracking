namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class NodeResolverExtensionsTests
    {
        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            INodeResolver<PropertyDeclarationSyntax, PropertyDefinition> resolver = new PropertyResolver();

            Action action = () => resolver.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsSupportedThrowsExceptionWithNullResolver()
        {
            var code = @"
namespace MyProject
{

}
";
            var node = await TestNode.FindNode<NamespaceDeclarationSyntax>(code).ConfigureAwait(false);

            INodeResolver<PropertyDeclarationSyntax, PropertyDefinition> resolver = null;

            Action action = () => resolver.IsSupported(node);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsSupportReturnsFalseIfTheResolverDoesNotMatchTheNodeType()
        {
            INodeResolver<PropertyDeclarationSyntax, PropertyDefinition> resolver = new PropertyResolver();

            var code = @"
namespace MyProject
{

}
";
            var node = await TestNode.FindNode<NamespaceDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = resolver.IsSupported(node);

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task IsSupportReturnsTrueIfTheResolverMatchesTheNodeType()
        {
            INodeResolver<PropertyDeclarationSyntax, PropertyDefinition> resolver = new PropertyResolver();

            var code = @"
namespace MyProject
{
    public class MyClass
    {
        public string MyProperty
        {
            get;
            set;
        }
    }
}
";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = resolver.IsSupported(node);

            actual.Should().BeTrue();
        }
    }
}