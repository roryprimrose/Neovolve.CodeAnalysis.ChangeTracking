namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class FieldComparerTests
    {
        [Theory]
        [InlineData(FieldModifiers.None, FieldModifiers.None, SemVerChangeType.None)]
        [InlineData(FieldModifiers.None, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.None, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.None)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.ReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.Static, FieldModifiers.Static, SemVerChangeType.None)]
        [InlineData(FieldModifiers.Static, FieldModifiers.StaticReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.ReadOnly, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(FieldModifiers.StaticReadOnly, FieldModifiers.StaticReadOnly, SemVerChangeType.None)]
        public void CompareItemsReturnsExpectedResultBasedOnFieldModifiers(FieldModifiers oldModifiers,
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

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new FieldComparer(attributeProcessor);

            var actual = sut.CompareItems(match, options).ToList();

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
        public void CompareItemsReturnsNoChangeWhenFieldsMatch()
        {
            var field = new TestFieldDefinition();
            var match = new ItemMatch<IFieldDefinition>(field, field);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new FieldComparer(attributeProcessor);

            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsRunsAdditionalChecksIfModifiersCheckFindsBreakingChange()
        {
            var oldItem = new TestFieldDefinition().Set(x => x.Modifiers = FieldModifiers.ReadOnly);
            var newItem = new TestFieldDefinition().Set(x => { x.Modifiers = FieldModifiers.Static; });
            var match = new ItemMatch<IFieldDefinition>(oldItem, newItem);
            var options = ComparerOptions.Default;

            var attributeProcessor = Substitute.For<IAttributeMatchProcessor>();

            var sut = new FieldComparer(attributeProcessor);

            // This should find a change in return type as well as modifiers
            var actual = sut.CompareItems(match, options).ToList();

            actual.Should().HaveCount(2);
        }
    }
}