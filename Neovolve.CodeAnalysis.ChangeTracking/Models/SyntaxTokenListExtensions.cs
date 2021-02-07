namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class SyntaxTokenListExtensions
    {
        public static AccessModifiers DetermineAccessModifier(this SyntaxTokenList tokenList,
            AccessModifiers defaultValue)
        {
            if (tokenList.HasModifier(SyntaxKind.ProtectedKeyword))
            {
                if (tokenList.HasModifier(SyntaxKind.InternalKeyword))
                {
                    return AccessModifiers.ProtectedInternal;
                }

                if (tokenList.HasModifier(SyntaxKind.PrivateKeyword))
                {
                    return AccessModifiers.ProtectedPrivate;
                }

                return AccessModifiers.Protected;
            }

            if (tokenList.HasModifier(SyntaxKind.InternalKeyword))
            {
                return AccessModifiers.Internal;
            }

            if (tokenList.HasModifier(SyntaxKind.PrivateKeyword))
            {
                return AccessModifiers.Private;
            }

            if (tokenList.HasModifier(SyntaxKind.PublicKeyword))
            {
                return AccessModifiers.Public;
            }

            return defaultValue;
        }

        public static bool HasModifier(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.Any(x => x.RawKind == (int) kind);
        }
    }
}