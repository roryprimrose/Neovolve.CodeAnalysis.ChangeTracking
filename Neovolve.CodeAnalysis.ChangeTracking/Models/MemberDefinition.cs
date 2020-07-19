namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberDefinition : ElementDefinition, IMemberDefinition
    {
        protected MemberDefinition(MemberDeclarationSyntax node, ITypeDefinition declaringType) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            AccessModifier = DetermineAccessModifier(node, DeclaringType);

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

        private static AccessModifier DetermineAccessModifier(MemberDeclarationSyntax node,
            ITypeDefinition declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            // See default values as identified at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/accessibility-levels
            if (declaringType is IInterfaceDefinition)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifier.Public);
            }

            if (declaringType is IClassDefinition)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifier.Private);
            }

            // TODO: Fill these out when the types are supported
            throw new NotSupportedException();

            // Struct default access modifier is private
            // Struct default enum is public
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