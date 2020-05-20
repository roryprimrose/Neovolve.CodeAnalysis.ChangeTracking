namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class ScenarioTests
    {
        private readonly IChangeCalculator _calculator;

        public ScenarioTests(ITestOutputHelper output)
        {
            var logger = output.BuildLogger();

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenFieldReturnTypeChanged()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value;
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public bool Value;
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenFieldScopeMadeMoreRestrictive()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value;
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    internal string Value;
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenOldPublicMemberMissingFromNewCode()
        {
            var oldCode = new List<string>
            {
                TestNode.ClassProperty,
                TestNode.Field
            };
            var newCode = new List<string>
            {
                TestNode.Field
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenPropertyGetChangedToPrivate()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public string Value { private get; set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenPropertyReturnTypeChanged()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public bool Value { get; set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenPropertyScopeMadeMoreRestrictive()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    internal string Value { get; set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task BreakingChangeFoundWhenPropertySetChangedToPrivate()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; private set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task FeatureChangeFoundWhenFieldScopeMadePublic()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    internal string Value;
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public string Value;
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task FeatureChangeFoundWhenNewCodeAddsPublicMember()
        {
            var oldCode = new List<string>
            {
                TestNode.Field
            };
            var newCode = new List<string>
            {
                TestNode.ClassProperty,
                TestNode.Field
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task FeatureChangeFoundWhenPropertyGetChangedToPublic()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value { private get; set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task FeatureChangeFoundWhenPropertyScopeMadePublic()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    internal string Value { get; set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task FeatureChangeFoundWhenPropertySetChangedToPublic()
        {
            var oldCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; private set; }
}
"
            };
            var newCode = new List<string>
            {
                @"
public class Test
{
    public string Value { get; set; }
}
"
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task NoChangeFoundWhenMatchingSameCode()
        {
            var oldCode = new List<string>
            {
                TestNode.ClassProperty,
                TestNode.Field
            };
            var newCode = new List<string>
            {
                TestNode.ClassProperty,
                TestNode.Field
            };

            var result = await _calculator.CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            result.Should().Be(SemVerChangeType.None);
        }
    }
}