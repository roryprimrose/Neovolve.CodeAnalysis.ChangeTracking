namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberDefinition : ElementDefinition, IMemberDefinition
    {
        protected MemberDefinition(MemberDeclarationSyntax node, ITypeDefinition declaringType) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            AccessModifier = node.DetermineAccessModifier(DeclaringType);

            if (declaringType.IsVisible == false)
            {
                IsVisible = false;
            }
            else
            {
                // Determine visibility based on the access modifier
                IsVisible = AccessModifier.IsVisible();
            }
        }

        /// <inheritdoc />
        public AccessModifier AccessModifier { get; }

        /// <inheritdoc />
        public ITypeDefinition DeclaringType { get; }

        /// <inheritdoc />
        public override bool IsVisible { get; }

        /// <inheritdoc />
        public abstract string ReturnType { get; }
    }
}