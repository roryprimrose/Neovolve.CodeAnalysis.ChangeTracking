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
        [Theory]
        [InlineData("Ignore", null, "IgnoreAttribute", "Ignore")]
        [InlineData("System.Ignore", "System", "IgnoreAttribute", "Ignore")]
        [InlineData("System.IgnoreAttribute", "System", "IgnoreAttribute", "Ignore")]
        [InlineData("System.Here.Ignore", "System.Here", "IgnoreAttribute", "Ignore")]
        [InlineData("System.Here.Ignore()", "System.Here", "IgnoreAttribute", "Ignore")]
        [InlineData("System.Here.IgnoreAttribute()", "System.Here", "IgnoreAttribute", "Ignore")]
        [InlineData("System.Here.Ignore(true, \"something\", 123)", "System.Here", "IgnoreAttribute", "Ignore")]
        public async Task ResolveCanParseAttributeInformation(
            string declaration,
            string namespaceIdentifier,
            string owningType,
            string name)
        {
            var code = TestNode.Field.Replace("public string MyField",
                "[" + declaration + "] public string MyField",
                StringComparison.Ordinal);

            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().HaveCount(1);

            var value = actual.Attributes.First();

            value.Namespace.Should().Be(namespaceIdentifier);
            value.OwningType.Should().Be(owningType);
            value.Name.Should().Be(name);
            value.Declaration.Should().Be(declaration);
            value.IsPublic.Should().BeTrue();
            value.MemberType.Should().Be(MemberType.Attribute);
        }

        [Theory]
        [InlineData("MyClass.cs")]
        [InlineData("MyNamespace\\MyClass.cs")]
        [InlineData("MyNamespace//MyClass.cs")]
        [InlineData("")]
        public async Task ResolvePopulatesFilePathWherePossible(string filePath)
        {
            var code = TestNode.ClassProperty;

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code, filePath).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.FilePath.Should().Be(filePath);
        }

        [Fact]
        public async Task ResolvePopulatesMemberStartPosition()
        {
            var code = TestNode.ClassProperty;

            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.LineIndex.Should().BeGreaterThan(0);
            actual.CharacterIndex.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ResolveReturnsEmptyAttributesForField()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(TestNode.Field).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Attributes.Should().BeEmpty();
        }

        [Fact]
        public async Task ResolveReturnsEmptyAttributesForProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.ClassProperty).ConfigureAwait(false);

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
        public async Task ResolveReturnsIsPublicBasedOnNestedGrandparentClassScopeForClassProperty(
            string accessors,
            bool expected)
        {
            var code = @"
namespace MyNamespace 
{
    class Grandparent
    {
        public class Parent
        {
            public class MyClass
            {
                public string MyItem { get; set; }
            }   
        }
    }
}
";

            code = code.Replace("class Grandparent", accessors + " class Grandparent", StringComparison.Ordinal);

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
        public async Task ResolveReturnsIsPublicBasedOnNestedGrandparentClassScopeForField(
            string accessors,
            bool expected)
        {
            var code = @"
namespace MyNamespace 
{
    class Grandparent
    {
        public class Parent
        {
            public class MyClass
            {
                public string MyItem;
            }   
        }
    }
}
";

            code = code.Replace("class Grandparent", accessors + " class Grandparent", StringComparison.Ordinal);

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
        public async Task ResolveReturnsIsPublicBasedOnNestedGrandparentClassScopeForInterfaceProperty(
            string accessors,
            bool expected)
        {
            var code = @"
namespace MyNamespace 
{
    class Grandparent
    {
        public class Parent
        {
            public interface MyInterface
            {
                string MyItem { get; set; }
            }   
        }
    }
}
";

            code = code.Replace("class Grandparent", accessors + " class Grandparent", StringComparison.Ordinal);

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
        public async Task ResolveReturnsIsPublicBasedOnNestedParentClassScopeForClassProperty(
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
        public async Task ResolveReturnsIsPublicBasedOnNestedParentClassScopeForInterfaceProperty(
            string accessors,
            bool expected)
        {
            var code = @"
namespace MyNamespace 
{
    class Parent
    {
        public interface MyInterface
        {
            string MyItem { get; set; }
        }   
    }
}
";

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
        public async Task ResolveReturnsIsPublicBasedOnParentClassScopeForClassProperty(string accessors, bool expected)
        {
            var code = TestNode.ClassProperty.Replace("public class MyClass",
                accessors + " class MyClass",
                StringComparison.Ordinal);

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
            var code = TestNode.Field.Replace("public class MyClass",
                accessors + " class MyClass",
                StringComparison.Ordinal);

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
        public async Task ResolveReturnsIsPublicBasedOnParentInterfaceScopeForInterfaceProperty(
            string accessors,
            bool expected)
        {
            var code = TestNode.InterfaceProperty.Replace("public interface MyInterface",
                accessors + " interface MyInterface",
                StringComparison.Ordinal);

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
            var code = TestNode.Field.Replace("public string MyField",
                accessors + " string MyField",
                StringComparison.Ordinal);

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
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                accessors + " string MyProperty",
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
            var code = TestNode.Field.Replace("public string MyField",
                fragment + " public string MyField",
                StringComparison.Ordinal);

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
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                fragment + " public string MyProperty",
                StringComparison.Ordinal);

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
            var code = TestNode.Field.Replace("public string MyField",
                fragment + " public string MyField",
                StringComparison.Ordinal);

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
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                fragment + " public string MyProperty",
                StringComparison.Ordinal);

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
        public async Task ResolveReturnsNestedGrandparentOwningTypeForClassProperty()
        {
            var code = @"
public class Grandparent
{
    public class Parent
    {
        public class MyClass
        {
            string MyItem { get; set; }
        }   
    }
}";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("Grandparent+Parent+MyClass");
        }

        [Fact]
        public async Task ResolveReturnsNestedGrandparentOwningTypeForField()
        {
            var code = @"
public class Grandparent
{
    public class Parent
    {
        public class MyClass
        {
            string MyItem;
        }
    }
}";
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("Grandparent+Parent+MyClass");
        }

        [Fact]
        public async Task ResolveReturnsNestedGrandparentOwningTypeForInterfaceProperty()
        {
            var code = @"
public class Grandparent
{
    public class Parent
    {
        public interface MyInterface
        {
            string MyItem { get; set; }
        }   
    }
}";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("Grandparent+Parent+MyInterface");
        }

        [Fact]
        public async Task ResolveReturnsNestedOwningTypeForClassProperty()
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

        [Fact]
        public async Task ResolveReturnsNestedOwningTypeForField()
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
        public async Task ResolveReturnsNestedOwningTypeForInterfaceProperty()
        {
            var code = @"
public class Parent
{
    public interface MyInterface
    {
        string MyItem { get; set; }
    }   
}";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("Parent+MyInterface");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeAndNamespaceForClassProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.ClassProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().Be("MyNamespace");
            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeAndNamespaceForField()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(TestNode.Field).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().Be("MyNamespace");
            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeAndNamespaceForInterfaceProperty()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.InterfaceProperty)
                .ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().Be("MyNamespace");
            actual.OwningType.Should().Be("MyInterface");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeForClassPropertyDefinition()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.ClassProperty).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeForFieldDefinition()
        {
            var node = await TestNode.FindNode<FieldDeclarationSyntax>(TestNode.Field).ConfigureAwait(false);

            var sut = new FieldResolver();

            var actual = sut.Resolve(node);

            actual.OwningType.Should().Be("MyClass");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeForInterfacePropertyDefinition()
        {
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(TestNode.InterfaceProperty)
                .ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = (PropertyDefinition) sut.Resolve(node);

            actual.OwningType.Should().Be("MyInterface");
        }

        [Fact]
        public async Task ResolveReturnsOwningTypeWithoutNamespaceForClassProperty()
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
        public async Task ResolveReturnsOwningTypeWithoutNamespaceForField()
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
        public async Task ResolveReturnsOwningTypeWithoutNamespaceForInterfaceProperty()
        {
            var code = @"
public interface MyInterface
{
    string MyItem { get; set; }
}   
";
            var node = await TestNode.FindNode<PropertyDeclarationSyntax>(code).ConfigureAwait(false);

            var sut = new PropertyResolver();

            var actual = sut.Resolve(node);

            actual.Namespace.Should().BeNull();
            actual.OwningType.Should().Be("MyInterface");
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
            var code = TestNode.Field.Replace("public string MyField",
                fragment + " public string MyField",
                StringComparison.Ordinal);

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
            var code = TestNode.ClassProperty.Replace("public string MyProperty",
                fragment + " public string MyProperty",
                StringComparison.Ordinal);

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

            Action action = () => resolver.Resolve(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}