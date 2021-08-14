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
    public class EnumDefinition : BaseTypeDefinition, IEnumDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumDefinition" /> class.
        /// </summary>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public EnumDefinition(EnumDeclarationSyntax node) : base(node)
        {
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
            Members = DetermineMembers(node);
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