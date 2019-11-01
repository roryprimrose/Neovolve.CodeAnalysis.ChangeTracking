namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class Scanner : IScanner
    {
        private readonly ILogger _logger;
        private readonly IList<INodeResolver> _resolvers;

        public Scanner(IEnumerable<INodeResolver> resolvers, ILogger logger)
        {
            _resolvers = resolvers.ToList();
            _logger = logger;
        }

        public IEnumerable<NodeDefinition> FindDefinitions(IEnumerable<SyntaxNode> nodes)
        {
            var definitions = new List<NodeDefinition>();

            foreach (var node in nodes)
            {
                var definitionsFound = FindDefinitions(node);

                definitions.AddRange(definitionsFound);
            }

            return definitions;
        }

        public IEnumerable<NodeDefinition> FindDefinitions(SyntaxNode node)
        {
            var definitions = new List<NodeDefinition>();

            FindDefinitions(node, definitions);

            return definitions;
        }

        private void FindDefinitions(SyntaxNode node, IList<NodeDefinition> definitions)
        {
            var resolver = FindSupportingResolver(node);

            if (resolver != null)
            {
                var definition = resolver.Resolve(node);

                definitions.Add(definition);

                if (resolver.EvaluateChildren == false)
                {
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