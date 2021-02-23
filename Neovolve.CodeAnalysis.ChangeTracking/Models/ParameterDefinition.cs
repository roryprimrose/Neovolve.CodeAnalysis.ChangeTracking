namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ParameterDefinition : ElementDefinition, IParameterDefinition
    {
        public ParameterDefinition(IMethodDefinition declaringMember, ParameterSyntax node) : base(node)
        {
            DeclaringMethod = declaringMember ?? throw new ArgumentNullException(nameof(declaringMember));
            node = node ?? throw new ArgumentNullException(nameof(node));

            var name = node.Identifier.Text;

            Name = name;
            FullName = DeclaringMethod.FullName + "_" + name;
            RawName = name;
            FullRawName = DeclaringMethod.FullRawName + "_" + name;

            Type = node.Type?.ToString() ?? string.Empty;
            DefaultValue = node.Default?.Value.ToString() ?? string.Empty;
            Modifiers = DetermineModifier(node);
        }

        private static ParameterModifiers DetermineModifier(ParameterSyntax node)
        {
            if (node.Modifiers.HasModifier(SyntaxKind.ThisKeyword))
            {
                return ParameterModifiers.This;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.RefKeyword))
            {
                return ParameterModifiers.Ref;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.OutKeyword))
            {
                return ParameterModifiers.Out;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.ParamsKeyword))
            {
                return ParameterModifiers.Params;
            }

            return ParameterModifiers.None;
        }

        public IMethodDefinition DeclaringMethod { get; }
        public string DefaultValue { get; }
        public override string FullName { get; }
        public override string FullRawName { get; }
        public override bool IsVisible => DeclaringMethod.IsVisible;
        public ParameterModifiers Modifiers { get; }
        public override string Name { get; }
        public override string RawName { get; }
        public string Type { get; }
    }
}