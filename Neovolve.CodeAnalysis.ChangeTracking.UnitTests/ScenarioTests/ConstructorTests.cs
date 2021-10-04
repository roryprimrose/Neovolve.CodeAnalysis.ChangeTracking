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

        public const string StructWithDefaultConstructorAndParameterConstructor =
            @"
namespace MyNamespace 
{
    public struct MyStruct
    {
        public MyStruct()
        {
        }

        public MyStruct(string first, int second, bool third, DateTimeOffset fourth)
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
        public async Task ReturnsBreakingWhenClassConstructorAddsParameter()
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
        public async Task ReturnsBreakingWhenClassConstructorChangesParameterType()
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
        public async Task ReturnsBreakingWhenClassConstructorChangesParameterTypeAndName()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor.Replace("DateTimeOffset fourth", "Stream other"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenClassConstructorRemovesParameter()
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

        [Fact]
        public async Task ReturnsBreakingWhenClassConstructorReordersParameters()
        {
            var oldCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(ClassWithDefaultConstructorAndParameterConstructor.Replace(", bool third, DateTimeOffset fourth",
                    ", DateTimeOffset fourth, bool third"))
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

        [Fact]
        public async Task ReturnsBreakingWhenStructConstructorAddsParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor.Replace(", DateTimeOffset fourth",
                    string.Empty))
            };
            var newCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor)
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructConstructorChangesParameterType()
        {
            var oldCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor.Replace("DateTimeOffset", "Stream"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructConstructorChangesParameterTypeAndName()
        {
            var oldCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor.Replace("DateTimeOffset fourth",
                    "Stream other"))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructConstructorRemovesParameter()
        {
            var oldCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor.Replace(", DateTimeOffset fourth",
                    string.Empty))
            };

            var options = OptionsFactory.BuildOptions();

            var result = await _calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            _output.WriteResult(result);

            result.ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public async Task ReturnsBreakingWhenStructConstructorReordersParameters()
        {
            var oldCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor)
            };
            var newCode = new List<CodeSource>
            {
                new(StructWithDefaultConstructorAndParameterConstructor.Replace(", bool third, DateTimeOffset fourth",
                    ", DateTimeOffset fourth, bool third"))
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