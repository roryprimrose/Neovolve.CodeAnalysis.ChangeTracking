﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using ModelBuilder;
    using NSubstitute;
    using Xunit;

    public class ChangeCalculatorExtensionTests
    {
        [Fact]
        public async Task CalculateChangeReturnsCalculatorResult()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TestNode.ClassProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(TestNode.Field)
            };
            var expected = Model.UsingModule<ConfigurationModule>().Create<ChangeCalculatorResult>();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                    Arg.Is<IEnumerable<SyntaxNode>>(
                        x => TestNode.FindNode<PropertyDeclarationSyntax>(x.First()) != null),
                    Arg.Is<IEnumerable<SyntaxNode>>(x => TestNode.FindNode<FieldDeclarationSyntax>(x.First()) != null))
                .Returns(expected);

            var actual = await calculator
                .CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangeReturnsCalculatorResultFromMultipleNodes()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TestNode.ClassProperty),
                new CodeSource(TestNode.Field)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(TestNode.Field),
                new CodeSource(TestNode.ClassProperty)
            };
            var expected = Model.UsingModule<ConfigurationModule>().Create<ChangeCalculatorResult>();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                    Arg.Any<IEnumerable<SyntaxNode>>(),
                    Arg.Any<IEnumerable<SyntaxNode>>())
                .Returns(expected);

            var actual = await calculator
                .CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CalculateChangeReturnsCalculatorResultWithoutCancellationToken()
        {
            var oldCode = new List<CodeSource>
            {
                new CodeSource(TestNode.ClassProperty)
            };
            var newCode = new List<CodeSource>
            {
                new CodeSource(TestNode.Field)
            };
            var expected = Model.UsingModule<ConfigurationModule>().Create<ChangeCalculatorResult>();

            var calculator = Substitute.For<IChangeCalculator>();

            calculator.CalculateChanges(
                    Arg.Is<IEnumerable<SyntaxNode>>(
                        x => TestNode.FindNode<PropertyDeclarationSyntax>(x.First()) != null),
                    Arg.Is<IEnumerable<SyntaxNode>>(x => TestNode.FindNode<FieldDeclarationSyntax>(x.First()) != null))
                .Returns(expected);

            var actual = await calculator
                .CalculateChange(oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CalculateChangeThrowsExceptionWithNullCalculator()
        {
            var oldCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();
            var newCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();

            Func<Task> action = async () => await ChangeCalculatorExtensions
                .CalculateChange(null!, oldCode, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangeThrowsExceptionWithNullNewCode()
        {
            var calculator = Substitute.For<IChangeCalculator>();
            var oldCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();

            Func<Task> action = async () => await calculator.CalculateChange(oldCode, null!, CancellationToken.None)
                .ConfigureAwait(false);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateChangeThrowsExceptionWithNullOldCode()
        {
            var calculator = Substitute.For<IChangeCalculator>();
            var newCode = Model.UsingModule<ConfigurationModule>().Create<List<CodeSource>>();

            Func<Task> action = async () => await calculator.CalculateChange(null!, newCode, CancellationToken.None)
                .ConfigureAwait(false);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}