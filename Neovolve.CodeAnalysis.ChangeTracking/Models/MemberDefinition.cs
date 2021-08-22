﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public abstract class MemberDefinition : ElementDefinition, IMemberDefinition
    {
        protected MemberDefinition(MemberDeclarationSyntax node, ITypeDefinition declaringType) : base(node)
        {
            DeclaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            AccessModifiers = DetermineAccessModifier(node, DeclaringType);

            if (declaringType.IsVisible == false)
            {
                IsVisible = false;
            }
            else
            {
                // Determine visibility based on the access modifiers
                IsVisible = AccessModifiers.IsVisible();
            }
        }

        protected IReadOnlyCollection<IParameterDefinition> DetermineParameters(ConstructorDeclarationSyntax node)
        {
            var parameters = new List<IParameterDefinition>();

            foreach (var declaredParameter in node.ParameterList.Parameters)
            {
                var parameter = new ParameterDefinition(this, declaredParameter);

                parameters.Add(parameter);
            }

            return parameters;
        }

        private static AccessModifiers DetermineAccessModifier(MemberDeclarationSyntax node,
            ITypeDefinition declaringType)
        {
            node = node ?? throw new ArgumentNullException(nameof(node));
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));

            // See default values as identified at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/accessibility-levels
            if (declaringType is IInterfaceDefinition)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifiers.Public);
            }

            if (declaringType is IClassDefinition)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifiers.Private);
            }

            if (declaringType is IStructDefinition)
            {
                return node.Modifiers.DetermineAccessModifier(AccessModifiers.Private);
            }

            // TODO: Fill these out when the types are supported
            throw new NotSupportedException();

            // Struct default enum is public
        }

        /// <inheritdoc />
        public AccessModifiers AccessModifiers { get; }

        /// <inheritdoc />
        public ITypeDefinition DeclaringType { get; set; }

        /// <inheritdoc />
        public abstract string ReturnType { get; }
    }
}