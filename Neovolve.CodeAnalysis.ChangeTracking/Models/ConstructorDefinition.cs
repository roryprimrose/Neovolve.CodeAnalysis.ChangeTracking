namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ConstructorDefinition : MemberDefinition, IConstructorDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the field.</param>
        /// <param name="node">The node that defines the argument.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public ConstructorDefinition(ITypeDefinition declaringType, ConstructorDeclarationSyntax node) : base(
            node,
            declaringType)
        {
            Modifiers = DetermineModifiers(node);

            string name = BuildName(node);

            Name = name;
            RawName = name;
            FullName = DeclaringType.FullName + "." + name;
            FullRawName = DeclaringType.FullRawName + "." + name;
            ReturnType = string.Empty;

            Parameters = DetermineParameters(node);
        }

        private static ConstructorModifiers DetermineModifiers(ConstructorDeclarationSyntax node)
        {
            if (node.Modifiers.HasModifier(SyntaxKind.StaticKeyword))
            {
                return ConstructorModifiers.Static;
            }

            return ConstructorModifiers.None;
        }

        private string BuildName(ConstructorDeclarationSyntax node)
        {
            var name = Modifiers == ConstructorModifiers.Static ? "cctor" : "ctor";

            name += "(";

            var parameterList = string.Empty;

            // We can't reference the parameters list here because it hasn't been built by the time we need to calculate the name
            // The reason for this is because building the parameter models needs a reference to the constructor name which we are building here
            foreach (var parameter in node.ParameterList.Parameters)
            {
                if (parameterList.Length > 0)
                {
                    parameterList += ", ";
                }

                parameterList += parameter.Type?.ToString() ?? parameter.Identifier.Text;
            }

            name += parameterList + ")";

            return name;
        }

        /// <inheritdoc />
        public override string FullName { get; }

        /// <inheritdoc />
        public override string FullRawName { get; }

        public ConstructorModifiers Modifiers { get; }

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