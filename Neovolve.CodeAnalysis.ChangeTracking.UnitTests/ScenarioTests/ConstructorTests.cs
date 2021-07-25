namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ScenarioTests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class ConstructorTests
    {
        public const string ClassWithDefaultConstructor =
            @"
namespace MyNamespace 
{
    public class MyClass
    {
        public MyClass()
        {
        }
    }  
}
";

        public const string ClassWithDefaultConstructorAndParameterConstructor =
            @"
namespace MyNamespace 
{
    public class MyClass
    {
        public MyClass()
        {
        }

        public MyClass(string first, int second, bool third, DateTimeOffset fourth)
        {
        }
    }  
}
";

        public const string ClassWithParameterConstructor =
            @"
namespace MyNamespace 
{
    public class MyClass
    {
        public MyClass(string first, int second, bool third, DateTimeOffset fourth)
        {
        }
    }  
}
";

        public const string EmptyClass =
            @"
namespace MyNamespace 
{
    public class MyClass
    {
    }  
}
";

        public const string EmptyStaticClass =
            @"
namespace MyNamespace 
{
    public static class MyClass
    {
    }  
}
";

        public const string StaticClassWithDefaultConstructor =
            @"
namespace MyNamespace 
{
    public static class MyClass
    {
        static MyClass()
        {
        }
    }  
}
";

        private readonly IChangeCalculator _calculator;
        private readonly ITestOutputHelper _output;

        public ConstructorTests(ITestOutputHelper output)
        {
            _output = output;

            var logger = output.BuildLogger(LogLevel.Information);

            _calculator = ChangeCalculatorFactory.BuildCalculator(logger);
        }

        [Fact]
        public async Task ReturnsBreakingWhenConstructorAddsParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor.Replace(", DateTimeOffset fourth", string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenConstructorChangesParameterType()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor.Replace("DateTimeOffset", "Stream"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenConstructorRemovesParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor.Replace(", DateTimeOffset fourth", string.Empty))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData(EmptyClass)]
        [InlineData(ClassWithDefaultConstructor)]
        public async Task ReturnsBreakingWhenConstructorWithParametersRemoved(string updatedCode)
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(updatedCode)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenDefaultConstructorRemovedAndAnotherConstructorExists()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithParameterConstructor)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Theory]
        [InlineData(EmptyClass)]
        [InlineData(ClassWithDefaultConstructor)]
        public async Task ReturnsFeatureWhenConstructorWithParametersAdded(string originalCode)
        {
            var oldCode = new List<CodeSource>
            {
                new(originalCode)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Feature);
        }

        [Fact]
        public async Task ReturnsNoChangeWheDefaultConstructorAddedToClassWithNoOtherConstructors()
        {
            var oldCode = new List<CodeSource>
            {
                new(EmptyClass)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructor)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsNoChangeWhenDefaultConstructorRemovedFromClassWithNoOtherConstructors()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(EmptyClass)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsNoChangeWhenStaticConstructorAddedToStaticClass()
        {
            var oldCode = new List<CodeSource>
            {
                new(EmptyStaticClass)
            };
            var newCode = new List<CodeSource>
            {
                new(StaticClassWithDefaultConstructor)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }

        [Fact]
        public async Task ReturnsNoChangeWhenStaticConstructorRemovedFromStaticClass()
        {
            var oldCode = new List<CodeSource>
            {
                new(StaticClassWithDefaultConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(EmptyStaticClass)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.None);
        }
    }
}