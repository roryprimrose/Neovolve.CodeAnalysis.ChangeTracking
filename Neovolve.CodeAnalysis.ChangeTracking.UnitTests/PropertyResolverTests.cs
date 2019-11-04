namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class PropertyResolverTests
    {
        private const string StandardProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        string MyProperty
        {
            get;
            set;
        }
    }   
}
";

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
            var resolver = new PropertyResolver();

            Action action = () => resolver.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task IsSupportReturnsFalseIfTheResolverDoesNotMatchTheNodeType()
        {
            var resolver = new PropertyResolver();

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
            var resolver = new PropertyResolver();

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(StandardProperty).ConfigureAwait(false);

            var actual = resolver.IsSupported(node);

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
            var code = StandardProperty.Replace("get;", accessors + " get;");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.CanRead.Should().Be(expected);
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
            var code = StandardProperty.Replace("set;", accessors + " set;");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.CanWrite.Should().Be(expected);
        }

        [Fact]
        public async Task ResolveReturnsEmptyAttributes()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(StandardProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.Attributes.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsIsPublic(string accessors, bool expected)
        {
            var code = StandardProperty.Replace("string MyProperty", accessors + " string MyProperty");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.IsPublic.Should().Be(expected);
        }

        [Theory]
        [InlineData("[Ignore, Serialize][Custom]", "Ignore", "Ignore", "Serialize", "Serialize", "Custom", "Custom")]
        [InlineData("[Ignore][Serialize][Custom]", "Ignore", "Ignore", "Serialize", "Serialize", "Custom", "Custom")]
        [InlineData(@"[Ignore]
[Serialize]
[Custom]
",
            "Ignore",
            "Ignore",
            "Serialize",
            "Serialize",
            "Custom",
            "Custom")]
        [InlineData("[Custom][Obsolete(\"message\"), Serialize]",
            "Custom",
            "Custom",
            "Obsolete(\"message\")",
            "Obsolete",
            "Serialize",
            "Serialize")]
        [InlineData("[Custom(true)][Serialize][Obsolete(\"message\")]",
            "Custom(true)",
            "Custom",
            "Serialize",
            "Serialize",
            "Obsolete(\"message\")",
            "Obsolete")]
        public async Task ResolveReturnsMultipleAttributeLists(
            string fragment,
            string firstDeclaration,
            string firstName,
            string secondDeclaration,
            string secondName,
            string thirdDeclaration,
            string thirdName)
        {
            var code = StandardProperty.Replace("string MyProperty", fragment + " string MyProperty");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.Attributes.Should().HaveCount(3);
            actual.Attributes.First().Declaration.Should().Be(firstDeclaration);
            actual.Attributes.First().Name.Should().Be(firstName);
            actual.Attributes.Skip(1).First().Declaration.Should().Be(secondDeclaration);
            actual.Attributes.Skip(1).First().Name.Should().Be(secondName);
            actual.Attributes.Skip(2).First().Declaration.Should().Be(thirdDeclaration);
            actual.Attributes.Skip(2).First().Name.Should().Be(thirdName);
        }

        [Theory]
        [InlineData("[Ignore, Serialize]", "Ignore", "Ignore", "Serialize", "Serialize")]
        [InlineData("[Obsolete(\"message\"), Serialize]",
            "Obsolete(\"message\")",
            "Obsolete",
            "Serialize",
            "Serialize")]
        [InlineData("[Custom(true), Obsolete(\"message\")]",
            "Custom(true)",
            "Custom",
            "Obsolete(\"message\")",
            "Obsolete")]
        public async Task ResolveReturnsMultipleAttributes(
            string fragment,
            string firstDeclaration,
            string firstName,
            string secondDeclaration,
            string secondName)
        {
            var code = StandardProperty.Replace("string MyProperty", fragment + " string MyProperty");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.Attributes.Should().HaveCount(2);
            actual.Attributes.First().Declaration.Should().Be(firstDeclaration);
            actual.Attributes.First().Name.Should().Be(firstName);
            actual.Attributes.Skip(1).First().Declaration.Should().Be(secondDeclaration);
            actual.Attributes.Skip(1).First().Name.Should().Be(secondName);
        }

        [Fact]
        public async Task ResolveReturnsPropertyClassAndNamespaceNames()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(StandardProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.Namespace.Should().Be("MyNamespace");
            actual.OwningType.Should().Be("MyClass");
            actual.Name.Should().Be("MyProperty");
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
            var code = StandardProperty.Replace("string MyProperty", dataType + " MyProperty");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.ReturnType.Should().Be(expected);
        }

        [Theory]
        [InlineData("[Ignore]", "Ignore", "Ignore")]
        [InlineData("[Serialize]", "Serialize", "Serialize")]
        [InlineData("[Obsolete]", "Obsolete", "Obsolete")]
        [InlineData("[Obsolete(\"This member will be removed in the next version\")]",
            "Obsolete(\"This member will be removed in the next version\")",
            "Obsolete")]
        public async Task ResolveReturnsSingleAttribute(string fragment, string declaration, string name)
        {
            var code = StandardProperty.Replace("string MyProperty", fragment + " string MyProperty");

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition)sut.Resolve(node);

            actual.Attributes.Should().HaveCount(1);
            actual.Attributes.First().Declaration.Should().Be(declaration);
            actual.Attributes.First().Name.Should().Be(name);
        }

        [Fact]
        public void ResolveThrowsExceptionWithNullNode()
        {
            var resolver = new PropertyResolver();

            Action action = () => resolver.Resolve(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}