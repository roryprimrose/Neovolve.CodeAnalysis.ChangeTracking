namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
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
        private readonly ITestOutputHelper _output;

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassChangesToInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("class", "interface"))
            };

            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenFieldReturnTypeChanged()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value;
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public bool Value;
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenFieldScopeMadeMoreRestrictive()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value;
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    internal string Value;
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenOldPublicMemberMissingFromNewCode()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(TestNode.ClassProperty),
        //                new CodeSource(TestNode.Field)
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(TestNode.Field)
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenPropertyGetChangedToPrivate()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { private get; set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenPropertyReturnTypeChanged()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public bool Value { get; set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenPropertyScopeMadeMoreRestrictive()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    internal string Value { get; set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task BreakingChangeFoundWhenPropertySetChangedToPrivate()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; private set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //        }

        //        [Fact]
        //        public async Task FeatureChangeFoundWhenFieldScopeMadePublic()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    internal string Value;
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value;
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        //        }

        //        [Fact]
        //        public async Task FeatureChangeFoundWhenNewCodeAddsPublicMember()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(TestNode.Field)
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(TestNode.ClassProperty),
        //                new CodeSource(TestNode.Field)
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        //        }

        //        [Fact]
        //        public async Task FeatureChangeFoundWhenPropertyGetChangedToPublic()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { private get; set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        //        }

        //        [Fact]
        //        public async Task FeatureChangeFoundWhenPropertyScopeMadePublic()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    internal string Value { get; set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        //        }

        //        [Fact]
        //        public async Task FeatureChangeFoundWhenPropertySetChangedToPublic()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; private set; }
        //}
        //")
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(@"
        //public class Test
        //{
        //    public string Value { get; set; }
        //}
        //")
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        //        }

        //        [Fact]
        //        public async Task NoChangeFoundWhenMatchingSameCode()
        //        {
        //            var oldCode = new List<CodeSource>
        //            {
        //                new CodeSource(TestNode.ClassProperty),
        //                new CodeSource(TestNode.Field)
        //            };
        //            var newCode = new List<CodeSource>
        //            {
        //                new CodeSource(TestNode.ClassProperty),
        //                new CodeSource(TestNode.Field)
        //            };

        //            var result = await _calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
        //                .ConfigureAwait(false);

        //            OutputResult(result);

        //            result.ChangeType.Should().Be(SemVerChangeType.None);
        //        }

        private void OutputResult(ChangeCalculatorResult result)
        {
            _output.WriteLine($"Overall result: {result.ChangeType}");
            _output.WriteLine(string.Empty);

            foreach (var comparisonResult in result.ComparisonResults)
            {
                _output.WriteLine(comparisonResult.Message);
            }
        }

        public string SingleClass => @"
namespace MyNamespace 
{
    [ClassAttribute(123, false, myName: ""on the class"")]
    public class MyClass
    {
        [PropertyAttribute(344, true, myName: ""on the property"")]
        public string MyProperty { get; set; }

        [FieldAttribute(885, myName: ""on the field"")]
        public string MyField;
    }  
}
";
    }
}