namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class Scanner : IScanner
    {
        private readonly ILogger _logger;
        private readonly IList<INodeResolver> _resolvers;

        public Scanner(IEnumerable<INodeResolver> resolvers, ILogger logger)
        {
            Ensure.Any.IsNotNull(resolvers, nameof(resolvers));
            Ensure.Any.IsNotNull(logger, nameof(logger));

            _resolvers = resolvers.ToList();

            Ensure.Collection.HasItems(_resolvers, nameof(resolvers));

            _logger = logger;
        }

        public IEnumerable<NodeDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes)
        {
            Ensure.Any.IsNotNull(nodes, nameof(nodes));

            var definitions = new List<NodeDefinition>();

            foreach (var node in nodes)
            {
                var definitionsFound = FindDefinitions(node);

                definitions.AddRange(definitionsFound);
            }

            return definitions;
        }

        private IEnumerable<NodeDefinition> FindDefinitions(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var definitions = new List<NodeDefinition>();

            FindDefinitions(node, definitions);

            return definitions;
        }

        private void FindDefinitions(SyntaxNode node, ICollection<NodeDefinition> definitions)
        {
            _logger.LogDebug("Checking node {0}", node.GetType().Name);

            var resolver = FindSupportingResolver(node);

            if (resolver != null)
            {
                if (resolver.SkipNode)
                {
                    _logger.LogDebug("Resolver {0} matches node {1} but skips processing it",
                        resolver.GetType().Namespace,
                        node.GetType().Namespace);

                    return;
                }

                var definition = resolver.Resolve(node);

                _logger.LogInformation("Resolver {0} matches node {1} and returned definition {2}",
                    resolver.GetType().Name,
                    node.GetType().Name,
                    definition.GetType().Name);

                definitions.Add(definition);

                if (resolver.EvaluateChildren == false)
                {
                    _logger.LogDebug("Skipping children of node {0}", node.GetType().Name);

                    // We are not going to recurse down into all the syntax children
                    // The resolver is telling us that there is no need to look further down this tree
                    return;
                }
            }

            var childNodes = node.ChildNodes();

            foreach (var childNode in childNodes)
            {
                FindDefinitions(childNode, definitions);
            }
        }

        private INodeResolver FindSupportingResolver(SyntaxNode node)
        {
            return _resolvers.FirstOrDefault(x => x.IsSupported(node));
        }
    }
}