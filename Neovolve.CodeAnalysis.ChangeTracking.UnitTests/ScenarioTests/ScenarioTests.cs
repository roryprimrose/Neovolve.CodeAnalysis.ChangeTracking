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

        public ScenarioTests(ITestOutputHelper output)
        {
            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task NoChangeFoundWhenMatchingSameCode()
        {
            var oldCode = new List<CodeSource>
            {
                new(TestNode.ClassProperty),
                new(TestNode.Field)
            };
            var newCode = new List<CodeSource>
            {
                new(TestNode.ClassProperty),
                new(TestNode.Field)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassChangesToInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleClass.Replace("class", "interface"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassChangesToStruct()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleClass.Replace("class", "struct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassReplacedByInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleClass)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleInterface)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesToClass()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleInterface.Replace("interface", "class"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenInterfaceChangesToStruct()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleInterface)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleInterface.Replace("interface", "struct"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructChangesToClass()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleStruct.Replace("struct", "class"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructChangesToInterface()
        {
            var oldCode = new List<CodeSource>
            {
                new(SingleStruct)
            };
            var newCode = new List<CodeSource>
            {
                new(SingleStruct.Replace("struct", "interface"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
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