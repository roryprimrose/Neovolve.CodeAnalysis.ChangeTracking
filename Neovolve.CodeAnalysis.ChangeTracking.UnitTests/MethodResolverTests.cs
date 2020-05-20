namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class MethodResolverTests
    {
        private const string StandardMethod = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public void MyItem()
        {
        }
    }   
}
";

        [Fact]
        public void EvaluateChildrenReturnsFalse()
        {
            var sut = new MethodResolver();

            var actual = sut.EvaluateChildren;

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var resolver = new MethodResolver();

            Action action = () => resolver.IsSupported(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsSupportReturnsFalseIfTheResolverDoesNotMatchTheNodeType()
        {
            var resolver = new MethodResolver();

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
            var resolver = new MethodResolver();

            var node = await TestNode.FindNode<MethodDeclarationSyntax>(StandardMethod).ConfigureAwait(false);

            var actual = resolver.IsSupported(node);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ResolveThrowsNotSupportedException()
        {
            var resolver = new MethodResolver();

            Action action = () => resolver.Resolve(null!);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void SkipNodeReturnsTrue()
        {
            var sut = new MethodResolver();

            var actual = sut.SkipNode;

            actual.Should().BeTrue();
        }
    }
}