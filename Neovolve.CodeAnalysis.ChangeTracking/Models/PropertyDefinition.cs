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
    public class PropertyDefinition : MemberDefinition, IPropertyDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDeclarationSyntax" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the property.</param>
        /// <param name="node">The node that defines the generic type constraints.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="declaringType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public PropertyDefinition(ITypeDefinition declaringType, PropertyDeclarationSyntax node) : base(node,
            declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            var name = node.Identifier.Text;

            Modifiers = DetermineModifiers(node);
            ReturnType = node.Type.ToString();
            Name = name;
            RawName = name;
            FullName = DeclaringType.FullName + "." + name;
            FullRawName = DeclaringType.FullRawName + "." + name;

            GetAccessor = DetermineAccessor(this, node, SyntaxKind.GetAccessorDeclaration);
            SetAccessor = DetermineAccessor(this, node, SyntaxKind.SetAccessorDeclaration);
        }

        private static IPropertyAccessorDefinition? DetermineAccessor(PropertyDefinition propertyDefinition,
            PropertyDeclarationSyntax node, SyntaxKind accessorType)
        {
            var accessorNode = FindAccessor(node, accessorType);

            if (accessorNode == null)
            {
                return null;
            }

            return new PropertyAccessorDefinition(propertyDefinition, accessorNode);
        }

        private static PropertyModifiers DetermineModifiers(PropertyDeclarationSyntax node)
        {
            var isVirtual = node.Modifiers.HasModifier(SyntaxKind.VirtualKeyword);
            var isAbstract = node.Modifiers.HasModifier(SyntaxKind.AbstractKeyword);
            var isNew = node.Modifiers.HasModifier(SyntaxKind.NewKeyword);
            var isOverride = node.Modifiers.HasModifier(SyntaxKind.OverrideKeyword);
            var isStatic = node.Modifiers.HasModifier(SyntaxKind.StaticKeyword);
            var isSealed = node.Modifiers.HasModifier(SyntaxKind.SealedKeyword);

            if (isNew)
            {
                if (isAbstract)
                {
                    if (isVirtual)
                    {
                        return PropertyModifiers.NewAbstractVirtual;
                    }

                    return PropertyModifiers.NewAbstract;
                }

                if (isStatic)
                {
                    return PropertyModifiers.NewStatic;
                }

                if (isVirtual)
                {
                    return PropertyModifiers.NewVirtual;
                }

                return PropertyModifiers.New;
            }

            if (isAbstract)
            {
                if (isOverride)
                {
                    return PropertyModifiers.AbstractOverride;
                }

                return PropertyModifiers.Abstract;
            }

            if (isOverride)
            {
                if (isSealed)
                {
                    return PropertyModifiers.SealedOverride;
                }

                return PropertyModifiers.Override;
            }

            if (isSealed)
            {
                return PropertyModifiers.Sealed;
            }

            if (isStatic)
            {
                return PropertyModifiers.Static;
            }

            if (isVirtual)
            {
                return PropertyModifiers.Virtual;
            }

            return PropertyModifiers.None;
        }

        private static AccessorDeclarationSyntax? FindAccessor(PropertyDeclarationSyntax node, SyntaxKind accessorType)
        {
            return node.AccessorList?.Accessors.FirstOrDefault(x => x.Kind() == accessorType);
        }

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public IPropertyAccessorDefinition? GetAccessor { get; }

        /// <inheritdoc />
        public PropertyModifiers Modifiers { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public override string RawName { get; }

        /// <inheritdoc />
        public override string ReturnType { get; }

        /// <inheritdoc />
        public IPropertyAccessorDefinition? SetAccessor { get; }
    }
}