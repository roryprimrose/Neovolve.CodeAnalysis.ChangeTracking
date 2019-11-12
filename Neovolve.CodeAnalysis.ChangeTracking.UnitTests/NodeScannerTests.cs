namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class NodeScannerTests
    {
        private const string StandardProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        string MyProperty
        {
            get;
            set;
        }
    }   
}
";

        private readonly ILogger _logger;

        public NodeScannerTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FindDefinitionsReturnsDefinitionMatchingResolver(bool evaluateChildren)
        {
            var resolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {resolver};
            var definition = Model.UsingModule<CompilerModule>().Create<PropertyDefinition>();
            var rootNode = await TestNode.Parse(StandardProperty).ConfigureAwait(false);
            var node = TestNode.FindNode<PropertyDeclarationSyntax>(rootNode);

            resolver.IsSupported(node).Returns(true);
            resolver.EvaluateChildren.Returns(evaluateChildren);
            resolver.Resolve(node).Returns(definition);

            var nodes = new List<SyntaxNode> {rootNode};

            var sut = new NodeScanner(resolvers, _logger);

            var actual = sut.FindDefinitions(nodes);

            actual.Should().Contain(definition);
        }

        [Fact]
        public void FindDefinitionsReturnsEmptyWhenNodesAreEmpty()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};

            // ReSharper disable once CollectionNeverUpdated.Local
            var nodes = new List<SyntaxNode>();

            var sut = new NodeScanner(resolvers, _logger);

            var actual = sut.FindDefinitions(nodes);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task FindDefinitionsReturnsEmptyWhenNoNodesMatchResolvers()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};
            var rootNode = await TestNode.Parse(StandardProperty).ConfigureAwait(false);

            var nodes = new List<SyntaxNode> {rootNode};

            var sut = new NodeScanner(resolvers, _logger);

            var actual = sut.FindDefinitions(nodes);

            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task FindDefinitionsSkipsProcessingWhenResolverSkipNodeIsTrue()
        {
            var resolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {resolver};
            var rootNode = await TestNode.Parse(StandardProperty).ConfigureAwait(false);

            resolver.IsSupported(rootNode).Returns(true);
            resolver.SkipNode.Returns(true);

            var nodes = new List<SyntaxNode> {rootNode};

            var sut = new NodeScanner(resolvers, _logger);

            var actual = sut.FindDefinitions(nodes);

            actual.Should().BeEmpty();
            resolver.Received(1).IsSupported(Arg.Any<SyntaxNode>());
        }

        [Fact]
        public void FindDefinitionsThrowsExceptionWithNullNodes()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};

            var sut = new NodeScanner(resolvers, _logger);

            Action action = () => sut.FindDefinitions(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWithEmptyResolvers()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var resolvers = new List<INodeResolver>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new NodeScanner(resolvers, _logger);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWithNullLogger()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new NodeScanner(resolvers, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage("Usage", "CA1806:Do not ignore method results", Justification = "Testing constructor guard clause")]
        public void ThrowsExceptionWithNullResolvers()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new NodeScanner(null, _logger);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}