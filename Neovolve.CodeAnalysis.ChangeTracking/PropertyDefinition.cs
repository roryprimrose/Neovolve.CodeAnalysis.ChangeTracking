namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="ConstraintListDefinition" />
    ///     class is used to describe a property.
    /// </summary>
    public class PropertyDefinition : IMemberDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDeclarationSyntax" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the property.</param>
        /// <param name="node">The node that defines the generic type constraints.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="declaringType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public PropertyDefinition(IMemberDefinition declaringType, PropertyDeclarationSyntax node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            Name = node.Identifier.Text;
            FullName = declaringType.FullName + "." + Name;

            Attributes = node.DetermineAttributes(this);
            Location = node.DetermineLocation();
            ReturnType = node.Type.ToString();
            IsVisible = node.IsVisible();
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

            if (accessor.IsVisible())
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<AttributeDefinition> Attributes { get; }

        /// <summary>
        ///     Gets whether the property declares a getter.
        /// </summary>
        public bool CanRead { get; }

        /// <summary>
        ///     Gets whether the property declares a setter.
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        ///     Gets the type that declares the property.
        /// </summary>
        public IMemberDefinition DeclaringType { get; }

        /// <inheritdoc />
        public string FullName { get; }

        /// <inheritdoc />
        public bool IsVisible { get; }

        /// <inheritdoc />
        public DefinitionLocation Location { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        ///     Gets the return type of the member.
        /// </summary>
        public string ReturnType { get; }
    }
}