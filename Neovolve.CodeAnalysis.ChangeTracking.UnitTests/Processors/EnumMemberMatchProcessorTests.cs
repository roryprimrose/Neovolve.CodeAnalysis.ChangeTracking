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

    public class EnumMemberMatchProcessorTests
    {
        private readonly ILogger _logger;

        public EnumMemberMatchProcessorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void CanCreateClass()
        {
            var comparer = Substitute.For<IEnumMemberComparer>();
            var evaluator = Substitute.For<IEnumMemberEvaluator>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new EnumMemberMatchProcessor(evaluator, comparer, _logger);

            action.Should().NotThrow();
        }
    }
}