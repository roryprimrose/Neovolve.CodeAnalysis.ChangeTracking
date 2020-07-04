namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberDefinition : ElementDefinition, IMemberDefinition
    {
        protected MemberDefinition(ITypeDefinition declaringType, MemberDeclarationSyntax node) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            AccessModifier = node.DetermineAccessModifier(DeclaringType);
        }

        /// <inheritdoc />
        public AccessModifier AccessModifier { get; }

        /// <inheritdoc />
        public ITypeDefinition DeclaringType { get; }

        /// <inheritdoc />
        public abstract string ReturnType { get; }
    }
}