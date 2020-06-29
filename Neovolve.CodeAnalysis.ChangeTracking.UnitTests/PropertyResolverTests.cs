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

            Action action = () => sut.IsSupported(null!);

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

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.ClassProperty).ConfigureAwait(false);

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
        [InlineData("protected", true)]
        [InlineData("protected virtual", true)]
        public async Task ResolveReturnsCanRead(string accessors, bool expected)
        {
            var code = TestNode.ClassProperty.Replace("get;", accessors + " get;", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (OldPropertyDefinition) sut.Resolve(node);

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

            var actual = (OldPropertyDefinition) sut.Resolve(node);

            actual.CanRead.Should().BeFalse();
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("public", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", true)]
        [InlineData("protected virtual", true)]
        public async Task ResolveReturnsCanWrite(string accessors, bool expected)
        {
            var code = TestNode.ClassProperty.Replace("set;", accessors + " set;", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (OldPropertyDefinition) sut.Resolve(node);

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

            var actual = (OldPropertyDefinition) sut.Resolve(node);

            actual.CanWrite.Should().BeFalse();
        }

        [Fact]
        public async Task ResolveReturnsDefinitionForClassWithImplementedInterface()
        {
            const string code = @"
namespace MyNamespace 
{
    public class MyClass : IMyInterface
    {
        public string MyItem
        {
            get;
            set;
        }
    }   
}
";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Name.Should().Be("MyItem");
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

            var actual = (OldPropertyDefinition) sut.Resolve(node);

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

            var actual = (OldPropertyDefinition) sut.Resolve(node);

            actual.Name.Should().Be("MyItem");
        }

        [Fact]
        public async Task ResolveReturnsMemberType()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.ClassProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.MemberType.Should().Be(MemberType.Property);
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
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                "public " + dataType + " MyProperty",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (OldPropertyDefinition) sut.Resolve(node);

            actual.ReturnType.Should().Be(expected);
        }

        [Fact]
        public async Task ResolveReturnsPropertyName()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.ClassProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (OldPropertyDefinition) sut.Resolve(node);

            actual.Name.Should().Be("MyProperty");
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullNode()
        {
            var sut = new PropertyResolver();

            Action action = () => sut.Resolve(null!);

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