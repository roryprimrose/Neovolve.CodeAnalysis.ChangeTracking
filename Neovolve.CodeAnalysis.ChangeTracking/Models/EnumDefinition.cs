namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="EnumDefinition" />
    ///     class describes an enum on a type.
    /// </summary>
    public class EnumDefinition : BaseTypeDefinition<EnumAccessModifiers>, IEnumDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumDefinition" /> class.
        /// </summary>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public EnumDefinition(EnumDeclarationSyntax node) : base(node)
        {
            AccessModifiers = DetermineAccessModifier(node, DeclaringType);
            IsVisible = DetermineIsVisible(node, DeclaringType);

            Members = DetermineMembers(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the field.</param>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public EnumDefinition(ITypeDefinition declaringType, EnumDeclarationSyntax node) : base(declaringType, node)
        {
            AccessModifiers = DetermineAccessModifier(node, DeclaringType);
            IsVisible = DetermineIsVisible(node, DeclaringType);

            Members = DetermineMembers(node);
        }

        private static bool DetermineIsVisible(BaseTypeDeclarationSyntax node, ITypeDefinition? declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType != null
                && declaringType.IsVisible == false)
            {
                // The parent type is not visible so this one can't be either
                return false;
            }

            // This is either a top level type or the parent type is visible
            // Determine visibility based on the access modifiers
            var accessModifier = DetermineAccessModifier(node, declaringType);

            return accessModifier.IsVisible();
        }

        private static EnumAccessModifiers DetermineAccessModifier(BaseTypeDeclarationSyntax node,
            ITypeDefinition? declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType == null)
            {
                return node.Modifiers.DetermineAccessModifier(EnumAccessModifiers.Internal);
            }

            return node.Modifiers.DetermineAccessModifier(EnumAccessModifiers.Private);
        }

        private IReadOnlyCollection<IEnumMemberDefinition> DetermineMembers(EnumDeclarationSyntax node)
        {
            var childNodes = node.ChildNodes().OfType<EnumMemberDeclarationSyntax>();
            var members = new List<IEnumMemberDefinition>();

            foreach (var childNode in childNodes)
            {
                var member = new EnumMemberDefinition(this, childNode, members.Count);

                members.Add(member);
            }

            return members.AsReadOnly();
        }

        public IReadOnlyCollection<IEnumMemberDefinition> Members { get; }
    }
}