namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class PropertyAccessorMatchProcessorTests
    {
        private readonly ILogger _logger;

        public PropertyAccessorMatchProcessorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CanCreateClass()
        {
            var comparer = Substitute.For<IPropertyAccessorComparer>();
            var evaluator = Substitute.For<IPropertyAccessorEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyAccessorMatchProcessor(evaluator, comparer, _logger);

            action.Should().NotThrow();
        }
    }
}