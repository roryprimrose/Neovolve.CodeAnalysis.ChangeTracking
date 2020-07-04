namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ConstraintListDefinition" />
    ///     class is used to describe a property.
    /// </summary>
    public class PropertyDefinition : ElementDefinition, IPropertyDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDeclarationSyntax" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the property.</param>
        /// <param name="node">The node that defines the generic type constraints.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="declaringType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public PropertyDefinition(ITypeDefinition declaringType, PropertyDeclarationSyntax node) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            node = node ?? throw new ArgumentNullException(nameof(node));

            var name = node.Identifier.Text;

            AccessModifier = node.DetermineAccessModifier();
            Name = name;
            RawName = name;
            FullName = DeclaringType.FullName + "." + name;
            FullRawName = DeclaringType.FullRawName + "." + name;

            ReturnType = node.Type.ToString();
            IsAbstract = node.HasModifier(SyntaxKind.AbstractKeyword);
            IsNew = node.HasModifier(SyntaxKind.NewKeyword);
            IsOverride = node.HasModifier(SyntaxKind.OverrideKeyword);
            IsSealed = node.HasModifier(SyntaxKind.SealedKeyword);
            IsStatic = node.HasModifier(SyntaxKind.StaticKeyword);
            IsVirtual = node.HasModifier(SyntaxKind.VirtualKeyword);
            CanRead = HasVisibleAccessor(node, IsVisible, SyntaxKind.GetAccessorDeclaration);
            CanWrite = HasVisibleAccessor(node, IsVisible, SyntaxKind.SetAccessorDeclaration);
        }

        private static bool HasVisibleAccessor(PropertyDeclarationSyntax node,
            bool propertyIsVisible,
            SyntaxKind accessorType)
        {
            if (propertyIsVisible == false)
            {
                // The property itself is not visible so as far as public consumption is concerned, we can't read or write the property
                return false;
            }

            var accessor =
                node.AccessorList?.Accessors.FirstOrDefault(x =>
                    x.Kind() == accessorType);

            if (accessor == null)
            {
                return false;
            }

            if (accessor.Modifiers.Count == 0)
            {
                return propertyIsVisible;
            }

            // Need to evaluate the actual access modifiers on the property accessor to determine the difference between Feature and Breaking
            if (accessor.IsVisible())
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public AccessModifier AccessModifier { get; }

        /// <inheritdoc />
        public bool CanRead { get; }

        /// <inheritdoc />
        public bool CanWrite { get; }

        /// <inheritdoc />
        public ITypeDefinition DeclaringType { get; }

        /// <inheritdoc />
        public override string Description => $"Property {FullName}";

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        public bool IsAbstract { get; }

        /// <inheritdoc />
        public bool IsNew { get; }

        /// <inheritdoc />
        public bool IsOverride { get; }

        /// <inheritdoc />
        public bool IsSealed { get; }

        /// <inheritdoc />
        public bool IsStatic { get; }

        /// <inheritdoc />
        public bool IsVirtual { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override string RawName { get; }

        /// <inheritdoc />
        public string ReturnType { get; }
    }
}