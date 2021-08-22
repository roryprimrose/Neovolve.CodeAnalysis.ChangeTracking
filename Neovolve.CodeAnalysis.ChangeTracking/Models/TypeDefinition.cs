namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="TypeDefinition" />
    ///     class is used to describe a type.
    /// </summary>
    public abstract class TypeDefinition : BaseTypeDefinition<AccessModifiers>, ITypeDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(TypeDeclarationSyntax node) : base(node)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            AccessModifiers = DetermineAccessModifier(node, DeclaringType);
            IsVisible = DetermineIsVisible(node, DeclaringType);

            ImplementedTypes = DetermineImplementedTypes(node);
            Properties = DetermineProperties(node);
            Methods = DetermineMethods(node);
            ChildClasses = DetermineChildClasses(node);
            ChildEnums = DetermineChildEnums(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            ChildStructs = DetermineChildStructs(node);
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces, ChildStructs, ChildEnums);
            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The parent type that declares this type.</param>
        /// <param name="node">The syntax node that defines the type.</param>
        protected TypeDefinition(ITypeDefinition declaringType, TypeDeclarationSyntax node) : base(declaringType, node)
        {
            AccessModifiers = DetermineAccessModifier(node, DeclaringType);
            IsVisible = DetermineIsVisible(node, DeclaringType);

            ImplementedTypes = DetermineImplementedTypes(node);
            Properties = DetermineProperties(node);
            Methods = DetermineMethods(node);
            ChildClasses = DetermineChildClasses(node);
            ChildEnums = DetermineChildEnums(node);
            ChildInterfaces = DetermineChildInterfaces(node);
            ChildStructs = DetermineChildStructs(node);
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces, ChildStructs, ChildEnums);
            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);
        }

        /// <inheritdoc />
        public virtual void MergePartialType(ITypeDefinition partialType)
        {
            partialType = partialType ?? throw new ArgumentNullException(nameof(partialType));

            if (GetType() != partialType.GetType())
            {
                throw new InvalidOperationException(
                    $"Unable to merge partial {partialType.GetType().FullName} type into {GetType().FullName}");
            }

            if (FullName != partialType.FullName)
            {
                throw new InvalidOperationException(
                    $"Unable to merge partial type {partialType.FullName} into {FullName}");
            }

            Attributes = Attributes.Union(partialType.Attributes).ToList().AsReadOnly();

            Methods = MergeMembers(Methods, partialType.Methods);
            Properties = MergeMembers(Properties, partialType.Properties);
            ChildClasses = MergeTypes(ChildClasses, partialType.ChildClasses);
            ChildEnums = MergeTypes(ChildEnums, partialType.ChildEnums);
            ChildInterfaces = MergeTypes(ChildInterfaces, partialType.ChildInterfaces);
            ChildStructs = MergeTypes(ChildStructs, partialType.ChildStructs);

            // Rebuild the child types
            ChildTypes = DetermineChildTypes(ChildClasses, ChildInterfaces, ChildStructs, ChildEnums);
        }

        /// <summary>
        ///     Gets the fields that are declared on the node.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>The fields that are declared on the node.</returns>
        protected IReadOnlyCollection<ConstructorDefinition> DetermineConstructors(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<ConstructorDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new ConstructorDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        /// <summary>
        ///     Gets the fields that are declared on the node.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>The fields that are declared on the node.</returns>
        protected IReadOnlyCollection<FieldDefinition> DetermineFields(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<FieldDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new FieldDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        protected IReadOnlyCollection<T> MergeMembers<T>(
            IReadOnlyCollection<T> currentMembers,
            IReadOnlyCollection<T> incomingMembers) where T : class, IMemberDefinition
        {
            var members = new List<T>(currentMembers);

            foreach (var incomingMember in incomingMembers)
            {
                incomingMember.DeclaringType = this;

                members.Add(incomingMember);
            }

            return members.AsReadOnly();
        }

        private static AccessModifiers DetermineAccessModifier(BaseTypeDeclarationSyntax node,
            ITypeDefinition? declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));

            if (declaringType == null)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifiers.Internal);
            }

            return node.Modifiers.DetermineAccessModifier(AccessModifiers.Private);
        }

        private static IReadOnlyCollection<IBaseTypeDefinition> DetermineChildTypes(
            IReadOnlyCollection<ITypeDefinition> childClasses,
            IReadOnlyCollection<ITypeDefinition> childInterfaces, IReadOnlyCollection<IStructDefinition> childStructs,
            IReadOnlyCollection<IEnumDefinition> childEnums)
        {
            var childTypes = new List<IBaseTypeDefinition>(childClasses);

            childTypes.AddRange(childInterfaces);
            childTypes.AddRange(childStructs);
            childTypes.AddRange(childEnums);

            return childTypes;
        }

        private static IReadOnlyCollection<IConstraintListDefinition> DetermineGenericConstraints(
            TypeDeclarationSyntax node)
        {
            var constraintLists = new List<ConstraintListDefinition>();

            foreach (var clauses in node.ConstraintClauses)
            {
                var constraintList = new ConstraintListDefinition(clauses);

                constraintLists.Add(constraintList);
            }

            return constraintLists.AsReadOnly();
        }

        private static IReadOnlyCollection<string> DetermineGenericTypeParameters(TypeDeclarationSyntax node)
        {
            var typeParameters = new List<string>();

            if (node.TypeParameterList == null)
            {
                return typeParameters;
            }

            foreach (var typeParameter in node.TypeParameterList.Parameters)
            {
                typeParameters.Add(typeParameter.Identifier.Text);
            }

            return typeParameters;
        }

        private static IReadOnlyCollection<string> DetermineImplementedTypes(BaseTypeDeclarationSyntax node)
        {
            var baseList = node.ChildNodes().OfType<BaseListSyntax>().FirstOrDefault();

            if (baseList == null)
            {
                return Array.Empty<string>();
            }

            var childTypes = baseList.Types.Select(x => x.ToString()).FastToList();

            return childTypes.AsReadOnly();
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

        private IReadOnlyCollection<IClassDefinition> DetermineChildClasses(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<ClassDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new ClassDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IEnumDefinition> DetermineChildEnums(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<EnumDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new EnumDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IInterfaceDefinition> DetermineChildInterfaces(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<InterfaceDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new InterfaceDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IStructDefinition> DetermineChildStructs(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<StructDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new StructDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IMethodDefinition> DetermineMethods(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<MethodDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new MethodDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<IPropertyDefinition> DetermineProperties(SyntaxNode node)
        {
            var childNodes = node.ChildNodes().OfType<PropertyDeclarationSyntax>();
            var childTypes = childNodes.Select(childNode => new PropertyDefinition(this, childNode)).FastToList();

            return childTypes.AsReadOnly();
        }

        private IReadOnlyCollection<T> MergeTypes<T>(
            IReadOnlyCollection<T> currentTypes,
            IReadOnlyCollection<T> incomingTypes) where T : class, IBaseTypeDefinition
        {
            var types = new List<T>(currentTypes);

            foreach (var incomingType in incomingTypes)
            {
                incomingType.DeclaringType = this;

                types.Add(incomingType);
            }

            return types.AsReadOnly();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IClassDefinition> ChildClasses { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IEnumDefinition> ChildEnums { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IInterfaceDefinition> ChildInterfaces { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IStructDefinition> ChildStructs { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IBaseTypeDefinition> ChildTypes { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> GenericTypeParameters { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ImplementedTypes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IMethodDefinition> Methods { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IPropertyDefinition> Properties { get; private set; }
    }
}