namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class PropertyResolver : MemberResolver, INodeResolver
    {
        public bool IsSupported(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            return node is PropertyDeclarationSyntax;
        }

        public NodeDefinition Resolve(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));
            Ensure.Type.IsOfType(node, typeof(PropertyDeclarationSyntax), nameof(node));

            var propertySyntax = (PropertyDeclarationSyntax) node;

            var property = Resolve<PropertyDefinition>(propertySyntax);

            property.Name = propertySyntax.Identifier.Text;
            property.ReturnType = propertySyntax.Type.ToString();

            var getAccessor =
                propertySyntax.AccessorList.Accessors.FirstOrDefault(x =>
                    x.Kind() == SyntaxKind.GetAccessorDeclaration);

            if (getAccessor != null)
            {
                if (getAccessor.Modifiers.Count == 0
                    || getAccessor.Modifiers.Any(x => x.Text == "public"))
                {
                    property.CanRead = true;
                }
            }

            var setAccessor =
                propertySyntax.AccessorList.Accessors.FirstOrDefault(x =>
                    x.Kind() == SyntaxKind.SetAccessorDeclaration);

            if (setAccessor != null)
            {
                if (setAccessor.Modifiers.Count == 0
                    || setAccessor.Modifiers.Any(x => x.Text == "public"))
                {
                    property.CanWrite = true;
                }
            }

            return property;
        }

        public bool EvaluateChildren { get; } = false;
    }
}