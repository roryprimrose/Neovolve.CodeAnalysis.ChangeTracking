namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class FieldResolverTests
    {
        [Fact]
        public void EvaluateChildrenReturnsFalse()
        {
            var sut = new FieldResolver();

            var actual = sut.EvaluateChildren;

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var resolver = new FieldResolver();

            Action action = () => resolver.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsSupportReturnsFalseIfTheResolverDoesNotMatchTheNodeType()
        {
            var resolver = new FieldResolver();

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
            var resolver = new FieldResolver();

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(TestNode.StandardField).ConfigureAwait(false);

            var actual = resolver.IsSupported(node);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task ResolveReturnsDefinitionWhenFieldHasAssignment()
        {
            var code = @"
namespace MyNamespace 
{
    public class MyClass
    {
        string MyItem = ""test"";
    }   
}
";
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Name.Should().Be("MyItem");
        }

        [Theory]
        [InlineData("string", "string")]
        [InlineData("Stream", "Stream")]
        [InlineData("System.DateTimeOffset", "System.DateTimeOffset")]
        [InlineData("DateTimeOffset", "DateTimeOffset")]
        [InlineData("System.IO.Stream", "System.IO.Stream")]
        [InlineData("[Ignore]string", "string")]
        [InlineData("[Ignore] string", "string")]
        [InlineData("[Serialize] string", "string")]
        public async Task ResolveReturnsFieldDataType(string dataType, string expected)
        {
            var code = TestNode.StandardField.Replace("string MyItem", dataType + " MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.ReturnType.Should().Be(expected);
        }

        [Fact]
        public async Task ResolveReturnsFieldName()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(TestNode.StandardField).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Name.Should().Be("MyItem");
        }

        [Fact]
        public async Task ResolveReturnsMemberType()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(TestNode.StandardField).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.MemberType.Should().Be("Field");
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullNode()
        {
            var resolver = new FieldResolver();

            Action action = () => resolver.Resolve(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SkipNodeReturnsFalse()
        {
            var sut = new FieldResolver();

            var actual = sut.SkipNode;

            actual.Should().BeFalse();
        }
    }
}