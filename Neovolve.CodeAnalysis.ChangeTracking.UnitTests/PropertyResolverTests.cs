namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class PropertyResolverTests
    {
        [Fact]
        public void EvaluateChildrenReturnsFalse()
        {
            var sut = new PropertyResolver();

            var actual = sut.EvaluateChildren;

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullNode()
        {
            var sut = new PropertyResolver();

            Action action = () => sut.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsSupportReturnsFalseIfTheResolverDoesNotMatchTheNodeType()
        {
            var sut = new PropertyResolver();

            var code = @"
namespace MyProject
{

}
";
            var node = await TestNode.FindNode<NamespaceDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = sut.IsSupported(node);

            actual.Should().BeFalse();
        }

        [Fact]
        public async Task IsSupportReturnsTrueIfTheResolverMatchesTheNodeType()
        {
            var sut = new PropertyResolver();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.StandardProperty).ConfigureAwait(false);

            var actual = sut.IsSupported(node);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("public", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsCanRead(string accessors, bool expected)
        {
            var code = TestNode.StandardProperty.Replace("get;", accessors + " get;", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.CanRead.Should().Be(expected);
        }

        [Fact]
        public async Task ResolveReturnsCanReadAsFalseWithWriteOnlyProperty()
        {
            const string code = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string MyItem
        {
            set;
        }
    }   
}
";
            var sut = new PropertyResolver();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.CanRead.Should().BeFalse();
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("public", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsCanWrite(string accessors, bool expected)
        {
            var code = TestNode.StandardProperty.Replace("set;", accessors + " set;", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.CanWrite.Should().Be(expected);
        }

        [Fact]
        public async Task ResolveReturnsCanWriteAsFalseWithReadOnlyProperty()
        {
            const string code = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string MyItem
        {
            get;
        }
    }   
}
";
            var sut = new PropertyResolver();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.CanWrite.Should().BeFalse();
        }

        [Fact]
        public async Task ResolveReturnsDefinitionWhenPropertyHasAssignment()
        {
            const string code = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string MyItem
        {
            get;
            set;
        } = ""stuff""
    }   
}
";
            var sut = new PropertyResolver();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.Name.Should().Be("MyItem");
        }

        [Fact]
        public async Task ResolveReturnsDefinitionWhenPropertyHasExpression()
        {
            const string code = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string MyItem
        {
            get;
            set;
        } => ""stuff""
    }   
}
";
            var sut = new PropertyResolver();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var actual = (PropertyDefinition) sut.Resolve(node);

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
        public async Task ResolveReturnsPropertyDataType(string dataType, string expected)
        {
            var code = TestNode.StandardProperty.Replace("string MyItem", dataType + " MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.ReturnType.Should().Be(expected);
        }

        [Fact]
        public async Task ResolveReturnsPropertyName()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.StandardProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.Name.Should().Be("MyItem");
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullNode()
        {
            var sut = new PropertyResolver();

            Action action = () => sut.Resolve(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SkipNodeReturnsFalse()
        {
            var sut = new PropertyResolver();

            var actual = sut.SkipNode;

            actual.Should().BeFalse();
        }
    }
}