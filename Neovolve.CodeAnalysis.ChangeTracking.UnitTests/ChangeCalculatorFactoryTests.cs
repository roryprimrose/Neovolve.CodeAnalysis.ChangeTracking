namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class ChangeCalculatorFactoryTests
    {
        private readonly ILogger _logger;

        public ChangeCalculatorFactoryTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        [Fact]
        public void BuildCalculatorReturnsCalculatorWithLogger()
        {
            var actual = ChangeCalculatorFactory.BuildCalculator(_logger);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void BuildCalculatorReturnsCalculatorWithNullLogger()
        {
            var actual = ChangeCalculatorFactory.BuildCalculator();

            actual.Should().NotBeNull();
        }
    }
}