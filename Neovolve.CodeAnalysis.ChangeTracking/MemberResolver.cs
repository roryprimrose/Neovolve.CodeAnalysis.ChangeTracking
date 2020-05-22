namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberResolver
    {
        protected virtual T Resolve<T>(MemberDeclarationSyntax member) where T : MemberDefinition, new()
        {
            Ensure.Any.IsNotNull(member, nameof(member));

            var node = new T();

            ResolveLocation(member, node);
            ResolveDeclarationInfo(member, node);
            ResolveAttributes(member, node);

            return node;
        }

        protected virtual AttributeDefinition ResolveAttribute(AttributeSyntax attributeSyntax)
        {
            Ensure.Any.IsNotNull(attributeSyntax, nameof(attributeSyntax));

            var name = attributeSyntax.Name.GetText().ToString();
            string? namespaceIdentifier = null;

            if (name.Contains(".", StringComparison.Ordinal))
            {
                namespaceIdentifier = name.Substring(0, name.LastIndexOf(".", StringComparison.Ordinal));
                name = name.Substring(name.LastIndexOf(".", StringComparison.Ordinal) + 1);
            }

            var typeName = name;

            if (name.EndsWith("Attribute", StringComparison.Ordinal) == false)
            {
                typeName += "Attribute";
            }
            else
            {
                // Strip the attribute identifier from the name
                name = name[..^9];
            }

            var attribute = new AttributeDefinition
            {
                Namespace = namespaceIdentifier,
                OwningType = typeName,
                Name = name,
                MemberType = MemberType.Attribute,

                // We will assume that the attribute is public. It would be very unusual if it wasn't
                IsPublic = true,
                Declaration = attributeSyntax.GetText().ToString()
            };

            return attribute;
        }

        private static List<ClassDeclarationSyntax> FindParentClasses(MemberDeclarationSyntax declaration)
        {
            var parentClasses = new List<ClassDeclarationSyntax>();

            var parentClass = declaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();

            while (parentClass != null)
            {
                parentClasses.Add(parentClass);

                var parent = parentClass;

                parentClass = parentClass.FirstAncestorOrSelf<ClassDeclarationSyntax>(x => x != parent);
            }

            return parentClasses;
        }

        private static void ResolveDeclarationInfo(MemberDeclarationSyntax declaration, MemberDefinition member)
        {
            var containerNamespace = declaration.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

            if (containerNamespace != null)
            {
                member.Namespace = containerNamespace.Name.GetText().ToString().Trim();
            }

            var parentInterface = declaration.FirstAncestorOrSelf<InterfaceDeclarationSyntax>();

            var parentClasses = FindParentClasses(declaration);

            var classNameHierarchy = parentClasses.Select(x => x.Identifier.Text).Reverse();
            var classTypeName = string.Join("+", classNameHierarchy);

            if (parentInterface != null)
            {
                var interfaceTypeName = parentInterface.Identifier.Text;

                if (string.IsNullOrWhiteSpace(classTypeName))
                {
                    member.OwningType = interfaceTypeName;
                }
                else
                {
                    member.OwningType = classTypeName + "+" + interfaceTypeName;
                }
            }
            else
            {
                member.OwningType = classTypeName;
            }

            if (parentInterface != null)
            {
                // The property is declared on an interface
                // Interface properties don't have modifiers so we will check the scope of the parent interface
                // then fall down to checking all parents in the class hierarchy
                member.IsPublic = parentInterface.Modifiers.Any(x => x.Text == "public");
            }
            else
            {
                // This property is declared on a class
                // First check the scope of the member, then fall down to checking all parents in the class hierarchy
                member.IsPublic = declaration.Modifiers.Any(x => x.Text == "public");
            }

            if (member.IsPublic)
            {
                // Either the parent interface or class member is public
                // Check if any parent class is not public
                var parentClassesArePublic = parentClasses.All(x => x.Modifiers.Any(y => y.Text == "public"));

                member.IsPublic = parentClassesArePublic;
            }
        }

        private static void ResolveLocation(MemberDeclarationSyntax declaration, MemberDefinition member)
        {
            var location = declaration.GetLocation();

            if (location.IsInSource
                && location.Kind == LocationKind.SourceFile)
            {
                member.FilePath = location.SourceTree?.FilePath ?? string.Empty;
            }

            var startPosition = location.GetLineSpan().StartLinePosition;

            member.LineIndex = startPosition.Line;
            member.CharacterIndex = startPosition.Character;
        }

        private void ResolveAttributes(MemberDeclarationSyntax declaration, MemberDefinition member)
        {
            foreach (var attributeList in declaration.AttributeLists)
            {
                foreach (var attributeSyntax in attributeList.Attributes)
                {
                    var attribute = ResolveAttribute(attributeSyntax);

                    member.Attributes.Add(attribute);
                }
            }
        }
    }
}