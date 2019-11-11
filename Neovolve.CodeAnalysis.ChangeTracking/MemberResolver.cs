namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberResolver
    {
        protected virtual T Resolve<T>(MemberDeclarationSyntax member) where T : MemberDefinition, new()
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            var node = new T();

            ResolveDeclarationInfo(member, node);
            ResolveAttributes(member, node);

            return node;
        }

        private static void ResolveAttributes(MemberDeclarationSyntax declaration, MemberDefinition member)
        {
            foreach (var attributeList in declaration.AttributeLists)
            {
                foreach (var attributeSyntax in attributeList.Attributes)
                {
                    var attribute = new AttributeDefinition
                    {
                        Name = attributeSyntax.Name.GetText().ToString(),
                        Declaration = attributeSyntax.GetText().ToString()
                    };

                    member.Attributes.Add(attribute);
                }
            }
        }

        private static void ResolveDeclarationInfo(MemberDeclarationSyntax declaration, MemberDefinition member)
        {
            var parentClass = declaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            
            var containerNamespace = parentClass.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

            if (containerNamespace != null)
            {
                // This class is defined in a namespace
                var namespaceIdentifier = (IdentifierNameSyntax)containerNamespace.Name;

                member.Namespace = namespaceIdentifier.Identifier.Text;
            }
            
            var parentClasses = new List<ClassDeclarationSyntax>();

            while (parentClass != null)
            {
                parentClasses.Add(parentClass);
                
                var parent = parentClass;

                parentClass = parentClass.FirstAncestorOrSelf<ClassDeclarationSyntax>(x => x != parent);
            }
            
            var classNameHierarchy = parentClasses.Select(x => x.Identifier.Text).Reverse();

            member.OwningType = string.Join("+", classNameHierarchy);

            var memberIsPublic = declaration.Modifiers.Any(x => x.Text == "public");

            if (memberIsPublic == false)
            {
                member.IsPublic = false;
            }
            else
            {
                // Check if any nest parent class is not public
                var parentClassesArePublic = parentClasses.All(x => x.Modifiers.Any(y => y.Text == "public"));

                member.IsPublic = parentClassesArePublic;
            }
        }
    }
}