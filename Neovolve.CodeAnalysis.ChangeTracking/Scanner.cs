namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.Logging;

    public class Scanner : IScanner
    {
        private readonly ILogger _logger;

        public Scanner(ILogger logger)
        {
            _logger = logger;
        }

        public Task<IAsyncEnumerable<NodeDefinition>> FindDefinitions(IAsyncEnumerable<SyntaxNode> nodes)
        {
            throw new NotImplementedException();
        }
    }
}