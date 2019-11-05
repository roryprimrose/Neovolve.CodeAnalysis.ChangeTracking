namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberResolver
    {
        protected virtual T Resolve<T>(MemberDeclarationSyntax node) where T : NodeDefinition, new()
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var definition = new T();

            ResolveMemberInfo(node, definition);
            ResolveAttributes(node, definition);

            return definition;
        }

        private static void ResolveAttributes(MemberDeclarationSyntax propertySyntax, NodeDefinition property)
        {
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
        }

        private static void ResolveMemberInfo(MemberDeclarationSyntax propertySyntax, NodeDefinition property)
        {
            var parentClass = propertySyntax.Parent as ClassDeclarationSyntax;
            var containerNamespace = parentClass?.Parent as NamespaceDeclarationSyntax;
            var namespaceIdentifier = containerNamespace?.Name as IdentifierNameSyntax;

            property.OwningType = parentClass?.Identifier.Text;
            property.Namespace = namespaceIdentifier?.Identifier.Text;
            property.IsPublic = propertySyntax.Modifiers.Any(x => x.Text == "public");
        }
    }
}