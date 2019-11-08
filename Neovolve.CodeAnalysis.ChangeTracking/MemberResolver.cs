namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberResolver
    {
        protected virtual T Resolve<T>(MemberDeclarationSyntax member) where T : NodeDefinition, new()
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            var node = new T();

            ResolveMemberInfo(member, node);
            ResolveAttributes(member, node);

            return node;
        }

        private static void ResolveAttributes(MemberDeclarationSyntax member, NodeDefinition node)
        {
            foreach (var attributeList in member.AttributeLists)
            {
                foreach (var attributeSyntax in attributeList.Attributes)
                {
                    var attribute = new AttributeDefinition
                    {
                        Name = attributeSyntax.Name.GetText().ToString(),
                        Declaration = attributeSyntax.GetText().ToString()
                    };

                    node.Attributes.Add(attribute);
                }
            }
        }

        private static void ResolveMemberInfo(MemberDeclarationSyntax member, NodeDefinition node)
        {
            var parentClass = member.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            var containerNamespace = parentClass.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
            var namespaceIdentifier = (IdentifierNameSyntax)containerNamespace.Name;

            node.OwningType = parentClass.Identifier.Text;
            node.Namespace = namespaceIdentifier.Identifier.Text;
            node.IsPublic = member.Modifiers.Any(x => x.Text == "public");
        }
    }
}