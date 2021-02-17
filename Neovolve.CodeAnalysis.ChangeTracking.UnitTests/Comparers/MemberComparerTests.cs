namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Comparers;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class MemberComparerTests : Tests<MemberComparer<TestPropertyDefinition>>
    {
        private readonly ITestOutputHelper _output;

        public MemberComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareItemsReturnsEmptyWhenNodesMatch()
        {
            var oldMember = new TestPropertyDefinition();
            var newMember = oldMember.JsonClone();
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareReturnsBreakingWhenReturnTypeIsChangedAndParentTypesExist()
        {
            var oldGrandparent =
                new TestClassDefinition().Set(x => x.GenericTypeParameters = new List<string> {"TOld"}.AsReadOnly());
            var oldParent = new TestClassDefinition().Set(x => x.DeclaringType = oldGrandparent);
            var oldMember = new TestPropertyDefinition().Set(x =>
            {
                x.DeclaringType = oldParent;
                x.ReturnType = "string";
            });
            var newGrandparent =
                new TestClassDefinition().Set(x => x.GenericTypeParameters = new List<string> {"TNew"}.AsReadOnly());
            var newParent = new TestClassDefinition().Set(x => x.DeclaringType = newGrandparent);
            var newMember = new TestPropertyDefinition().Set(x =>
            {
                x.DeclaringType = newParent;
                x.ReturnType = "DateTime";
            });
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().NotBeEmpty();
            actual[0].ChangeType.Should().Be(SemVerChangeType.Breaking);
        }

        [Fact]
        public void CompareReturnsEmptyWhenReturnTypeIsRenamedGenericTypeOnGrandparentType()
        {
            var oldParent =
                new TestClassDefinition().Set(x => x.GenericTypeParameters = new List<string> {"TOld"}.AsReadOnly());
            var oldMember = new TestPropertyDefinition().Set(x =>
            {
                x.DeclaringType = oldParent;
                x.ReturnType = "TOld";
            });
            var newParent =
                new TestClassDefinition().Set(x => x.GenericTypeParameters = new List<string> {"TNew"}.AsReadOnly());
            var newMember = oldMember.JsonClone().Set(x =>
            {
                x.DeclaringType = newParent;
                x.ReturnType = "TNew";
            });
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void CompareReturnsEmptyWhenReturnTypeIsRenamedGenericTypeOnParentType()
        {
            var oldGrandparent =
                new TestClassDefinition().Set(x => x.GenericTypeParameters = new List<string> {"TOld"}.AsReadOnly());
            var oldParent = new TestClassDefinition().Set(x => x.DeclaringType = oldGrandparent);
            var oldMember = new TestPropertyDefinition().Set(x =>
            {
                x.DeclaringType = oldParent;
                x.ReturnType = "TOld";
            });
            var newGrandparent =
                new TestClassDefinition().Set(x => x.GenericTypeParameters = new List<string> {"TNew"}.AsReadOnly());
            var newParent = new TestClassDefinition().Set(x => x.DeclaringType = newGrandparent);
            var newMember = new TestPropertyDefinition().Set(x =>
            {
                x.DeclaringType = newParent;
                x.ReturnType = "TNew";
            });
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().BeEmpty();
        }

        [Theory]
        [InlineData("string", "string", null)]
        [InlineData("string", "DateTimeOffset", SemVerChangeType.Breaking)]
        public void CompareReturnsResultBasedOnReturnType(string oldValue, string newValue, SemVerChangeType? expected)
        {
            var oldMember = new TestPropertyDefinition()
                .Set(x => x.ReturnType = oldValue);
            var newMember = oldMember.JsonClone().Set(x => x.ReturnType = newValue);
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            if (expected == null)
            {
                actual.Should().BeEmpty();
            }
            else
            {
                actual[0].ChangeType.Should().Be(expected);
            }
        }

        [Fact]
        public void CompareReturnsResultFromAccessModifierComparer()
        {
            var oldMember = new TestPropertyDefinition();
            var newMember = oldMember.JsonClone();
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);
            var options = ComparerOptions.Default;
            var changeType = Model.Create<SemVerChangeType>();
            var message = Guid.NewGuid().ToString();
            var result = new ComparisonResult(changeType, oldMember, newMember, message);
            var results = new[] {result};

            Service<IAccessModifiersComparer>()
                .CompareItems(
                    Arg.Is<ItemMatch<IAccessModifiersElement<AccessModifiers>>>(x =>
                        x.OldItem == oldMember && x.NewItem == newMember), options).Returns(results);

            var actual = SUT.CompareItems(match, options).ToList();

            _output.WriteResults(actual);

            actual.Should().HaveCount(1);
            actual[0].Should().Be(result);
        }

        [Fact]
        public void CompareThrowsExceptionWithNullMatch()
        {
            var options = ComparerOptions.Default;

            Action action = () => SUT.CompareItems(null!, options);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CompareThrowsExceptionWithNullOptions()
        {
            var oldMember = new TestPropertyDefinition();
            var newMember = oldMember.JsonClone();
            var match = new ItemMatch<TestPropertyDefinition>(oldMember, newMember);

            Action action = () => SUT.CompareItems(match, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}