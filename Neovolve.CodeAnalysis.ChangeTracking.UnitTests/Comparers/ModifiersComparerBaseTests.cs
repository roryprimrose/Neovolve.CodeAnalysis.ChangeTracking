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
        public void CompareMatchReturnsEmptyWhenNoChangeFound()
        {
            var options = new ComparerOptions();
            var oldElement = new TestPropertyDefinition();
            var newElement = new TestPropertyDefinition();
            var match = new ItemMatch<IElementDefinition>(oldElement, newElement);

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(SemVerChangeType.None);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, AccessModifiers.Internal, AccessModifiers.Private, options);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareMatchReturnsPluralModifierLabelWhenMultipleModifiersAdded()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();

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
                .Returns(SemVerChangeType.Feature);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
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
        public void CompareMatchReturnsPluralModifierLabelWhenMultipleModifiersChanged(string oldDeclaredModifiers,
            string newDeclaredModifiers)
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();

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
                .Returns(SemVerChangeType.Feature);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
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
        public void CompareMatchReturnsPluralModifierLabelWhenMultipleModifiersRemoved()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();

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
                .Returns(SemVerChangeType.Breaking);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
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
        public void CompareMatchReturnsResultForAddedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            const SemVerChangeType expected = SemVerChangeType.Feature;

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

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.ChangeType.Should().Be(expected);
            result.OldItem.Should().Be(oldElement);
            result.NewItem.Should().Be(newElement);
        }

        [Fact]
        public void CompareMatchReturnsResultForChangedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            const SemVerChangeType expected = SemVerChangeType.Feature;

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

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.ChangeType.Should().Be(expected);
            result.OldItem.Should().Be(oldElement);
            result.NewItem.Should().Be(newElement);
        }

        [Fact]
        public void CompareMatchReturnsResultForRemovedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            const SemVerChangeType expected = SemVerChangeType.Breaking;

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

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();
            
            result.ChangeType.Should().Be(expected);
            result.OldItem.Should().Be(oldElement);
            result.NewItem.Should().Be(newElement);
        }

        [Fact]
        public void CompareMatchReturnsSingularModifierLabelWhenSingleModifiersAdded()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();

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
                .Returns(SemVerChangeType.Feature);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
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
        public void CompareMatchReturnsSingularModifierLabelWhenSingleModifiersChanged()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();

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
                .Returns(SemVerChangeType.Feature);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
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
        public void CompareMatchReturnsSingularModifierLabelWhenSingleModifiersRemoved()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();

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
                .Returns(SemVerChangeType.Breaking);

            var sut = new Wrapper(changeTable);

            var actual = sut.RunCompareMatch(match, oldElement.AccessModifiers, newElement.AccessModifiers, options)
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
        public void CompareMatchThrowsExceptionWithNullMatch()
        {
            var options = new ComparerOptions();

            var changeTable = Substitute.For<IAccessModifiersChangeTable>();

            var sut = new Wrapper(changeTable);

            Action action = () =>
                sut.RunCompareMatch(null!, AccessModifiers.Internal, AccessModifiers.Private, options)
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

            public IEnumerable<ComparisonResult> RunCompareMatch(ItemMatch<IElementDefinition> match,
                AccessModifiers oldValue, AccessModifiers newValue, ComparerOptions options)
            {
                return base.CompareMatch(match, oldValue, newValue, options);
            }

            protected override string GetDeclaredModifiers(IElementDefinition element)
            {
                return element.GetDeclaredAccessModifiers();
            }

            protected override string ModifierLabel { get; } = ModiferLabel;
        }
    }
}