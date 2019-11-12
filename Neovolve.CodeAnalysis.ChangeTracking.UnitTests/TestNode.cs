namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class TestNode
    {
        public const string StandardField = @"
namespace MyNamespace 
{
    public class MyClass
    {
        string MyItem;
    }   
}
";

        public const string StandardProperty = @"
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

        public static async Task<T> FindNode<T>(string code) where T : SyntaxNode
        {
            var root = await Parse(code).ConfigureAwait(false);

            return FindNode<T>(root);
        }

        public static T FindNode<T>(SyntaxNode node) where T : SyntaxNode
        {
            if (node is T syntaxNode)
            {
                return syntaxNode;
            }

            return node.ChildNodes().Select(FindNode<T>).FirstOrDefault(nodeFound => nodeFound != null);
        }

        public static Task<SyntaxNode> Parse(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);

            return tree.GetRootAsync();
        }
    }
}