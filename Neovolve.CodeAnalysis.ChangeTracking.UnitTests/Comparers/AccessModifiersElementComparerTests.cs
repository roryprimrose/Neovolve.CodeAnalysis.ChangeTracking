namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class AccessModifiersElementComparerTests
    {
        private readonly ITestOutputHelper _output;

        public AccessModifiersElementComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsResultForChangedModifiers()
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

            var match = new ItemMatch<IAccessModifiersElement<AccessModifiers>>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.AccessModifiers, newElement.AccessModifiers)
                .Returns(expected);

            var sut = new Wrapper(changeTable);

            var actual = sut.CompareItems(match, options)
                .ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();

            var result = actual.Single();

            result.Message.Should().Contain("protected internal");
            result.Message.Should().Contain("access modifiers");
            result.Message.Should().NotContain("sealed");
            result.Message.Should().NotContain("override");
        }

        private class Wrapper : AccessModifiersElementComparer<AccessModifiers>
        {
            public Wrapper(IChangeTable<AccessModifiers> changeTable) : base(changeTable)
            {
            }
        }
    }
}