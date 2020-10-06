namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MethodDefinition : MemberDefinition, IMethodDefinition
    {
        public MethodDefinition(ITypeDefinition declaringType, MethodDeclarationSyntax node) : base(node, declaringType)
        {
            var name = DetermineName(node);
            var rawName = DetermineRawName(node);

            Modifiers = DetermineModifiers(node);
            ReturnType = node.ReturnType.ToString();
            Name = name;
            RawName = rawName;
            FullName = DeclaringType.FullName + "." + name;
            FullRawName = DeclaringType.FullRawName + "." + rawName;

            GenericTypeParameters = DetermineGenericTypeParameters(node);
            GenericConstraints = DetermineGenericConstraints(node);
            Parameters = DetermineParameters(node);
        }

        private static IReadOnlyCollection<IConstraintListDefinition> DetermineGenericConstraints(
            MethodDeclarationSyntax node)
        {
            var constraintLists = new List<ConstraintListDefinition>();

            foreach (var clauses in node.ConstraintClauses)
            {
                var constraintList = new ConstraintListDefinition(clauses);

                constraintLists.Add(constraintList);
            }

            return constraintLists.AsReadOnly();
        }

        private static IReadOnlyCollection<string> DetermineGenericTypeParameters(MethodDeclarationSyntax node)
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

        private static MethodModifiers DetermineModifiers(MethodDeclarationSyntax node)
        {
            var value = MethodModifiers.None;

            if (node.Modifiers.HasModifier(SyntaxKind.AsyncKeyword))
            {
                value = value | MethodModifiers.Async;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.VirtualKeyword))
            {
                value = value | MethodModifiers.Virtual;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.AbstractKeyword))
            {
                value = value | MethodModifiers.Abstract;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.NewKeyword))
            {
                value = value | MethodModifiers.New;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.OverrideKeyword))
            {
                value = value | MethodModifiers.Override;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.StaticKeyword))
            {
                value = value | MethodModifiers.Static;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.SealedKeyword))
            {
                value = value | MethodModifiers.Sealed;
            }

            return value;
        }

        private static string DetermineName(MethodDeclarationSyntax node)
        {
            var name = DetermineRawName(node);

            var typeParameters = node.ChildNodes().OfType<TypeParameterListSyntax>().FirstOrDefault();

            if (typeParameters == null)
            {
                return name;
            }

            var parameterList = typeParameters.ToString();

            return name + parameterList;
        }

        private static string DetermineRawName(MethodDeclarationSyntax node)
        {
            var name = string.Empty;

            name += node.Identifier.Text;

            if (node.ExplicitInterfaceSpecifier != null)
            {
                name = node.ExplicitInterfaceSpecifier.Name + "." + name;
            }

            return name;
        }

        private IReadOnlyCollection<IParameterDefinition> DetermineParameters(MethodDeclarationSyntax node)
        {
            var parameters = new List<IParameterDefinition>();

            foreach (var declaredParameter in node.ParameterList.Parameters)
            {
                var parameter = new ParameterDefinition(this, declaredParameter);

                parameters.Add(parameter);
            }

            return parameters;
        }

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IConstraintListDefinition> GenericConstraints { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> GenericTypeParameters { get; }

        /// <inheritdoc />
        public MethodModifiers Modifiers { get; }

        /// <inheritdoc />
        public override string Name { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IParameterDefinition> Parameters { get; }

        /// <inheritdoc />
        public override string RawName { get; }

        /// <inheritdoc />
        public override string ReturnType { get; }
    }
}