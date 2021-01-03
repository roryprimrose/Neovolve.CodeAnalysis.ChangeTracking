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

    public class ModifiersElementComparerTests
    {
        private readonly ITestOutputHelper _output;

        public ModifiersElementComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsResultForChangedModifiers()
        {
            var options = new ComparerOptions();
            var fullName = Guid.NewGuid().ToString();
            var expected = SemVerChangeType.Feature;

            var changeTable = Substitute.For<IClassModifiersChangeTable>();
            var oldElement = Substitute.For<IClassDefinition>();
            var newElement = Substitute.For<IClassDefinition>();

            oldElement.Modifiers.Returns(ClassModifiers.AbstractPartial);
            oldElement.DeclaredModifiers.Returns("public abstract partial");
            newElement.FullName.Returns(fullName);
            newElement.Modifiers.Returns(ClassModifiers.StaticPartial);
            newElement.DeclaredModifiers.Returns("internal static partial");

            var match = new ItemMatch<IModifiersElement<ClassModifiers>>(oldElement, newElement);

            changeTable.CalculateChange(oldElement.Modifiers, newElement.Modifiers)
                .Returns(expected);

            var sut = new ModifiersElementComparer<ClassModifiers>(changeTable);

            var actual = sut.CompareItems(match, options)
                .ToList();

            actual.Should().NotBeEmpty();

            var result = actual.Single();

            _output.WriteLine(result.Message);

            result.Message.Should().Contain("abstract partial");
            result.Message.Should().Contain("static partial");
            result.Message.Should().Contain("modifiers");
            result.Message.Should().NotContain("access modifiers");
            result.Message.Should().NotContain("public");
            result.Message.Should().NotContain("internal");
        }
    }
}