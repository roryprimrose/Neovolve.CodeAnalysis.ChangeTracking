namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class SyntaxTokenListExtensions
    {
        public static AccessModifier DetermineAccessModifier(this SyntaxTokenList tokenList,
            AccessModifier defaultValue)
        {
            if (tokenList.HasModifier(SyntaxKind.ProtectedKeyword))
            {
                if (tokenList.HasModifier(SyntaxKind.InternalKeyword))
                {
                    return AccessModifier.ProtectedInternal;
                }

                if (tokenList.HasModifier(SyntaxKind.PrivateKeyword))
                {
                    return AccessModifier.ProtectedPrivate;
                }

                return AccessModifier.Protected;
            }

            if (tokenList.HasModifier(SyntaxKind.InternalKeyword))
            {
                return AccessModifier.Internal;
            }

            if (tokenList.HasModifier(SyntaxKind.PrivateKeyword))
            {
                return AccessModifier.Private;
            }

            if (tokenList.HasModifier(SyntaxKind.PublicKeyword))
            {
                return AccessModifier.Public;
            }

            return defaultValue;
        }

        public static bool HasModifier(this SyntaxTokenList tokenList, SyntaxKind kind)
        {
            return tokenList.Any(x => x.RawKind == (int) kind);
        }
    }
}