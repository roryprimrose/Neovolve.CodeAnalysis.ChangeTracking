﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class NodeScanner : INodeScanner
    {
        private readonly ILogger? _logger;
        private readonly IList<INodeResolver> _resolvers;

        public NodeScanner(IEnumerable<INodeResolver> resolvers, ILogger? logger)
        {
            Ensure.Any.IsNotNull(resolvers, nameof(resolvers));

            _resolvers = resolvers.FastToList();

            Ensure.Collection.HasItems(_resolvers, nameof(resolvers));

            _logger = logger;
        }

        public IEnumerable<MemberDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes)
        {
            Ensure.Any.IsNotNull(nodes, nameof(nodes));

            var definitions = new List<MemberDefinition>();

            foreach (var node in nodes)
            {
                var definitionsFound = FindDefinitions(node);

                definitions.AddRange(definitionsFound);
            }

            return definitions;
        }

        private IEnumerable<MemberDefinition> FindDefinitions(SyntaxNode node)
        {
            Ensure.Any.IsNotNull(node, nameof(node));

            var definitions = new List<MemberDefinition>();

            FindDefinitions(node, definitions);

            return definitions;
        }

        private void FindDefinitions(SyntaxNode node, ICollection<MemberDefinition> definitions)
        {
            _logger?.LogDebug("Checking member {0}", node.GetType().Name);

            var resolver = FindSupportingResolver(node);

            if (resolver != null)
            {
                if (resolver.SkipNode)
                {
                    _logger?.LogDebug("Resolver {0} matches member {1} but skips processing it",
                        resolver.GetType().Namespace,
                        node.GetType().Namespace);

                    return;
                }

                var definition = resolver.Resolve(node);

                _logger?.LogDebug("Resolver {0} matches member {1} and returned definition {2}",
                    resolver.GetType().Name,
                    node.GetType().Name,
                    definition.GetType().Name);

                definitions.Add(definition);

                if (resolver.EvaluateChildren == false)
                {
                    _logger?.LogDebug("Skipping children of member {0}", node.GetType().Name);

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