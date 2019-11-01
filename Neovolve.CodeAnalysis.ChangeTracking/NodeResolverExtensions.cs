namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using EnsureThat;
    using Microsoft.CodeAnalysis;

    public static class NodeResolverExtensions
    {
        public static bool IsSupported<TN, TD>(this INodeResolver<TN, TD> resolver, SyntaxNode node)
            where TN : SyntaxNode
        {
            Ensure.Any.IsNotNull(resolver, nameof(resolver));
            Ensure.Any.IsNotNull(node, nameof(node));

            if (node is TN)
            {
                return true;
            }

            return false;
        }
    }
}