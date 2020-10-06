namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class ParameterDefinition : ElementDefinition, IParameterDefinition
    {
        public ParameterDefinition(IMemberDefinition declaringMember, ParameterSyntax node) : base(node)
        {
            DeclaringMember = declaringMember ?? throw new ArgumentNullException(nameof(declaringMember));
            node = node ?? throw new ArgumentNullException(nameof(node));

            var name = node.Identifier.Text;

            Name = name;
            FullName = DeclaringMember.FullName + "_" + name;
            RawName = name;
            FullRawName = DeclaringMember.FullRawName + "_" + name;

            Type = node.Type?.ToString() ?? string.Empty;
            DefaultValue = node.Default?.Value.ToString() ?? string.Empty;
            Modifier = DetermineModifier(node);
        }

        private static ParameterModifier DetermineModifier(ParameterSyntax node)
        {
            if (node.Modifiers.HasModifier(SyntaxKind.ThisKeyword))
            {
                return ParameterModifier.This;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.RefKeyword))
            {
                return ParameterModifier.Ref;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.OutKeyword))
            {
                return ParameterModifier.Out;
            }

            if (node.Modifiers.HasModifier(SyntaxKind.ParamsKeyword))
            {
                return ParameterModifier.Params;
            }

            return ParameterModifier.None;
        }

        public IMemberDefinition DeclaringMember { get; }
        public string DefaultValue { get; }
        public override string FullName { get; }
        public override string FullRawName { get; }
        public override bool IsVisible => DeclaringMember.IsVisible;
        public ParameterModifier Modifier { get; }
        public override string Name { get; }
        public override string RawName { get; }
        public string Type { get; }
    }
}