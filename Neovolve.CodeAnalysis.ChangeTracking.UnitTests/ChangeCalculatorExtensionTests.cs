namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;

    public class ChangeCalculatorExtensionTests
    {
        [Fact]
        public void CalculateChangesThrowsExceptionWithNullNewCodeSource()
        {
            var calculator = Substitute.For<IChangeCalculator>();
            var oldCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();

            Func<Task> action = async () => await calculator.CalculateChanges(oldCode, null!, CancellationToken.None)
                .ConfigureAwait(false);

            action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangesThrowsExceptionWithNullOldCodeSource()
        {
            var calculator = Substitute.For<IChangeCalculator>();
            var newCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();

            Func<Task> action = async () => await calculator.CalculateChanges(null!, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CalculateChangesWithCodeSourceReturnsResultsForMultipleDefinitions()
        {
            var oldCode = new List<CodeSource>
            {
                new(TestNode.MultipleClasses)
            };
            var newCode = new List<CodeSource>
            {
                new(TestNode.MultipleInterfaces)
            };
            var options = new ComparerOptions();
            var expected = new ChangeCalculatorResult();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().Count() == 2),
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<InterfaceDefinition>().Count() == 2),
                options).Returns(expected);

            var actual = await calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangesWithCodeSourceReturnsResultsUsingDefaultOptions()
        {
            var oldCode = new List<CodeSource>
            {
                new(TestNode.ClassProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(TestNode.Field)
            };
            var expected = new ChangeCalculatorResult();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 0),
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 1),
                Arg.Any<ComparerOptions>()).Returns(expected);

            var actual = await calculator.CalculateChanges(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangesWithCodeSourceReturnsResultsUsingProvidedOptions()
        {
            var oldCode = new List<CodeSource>
            {
                new(TestNode.ClassProperty)
            };
            var newCode = new List<CodeSource>
            {
                new(TestNode.Field)
            };
            var options = new ComparerOptions();
            var expected = new ChangeCalculatorResult();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 0),
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 1),
                options).Returns(expected);

            var actual = await calculator.CalculateChanges(oldCode, newCode, options, CancellationToken.None)
                .ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CalculateChangesWithCodeSourceThrowsExceptionWithNullCalculator()
        {
            var oldCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();
            var newCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();

            Func<Task> action = async () => await ChangeCalculatorExtensions
                .CalculateChanges(null!, oldCode, newCode, CancellationToken.None).ConfigureAwait(false);

            action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CalculateChangesWithSyntaxNodesReturnsResultsForMultipleDefinitions()
        {
            var oldNode = await TestNode.Parse(TestNode.MultipleClasses + Environment.NewLine + TestNode.MultipleInterfaces).ConfigureAwait(false);
            var oldNodes = new List<SyntaxNode>
            {
                oldNode
            };
            var newNode = await TestNode.Parse(TestNode.MultipleStructs + Environment.NewLine + TestNode.Enum).ConfigureAwait(false);
            var newNodes = new List<SyntaxNode>
            {
                newNode
            };
            var options = new ComparerOptions();
            var expected = new ChangeCalculatorResult();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().Count() == 2 && x.OfType<InterfaceDefinition>().Count() == 2),
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<StructDefinition>().Count() == 2 && x.OfType<EnumDefinition>().Count() == 1),
                options).Returns(expected);

            var actual = calculator.CalculateChanges(oldNodes, newNodes, options);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangesWithSyntaxNodesReturnsResultsUsingDefaultOptions()
        {
            var oldNode = await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false);
            var oldNodes = new List<SyntaxNode>
            {
                oldNode
            };
            var newNode = await TestNode.Parse(TestNode.Field).ConfigureAwait(false);
            var newNodes = new List<SyntaxNode>
            {
                newNode
            };
            var expected = new ChangeCalculatorResult();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 0),
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 1),
                Arg.Any<ComparerOptions>()).Returns(expected);

            var actual = calculator.CalculateChanges(oldNodes, newNodes);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangesWithSyntaxNodesReturnsResultsUsingProvidedOptions()
        {
            var oldNode = await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false);
            var oldNodes = new List<SyntaxNode>
            {
                oldNode
            };
            var newNode = await TestNode.Parse(TestNode.Field).ConfigureAwait(false);
            var newNodes = new List<SyntaxNode>
            {
                newNode
            };
            var options = new ComparerOptions();
            var expected = new ChangeCalculatorResult();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 0),
                Arg.Is<IEnumerable<IBaseTypeDefinition>>(x => x.OfType<ClassDefinition>().First().Fields.Count == 1),
                options).Returns(expected);

            var actual = calculator.CalculateChanges(oldNodes, newNodes, options);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangesWithSyntaxNodesThrowsExceptionWithNullCalculator()
        {
            var oldNode = await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false);
            var oldNodes = new List<SyntaxNode>
            {
                oldNode
            };
            var newNode = await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false);
            var newNodes = new List<SyntaxNode>
            {
                newNode
            };

            Action action = () => ChangeCalculatorExtensions.CalculateChanges(null!, oldNodes, newNodes);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task CalculateChangesWithSyntaxNodesThrowsExceptionWithNullNewNodes()
        {
            var oldNode = await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false);
            var oldNodes = new List<SyntaxNode>
            {
                oldNode
            };

            var calculator = Substitute.For<IChangeCalculator>();

            Action action = () => calculator.CalculateChanges(oldNodes, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task CalculateChangesWithSyntaxNodesThrowsExceptionWithNullOldNodes()
        {
            var newNode = await TestNode.Parse(TestNode.ClassProperty).ConfigureAwait(false);
            var newNodes = new List<SyntaxNode>
            {
                newNode
            };

            var calculator = Substitute.For<IChangeCalculator>();

            Action action = () => calculator.CalculateChanges(null!, newNodes);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}