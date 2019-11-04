namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ScannerTests
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

        public ScannerTests(ITestOutputHelper output)
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
            var definition = Model.Create<PropertyDefinition>();
            var rootNode = await TestNode.Parse(StandardProperty).ConfigureAwait(false);
            var node = TestNode.FindNode<PropertyDeclarationSyntax>(rootNode);

            resolver.IsSupported(node).Returns(true);
            resolver.EvaluateChildren.Returns(evaluateChildren);
            resolver.Resolve(node).Returns(definition);

            var nodes = new List<SyntaxNode> {rootNode};

            var sut = new Scanner(resolvers, _logger);

            var actual = sut.FindDefinitions(nodes);

            actual.Should().Contain(definition);
        }
        
        [Fact]
        public void FindDefinitionsReturnsEmptyWhenNodesAreEmpty()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};
            var nodes = new List<SyntaxNode>();

            var sut = new Scanner(resolvers, _logger);

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
            var node = TestNode.FindNode<PropertyDeclarationSyntax>(rootNode);

            var nodes = new List<SyntaxNode> {rootNode};

            var sut = new Scanner(resolvers, _logger);

            var actual = sut.FindDefinitions(nodes);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void FindDefinitionsThrowsExceptionWithNullNodes()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};

            var sut = new Scanner(resolvers, _logger);

            Action action = () => sut.FindDefinitions(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithEmptyResolvers()
        {
            var resolvers = new List<INodeResolver>();

            Action action = () => new Scanner(resolvers, _logger);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullLogger()
        {
            var firstResolver = Substitute.For<INodeResolver>();
            var secondResolver = Substitute.For<INodeResolver>();
            var resolvers = new List<INodeResolver> {firstResolver, secondResolver};

            Action action = () => new Scanner(resolvers, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullResolvers()
        {
            Action action = () => new Scanner(null, _logger);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}