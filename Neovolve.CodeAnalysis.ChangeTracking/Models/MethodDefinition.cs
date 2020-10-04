namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class MethodDefinition : MemberDefinition
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

        public override string FullName { get; }
        public override string FullRawName { get; }

        public MethodModifiers Modifiers { get; }

        public override string Name { get; }
        public override string RawName { get; }
        public override string ReturnType { get; }
    }
}