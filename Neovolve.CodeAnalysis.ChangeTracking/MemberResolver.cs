namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
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

            if (containerNamespace != null)
            {
                // This class is defined in a namespace
                var namespaceIdentifier = (IdentifierNameSyntax)containerNamespace.Name;

                node.Namespace = namespaceIdentifier.Identifier.Text;
            }
            
            var parentClasses = new List<ClassDeclarationSyntax>();

            while (parentClass != null)
            {
                parentClasses.Add(parentClass);
                
                var parent = parentClass;

                parentClass = parentClass.FirstAncestorOrSelf<ClassDeclarationSyntax>(x => x != parent);
            }
            
            var classNameHierarchy = parentClasses.Select(x => x.Identifier.Text).Reverse();

            node.OwningType = string.Join("+", classNameHierarchy);

            var memberIsPublic = member.Modifiers.Any(x => x.Text == "public");

            if (memberIsPublic == false)
            {
                node.IsPublic = false;
            }
            else
            {
                // Check if any nest parent class is not public
                var parentClassesArePublic = parentClasses.All(x => x.Modifiers.Any(y => y.Text == "public"));

                node.IsPublic = parentClassesArePublic;
            }
        }
    }
}