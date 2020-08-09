﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
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
        public async Task NoChangeFoundWhenMatchingSameCode()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TestNode.ClassProperty),
                new CodeSource(TestNode.Field)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(TestNode.ClassProperty),
                new CodeSource(TestNode.Field)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
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

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassChangesToStruct()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleClass.Replace("class", "struct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassReplacedByInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesToClass()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface.Replace("interface", "class"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesToStruct()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleInterface.Replace("interface", "struct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructChangesToClass()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("struct", "class"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructChangesToInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(SingleStruct.Replace("struct", "interface"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            OutputResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        private void OutputResult(ChangeCalculatorResult result)
        {
            _output.WriteLine($"Overall result: {result.ChangeType}");
            _output.WriteLine(string.Empty);

            foreach (var comparisonResult in result.ComparisonResults)
            {
                _output.WriteLine(comparisonResult.ChangeType + ": " + comparisonResult.Message);
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

        public string SingleInterface => @"
namespace MyNamespace 
{
    public interface MyInterface
    {
        string MyProperty { get; set; }
    }  
}
";

        public string SingleStruct => @"
namespace MyNamespace 
{
    public struct MyStruct
    {
        string MyProperty { get; set; }
    }  
}
";
    }
}