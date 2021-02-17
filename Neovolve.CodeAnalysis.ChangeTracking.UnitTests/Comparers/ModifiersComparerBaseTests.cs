namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ModifiersComparerBaseTests
    {
        private readonly ITestOutputHelper _output;
        private const string ModiferLabel = "ModifierLabel";

        public ModifiersComparerBaseTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenNoChangeFound()
        {
            var options = new ComparerOptions();
            var oldElement = new TestPropertyDefinition();
            var newElement = new TestPropertyDefinition();
            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(SemVerChangeType.None);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, AccessModifiers.Internal, AccessModifiers.Private, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareItemsReturnsPluralModifierLabelWhenMultipleModifiersAdded()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.Private);
            oldElement.DeclaredModifiers.Returns("sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.ProtectedInternal);
            newElement.DeclaredModifiers.Returns("protected internal sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.Message.Should().Contain("Property");
            result.Message.Should().Contain(fullName);
            result.Message.Should().Contain("has added");
            result.Message.Should().Contain("protected internal");
            result.Message.Should().Contain(ModiferLabel + "s");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        [Theory]
        [InlineData("private", "protected internal")]
        [InlineData("private protected", "protected internal")]
        [InlineData("private protected", "protected")]
        public void CompareItemsReturnsPluralModifierLabelWhenMultipleModifiersChanged(string oldDeclaredModifiers,
            string newDeclaredModifiers)
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.ProtectedPrivate);
            oldElement.DeclaredModifiers.Returns($"{oldDeclaredModifiers} sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.ProtectedInternal);
            newElement.DeclaredModifiers.Returns($"{newDeclaredModifiers} sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.Message.Should().Contain("Property");
            result.Message.Should().Contain(fullName);
            result.Message.Should().Contain("has changed");
            result.Message.Should().Contain(oldDeclaredModifiers);
            result.Message.Should().Contain(newDeclaredModifiers);
            result.Message.Should().Contain(ModiferLabel + "s");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        [Fact]
        public void CompareItemsReturnsPluralModifierLabelWhenMultipleModifiersRemoved()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Breaking;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.ProtectedInternal);
            oldElement.DeclaredModifiers.Returns("protected internal sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.Private);
            newElement.DeclaredModifiers.Returns("sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.Message.Should().Contain("Property");
            result.Message.Should().Contain(fullName);
            result.Message.Should().Contain("has removed");
            result.Message.Should().Contain("protected internal");
            result.Message.Should().Contain(ModiferLabel + "s");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        [Fact]
        public void CompareItemsReturnsResultForAddedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.Private);
            oldElement.DeclaredModifiers.Returns("sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.ProtectedInternal);
            newElement.DeclaredModifiers.Returns("protected internal sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.ChangeType.Should().Be(expected);
            result.OldItem.Should().Be(oldElement);
            result.NewItem.Should().Be(newElement);
        }

        [Fact]
        public void CompareItemsReturnsResultForChangedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.Private);
            oldElement.DeclaredModifiers.Returns("private sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.ProtectedInternal);
            newElement.DeclaredModifiers.Returns("protected internal sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.ChangeType.Should().Be(expected);
            result.OldItem.Should().Be(oldElement);
            result.NewItem.Should().Be(newElement);
        }

        [Fact]
        public void CompareItemsReturnsResultForRemovedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Breaking;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.ProtectedInternal);
            oldElement.DeclaredModifiers.Returns("protected internal sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.Private);
            newElement.DeclaredModifiers.Returns("sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.ChangeType.Should().Be(expected);
            result.OldItem.Should().Be(oldElement);
            result.NewItem.Should().Be(newElement);
        }

        [Fact]
        public void CompareItemsReturnsSingularModifierLabelWhenSingleModifiersAdded()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.Private);
            oldElement.DeclaredModifiers.Returns("sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.Public);
            newElement.DeclaredModifiers.Returns("public sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.Message.Should().Contain("Property");
            result.Message.Should().Contain(fullName);
            result.Message.Should().Contain("has added");
            result.Message.Should().Contain("public");
            result.Message.Should().Contain(ModiferLabel);
            result.Message.Should().NotContain(ModiferLabel + "s");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        [Fact]
        public void CompareItemsReturnsSingularModifierLabelWhenSingleModifiersChanged()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.Private);
            oldElement.DeclaredModifiers.Returns("private sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.Public);
            newElement.DeclaredModifiers.Returns("public sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();

            result.Message.Should().Contain("Property");
            result.Message.Should().Contain(fullName);
            result.Message.Should().Contain("has changed");
            result.Message.Should().Contain("private");
            result.Message.Should().Contain("public");
            result.Message.Should().Contain(ModiferLabel);
            result.Message.Should().NotContain(ModiferLabel + "s");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        [Fact]
        public void CompareItemsReturnsSingularModifierLabelWhenSingleModifiersRemoved()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Breaking;

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();
            var oldElement = Substitute.For<IPropertyDefinition>();
            var newElement = Substitute.For<IPropertyDefinition>();

            oldElement.AccessModifiers.Returns(AccessModifiers.Public);
            oldElement.DeclaredModifiers.Returns("public sealed override");
            newElement.FullName.Returns(fullName);
            newElement.AccessModifiers.Returns(AccessModifiers.Private);
            newElement.DeclaredModifiers.Returns("sealed override");

            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareItems(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.Message.Should().Contain("Property");
            result.Message.Should().Contain(fullName);
            result.Message.Should().Contain("has removed");
            result.Message.Should().Contain("public");
            result.Message.Should().Contain(ModiferLabel);
            result.Message.Should().NotContain(ModiferLabel + "s");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        [Fact]
        public void CompareItemsThrowsExceptionWithNullMatch()
        {
            var options = new ComparerOptions();

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();

            var sut = new Wrapper(changeTable);

            Action action = () =>
                sut.RunCompareItems(null!, AccessModifiers.Internal, AccessModifiers.Private, options)
                    .ForceEnumeration();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullChangeTable()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : ModifiersComparerBase<AccessModifiers>
        {
            public Wrapper(IChangeTable<AccessModifiers> changeTable) : base(changeTable)
            {
            }

            public IEnumerable<ComparisonResult> RunCompareItems(ItemMatch<IElementDefinition> match,
                AccessModifiers oldValue, AccessModifiers newValue, ComparerOptions options)
            {
                return base.CompareItems(match, oldValue, newValue, options);
            }

            protected override string GetDeclaredModifiers(IElementDefinition element)
            {
                return element.GetDeclaredAccessModifiers();
            }

            protected override string ModifierLabel { get; } = ModiferLabel;
        }
    }
}