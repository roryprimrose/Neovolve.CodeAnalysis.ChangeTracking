namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class FieldComparerTests
    {
        private readonly ITestOutputHelper _output;

        public FieldComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(FieldModifiers.None, FieldModifiers.None, SemVerChangeType.None)]
        [InlineData(FieldModifiers.None, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.None, SemVerChangeType.Feature)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.None)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.Static, SemVerChangeType.None)]
        [InlineData(FieldModifiers.Static, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.Static, SemVerChangeType.Feature)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.None)]
        public void CompareMatchReturnsExpectedResultBasedOnFieldModifiers(FieldModifiers oldModifiers,
            FieldModifiers newModifiers,
            SemVerChangeType expected)
        {
            static string CalculateDeclared(FieldModifiers value)
            {
                return value switch
                {
                    FieldModifiers.Static => "static",
                    FieldModifiers.ReadOnly => "readonly",
                    FieldModifiers.StaticReadOnly => "static readonly",
                    _ => string.Empty
                };
            }

            var oldItem = new TestFieldDefinition().Set(x =>
            {
                x.Modifiers = oldModifiers;
                x.DeclaredModifiers = CalculateDeclared(oldModifiers);
            });
            var newItem = new TestFieldDefinition().Set(x =>
            {
                x.Modifiers = newModifiers;
                x.DeclaredModifiers = CalculateDeclared(newModifiers);
                x.ReturnType = oldItem.ReturnType;
            });
            var match = new ItemMatch<IFieldDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var result = new ComparisonResult(expected, oldItem, newItem, "Some message");

            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var modifiersComparer = Substitute.For<IFieldModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            if (expected != SemVerChangeType.None)
            {
                modifiersComparer.CompareMatch(Arg.Is<ItemMatch<IModifiersElement<FieldModifiers>>>(x => x.OldItem == oldItem && x.NewItem == newItem), options).Returns(new[] { result });
            }

            var sut = new FieldComparer(accessModifiersComparer, modifiersComparer, attributeProcessor);

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            if (expected == SemVerChangeType.None)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual.Should().HaveCount(1);
                actual[0].ChangeType.Should().Be(expected);
                actual[0].OldItem.Should().Be(oldItem);
                actual[0].NewItem.Should().Be(newItem);
            }
        }

        [Fact]
        public void CompareMatchReturnsNoChangeWhenFieldsMatch()
        {
            var field = new TestFieldDefinition();
            var match = new ItemMatch<IFieldDefinition>(field, field);
            var options = ComparerOptions.Default;

            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var modifiersComparer = Substitute.For<IFieldModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new FieldComparer(accessModifiersComparer, modifiersComparer, attributeProcessor);

            var actual = sut.CompareMatch(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchDoesNotRunAdditionalChecksIfAccessModifiersCheckFindsBreakingChange()
        {
            var oldItem = new TestFieldDefinition();
            var newItem = new TestFieldDefinition().Set(x => x.AccessModifiers = AccessModifiers.Private);
            var match = new ItemMatch<IFieldDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;
            var modifierResult = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, "Different modifier");
            var accessModifierResult = new ComparisonResult(SemVerChangeType.Breaking, oldItem, newItem, "Different access modifier");

            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var modifiersComparer = Substitute.For<IFieldModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            accessModifiersComparer.CompareMatch(Arg.Is<ItemMatch<IAccessModifiersElement<AccessModifiers>>>(x => x.OldItem == oldItem && x.NewItem == newItem), options).Returns(new[] { accessModifierResult });
            modifiersComparer.CompareMatch(Arg.Is<ItemMatch<IModifiersElement<FieldModifiers>>>(x => x.OldItem == oldItem && x.NewItem == newItem), options).Returns(new []{ modifierResult });

            var sut = new FieldComparer(accessModifiersComparer, modifiersComparer, attributeProcessor);

            var actual = sut.CompareMatch(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().Be(accessModifierResult);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullAccessModifiersComparer()
        {
            var modifiersComparer = Substitute.For<IFieldModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldComparer(null!, modifiersComparer, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullAttributeProcessor()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var modifiersComparer = Substitute.For<IFieldModifiersComparer>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldComparer(accessModifiersComparer, modifiersComparer, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullModifiersComparer()
        {
            var accessModifiersComparer = Substitute.For<IAccessModifiersComparer>();
            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new FieldComparer(accessModifiersComparer, null!, attributeProcessor);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}