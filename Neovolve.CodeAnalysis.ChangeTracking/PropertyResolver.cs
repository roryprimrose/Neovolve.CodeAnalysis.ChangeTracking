namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class PropertyResolver : INodeResolver
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

            var propertySyntax = node as PropertyDeclarationSyntax;

            var parentClass = propertySyntax.Parent as ClassDeclarationSyntax;
            var containerNamespace = parentClass?.Parent as NamespaceDeclarationSyntax;
            var namespaceIdentifier = containerNamespace?.Name as IdentifierNameSyntax;

            var property = new PropertyDefinition
            {
                Name = propertySyntax.Identifier.Text,
                ClassName = parentClass?.Identifier.Text,
                Namespace = namespaceIdentifier?.Identifier.Text,
                IsPublic = propertySyntax.Modifiers.Any(x => x.Text == "public"),
                DataType = propertySyntax.Type.ToString()
            };

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

            foreach (var attributeList in propertySyntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeList.Attributes)
                {
                    var attribute = new AttributeDefinition
                    {
                        Name = attributeSyntax.Name.GetText().ToString(),
                        Declaration = attributeSyntax.GetText().ToString()
                    };

                    property.Attributes.Add(attribute);
                }
            }

            return property;
        }

        public bool EvaluateChildren { get; } = false;
    }
}