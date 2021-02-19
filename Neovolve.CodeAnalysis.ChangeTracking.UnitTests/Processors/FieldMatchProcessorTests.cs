namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Processors
{
    using System;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class FieldMatchProcessorTests
    {
        private readonly ILogger _logger;

        public FieldMatchProcessorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CanCreateClass()
        {
            var comparer = Substitute.For<IFieldComparer>();
            var evaluator = Substitute.For<IFieldEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldMatchProcessor(evaluator, comparer, _logger);

            action.Should().NotThrow();
        }
    }
}