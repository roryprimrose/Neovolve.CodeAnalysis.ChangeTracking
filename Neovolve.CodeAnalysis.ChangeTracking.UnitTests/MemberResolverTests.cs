namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class MemberResolverTests
    {
        private const string StandardField = @"
namespace MyNamespace 
{
    public class MyClass
    {
        string MyItem;
    }   
}
";

        private const string StandardProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        string MyItem
        {
            get;
            set;
        }
    }   
}
";

        [Fact]
        public async Task ResolveReturnsClassAndNamespaceNamesForField()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(StandardField).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().Be("MyNamespace");
            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsClassAndNamespaceNamesForProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(StandardProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().Be("MyNamespace");
            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsClassWithoutNamespaceForField()
        {
            var code = @"
public class MyClass
{
    string MyItem;
}   
";
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsClassWithoutNamespaceForProperty()
        {
            var code = @"
public class MyClass
{
    string MyItem { get; set; }
}   
";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsEmptyAttributesForField()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(StandardField).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().BeEmpty();
        }

        [Fact]
        public async Task ResolveReturnsEmptyAttributesForProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(StandardProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsIsPublicBasedOnNestedParentClassScopeForField(string accessors, bool expected)
        {
            var code = @"
namespace MyNamespace 
{
    class Parent
    {
        public class MyClass
        {
            public string MyItem;
        }   
    }
}
";
            ;

            code = code.Replace("class Parent", accessors + " class Parent", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.IsPublic.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsIsPublicBasedOnNestedParentClassScopeForProperty(
            string accessors,
            bool expected)
        {
            var code = @"
namespace MyNamespace 
{
    class Parent
    {
        public class MyClass
        {
            public string MyItem { get; set; }
        }   
    }
}
";
            ;

            code = code.Replace("class Parent", accessors + " class Parent", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.IsPublic.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsIsPublicBasedOnParentClassScopeForField(string accessors, bool expected)
        {
            var code = StandardField
                .Replace("public class MyClass", accessors + " class MyClass", StringComparison.Ordinal)
                .Replace("string MyItem", "public string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.IsPublic.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsIsPublicBasedOnParentClassScopeForProperty(string accessors, bool expected)
        {
            var code = StandardProperty
                .Replace("public class MyClass", accessors + " class MyClass", StringComparison.Ordinal)
                .Replace("string MyItem", "public string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.IsPublic.Should().Be(expected);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("public", true)]
        [InlineData("public readonly", true)]
        [InlineData("public virtual", true)]
        [InlineData("private", false)]
        [InlineData("internal", false)]
        [InlineData("internal virtual", false)]
        [InlineData("protected", false)]
        [InlineData("protected virtual", false)]
        public async Task ResolveReturnsIsPublicForField(string accessors, bool expected)
        {
            var code = StandardField.Replace("string MyItem", accessors + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.IsPublic.Should().Be(expected);
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
        public async Task ResolveReturnsIsPublicForProperty(string accessors, bool expected)
        {
            var code = StandardProperty.Replace("string MyItem",
                accessors + " string MyItem",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

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
        public async Task ResolveReturnsMultipleAttributeListsForField(
            string fragment,
            string firstDeclaration,
            string firstName,
            string secondDeclaration,
            string secondName,
            string thirdDeclaration,
            string thirdName)
        {
            var code = StandardField.Replace("string MyItem", fragment + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().HaveCount(3);
            actual.Attributes.First().Declaration.Should().Be(firstDeclaration);
            actual.Attributes.First().Name.Should().Be(firstName);
            actual.Attributes.Skip(1).First().Declaration.Should().Be(secondDeclaration);
            actual.Attributes.Skip(1).First().Name.Should().Be(secondName);
            actual.Attributes.Skip(2).First().Declaration.Should().Be(thirdDeclaration);
            actual.Attributes.Skip(2).First().Name.Should().Be(thirdName);
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
        public async Task ResolveReturnsMultipleAttributeListsForProperty(
            string fragment,
            string firstDeclaration,
            string firstName,
            string secondDeclaration,
            string secondName,
            string thirdDeclaration,
            string thirdName)
        {
            var code = StandardProperty.Replace("string MyItem", fragment + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

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
        public async Task ResolveReturnsMultipleAttributesForField(
            string fragment,
            string firstDeclaration,
            string firstName,
            string secondDeclaration,
            string secondName)
        {
            var code = StandardField.Replace("string MyItem", fragment + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().HaveCount(2);
            actual.Attributes.First().Declaration.Should().Be(firstDeclaration);
            actual.Attributes.First().Name.Should().Be(firstName);
            actual.Attributes.Skip(1).First().Declaration.Should().Be(secondDeclaration);
            actual.Attributes.Skip(1).First().Name.Should().Be(secondName);
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
        public async Task ResolveReturnsMultipleAttributesForProperty(
            string fragment,
            string firstDeclaration,
            string firstName,
            string secondDeclaration,
            string secondName)
        {
            var code = StandardProperty.Replace("string MyItem", fragment + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().HaveCount(2);
            actual.Attributes.First().Declaration.Should().Be(firstDeclaration);
            actual.Attributes.First().Name.Should().Be(firstName);
            actual.Attributes.Skip(1).First().Declaration.Should().Be(secondDeclaration);
            actual.Attributes.Skip(1).First().Name.Should().Be(secondName);
        }

        [Fact]
        public async Task ResolveReturnsNestedClassNameForField()
        {
            var code = @"
public class Parent
{
    public class MyClass
    {
        string MyItem;
    }   
}";
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("Parent+MyClass");
        }

        [Fact]
        public async Task ResolveReturnsNestedClassNameForProperty()
        {
            var code = @"
public class Parent
{
    public class MyClass
    {
        string MyItem { get; set; }
    }   
}";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("Parent+MyClass");
        }

        [Theory]
        [InlineData("[Ignore]", "Ignore", "Ignore")]
        [InlineData("[Serialize]", "Serialize", "Serialize")]
        [InlineData("[Obsolete]", "Obsolete", "Obsolete")]
        [InlineData("[Obsolete(\"This member will be removed in the next version\")]",
            "Obsolete(\"This member will be removed in the next version\")",
            "Obsolete")]
        public async Task ResolveReturnsSingleAttributeForField(string fragment, string declaration, string name)
        {
            var code = StandardField.Replace("string MyItem", fragment + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().HaveCount(1);
            actual.Attributes.First().Declaration.Should().Be(declaration);
            actual.Attributes.First().Name.Should().Be(name);
        }

        [Theory]
        [InlineData("[Ignore]", "Ignore", "Ignore")]
        [InlineData("[Serialize]", "Serialize", "Serialize")]
        [InlineData("[Obsolete]", "Obsolete", "Obsolete")]
        [InlineData("[Obsolete(\"This member will be removed in the next version\")]",
            "Obsolete(\"This member will be removed in the next version\")",
            "Obsolete")]
        public async Task ResolveReturnsSingleAttributeForProperty(string fragment, string declaration, string name)
        {
            var code = StandardProperty.Replace("string MyItem", fragment + " string MyItem", StringComparison.Ordinal);

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

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