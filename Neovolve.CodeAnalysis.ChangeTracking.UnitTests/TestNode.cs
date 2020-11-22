namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class TestNode
    {
        public const string ClassProperty = @"
namespace MyNamespace 
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

        public const string MultipleClasses = @"
namespace MyNamespace 
{
    public class FirstClass
    {
    }   

    public class SecondClass
    {
    }   
}
";

        public const string MultipleStructs = @"
namespace MyNamespace 
{
    public struct FirstStruct
    {
    }   

    public struct SecondStruct
    {
    }   
}
";

        public const string MultipleInterfaces = @"
namespace MyNamespace 
{
    public interface FirstInterface
    {
    }   

    public interface SecondInterface
    {
    }   
}
";

        public const string Field = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string MyField;
    }   
}
";

        public const string InterfaceProperty = @"
namespace MyNamespace 
{
    public interface MyInterface
    {
        string MyProperty
        {
            get;
            set;
        }
    }   
}
";

        public static async Task<T> FindNode<T>(string code, string filePath = "") where T : SyntaxNode
        {
            var root = await Parse(code, filePath).ConfigureAwait(false);

            var node = FindNode<T>(root);

            if (node == null)
            {
                throw new InvalidOperationException("Failed to find node");
            }

            return node;
        }

        public static T? FindNode<T>(SyntaxNode node) where T : SyntaxNode
        {
            if (node is T syntaxNode)
            {
                return syntaxNode;
            }

            return node.ChildNodes().Select(FindNode<T>).FirstOrDefault(nodeFound => nodeFound != null);
        }

        public static Task<SyntaxNode> Parse(string code, string filePath = "")
        {
            var tree = CSharpSyntaxTree.ParseText(code, null, filePath);

            return tree.GetRootAsync();
        }
    }
}