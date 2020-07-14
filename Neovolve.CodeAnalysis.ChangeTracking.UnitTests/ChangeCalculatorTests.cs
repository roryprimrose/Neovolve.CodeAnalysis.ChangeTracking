namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public class ChangeCalculatorTests
    {
        private readonly ILogger _logger;

        public ChangeCalculatorTests(ITestOutputHelper output)
        {
            _logger = output.BuildLogger();
        }

        //[Fact]
        //public void CalculateChangesReturnsBreakingWhenClassRemoved()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var removedClass = new TestClassDefinition();
        //    var removedClasses = new List<IClassDefinition> {removedClass};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    classMatches.ItemsRemoved.Returns(removedClasses);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //}

        //[Fact]
        //public void CalculateChangesReturnsBreakingWhenInterfaceRemoved()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var removedInterface = new TestInterfaceDefinition();
        //    var removedInterfaces = new List<IInterfaceDefinition> {removedInterface};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    interfaceMatches.ItemsRemoved.Returns(removedInterfaces);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //}

        //[Fact]
        //public void CalculateChangesReturnsBreakingWhenMatchingClassRemovesOldItem()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var matchingClass = new TestClassDefinition();
        //    var itemChanged = new TestPropertyDefinition();
        //    var match = new ItemMatch<ITypeDefinition>(matchingClass, matchingClass);
        //    var matches = new List<ItemMatch<ITypeDefinition>> {match};
        //    var result = ComparisonResult.ItemRemoved(itemChanged);
        //    var results = new List<ComparisonResult> {result};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    classMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //    actual.ComparisonResults.Should().Contain(result);
        //}

        //[Fact]
        //public void CalculateChangesReturnsBreakingWhenMatchingInterfaceRemovesOldItem()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var matchingInterface = new TestInterfaceDefinition();
        //    var itemChanged = new TestPropertyDefinition();
        //    var match = new ItemMatch<ITypeDefinition>(matchingInterface, matchingInterface);
        //    var matches = new List<ItemMatch<ITypeDefinition>> {match};
        //    var result = ComparisonResult.ItemRemoved(itemChanged);
        //    var results = new List<ComparisonResult> {result};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    interfaceMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(interfaceMatches, classMatches);
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Breaking);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //    actual.ComparisonResults.Should().Contain(result);
        //}

        //[Fact]
        //public void CalculateChangesReturnsFeatureWhenClassAdded()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var addedClass = new TestClassDefinition();
        //    var addedClasses = new List<IClassDefinition> {addedClass};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    classMatches.ItemsAdded.Returns(addedClasses);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //}

        //[Fact]
        //public void CalculateChangesReturnsFeatureWhenInterfaceAdded()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var addedInterface = new TestInterfaceDefinition();
        //    var addedInterfaces = new List<IInterfaceDefinition> {addedInterface};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    interfaceMatches.ItemsAdded.Returns(addedInterfaces);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //}

        //[Fact]
        //public void CalculateChangesReturnsFeatureWhenMatchingClassHasNewItem()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var matchingClass = new TestClassDefinition();
        //    var itemChanged = new TestPropertyDefinition();
        //    var match = new ItemMatch<ITypeDefinition>(matchingClass, matchingClass);
        //    var matches = new List<ItemMatch<ITypeDefinition>> {match};
        //    var result = ComparisonResult.ItemAdded(itemChanged);
        //    var results = new List<ComparisonResult> {result};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    classMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //    actual.ComparisonResults.Should().Contain(result);
        //}

        //[Fact]
        //public void CalculateChangesReturnsFeatureWhenMatchingInterfaceAddsNewItem()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var matchingInterface = new TestInterfaceDefinition();
        //    var itemChanged = new TestPropertyDefinition();
        //    var match = new ItemMatch<ITypeDefinition>(matchingInterface, matchingInterface);
        //    var matches = new List<ItemMatch<ITypeDefinition>> {match};
        //    var result = ComparisonResult.ItemAdded(itemChanged);
        //    var results = new List<ComparisonResult> {result};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    interfaceMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(interfaceMatches, classMatches);
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.Feature);
        //    actual.ComparisonResults.Should().HaveCount(1);
        //    actual.ComparisonResults.Should().Contain(result);
        //}

        //[Theory]
        //[InlineData("MyNamespace.MyClass", "MyNamespace.SomeOtherClass", false)]
        //[InlineData("MyNamespace.MyClass", "MyNamespace.MyClass", true)]
        //public void CalculateChangesMatchesTypesUsingFullName(string firstName, string secondName, bool expected)
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var firstClass = new TestClassDefinition().Set(x => x.FullName = firstName);
        //    var secondClass = new TestClassDefinition().Set(x => x.FullName = secondName);
        //    var match = new ItemMatch<ITypeDefinition>(firstClass, secondClass);
        //    var matches = new List<ItemMatch<ITypeDefinition>> { match };
        //    var result = ComparisonResult.NoChange(match);
        //    var results = new List<ComparisonResult> { result };

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    classMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);
        //    evaluator.When(x => x.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>())).Do(x =>
        //    {
        //        var actual = x.Arg<Func<ITypeDefinition, ITypeDefinition, bool>>()(firstClass, secondClass);

        //        actual.Should().Be(expected);
        //    });
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.None);
        //    actual.ComparisonResults.Should().BeEmpty();
        //}

        //[Fact]
        //public void CalculateChangesReturnsNoChangeWhenMatchingClassFound()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var matchingClass = new TestClassDefinition();
        //    var match = new ItemMatch<ITypeDefinition>(matchingClass, matchingClass);
        //    var matches = new List<ItemMatch<ITypeDefinition>> {match};
        //    var result = ComparisonResult.NoChange(match);
        //    var results = new List<ComparisonResult> {result};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    classMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(classMatches, interfaceMatches);
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.None);
        //    actual.ComparisonResults.Should().BeEmpty();
        //}

        //[Fact]
        //public void CalculateChangesReturnsNoChangeWhenMatchingInterfaceFound()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var matchingInterface = new TestInterfaceDefinition();
        //    var match = new ItemMatch<ITypeDefinition>(matchingInterface, matchingInterface);
        //    var matches = new List<ItemMatch<ITypeDefinition>> {match};
        //    var result = ComparisonResult.NoChange(match);
        //    var results = new List<ComparisonResult> {result};

        //    var classMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<ITypeDefinition>>();
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    interfaceMatches.MatchingItems.Returns(matches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<IEnumerable<ITypeDefinition>>(),
        //        Arg.Any<Func<ITypeDefinition, ITypeDefinition, bool>>()).Returns(interfaceMatches, classMatches);
        //    comparer.CompareItems(match, options).Returns(results);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.None);
        //    actual.ComparisonResults.Should().BeEmpty();
        //}

        //[Fact]
        //public void CalculateChangesReturnsNoChangeWithEmptyTypes()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();
        //    var newTypes = Array.Empty<TestClassDefinition>();
        //    var classMatches = Substitute.For<IMatchResults<IClassDefinition>>();
        //    var interfaceMatches = Substitute.For<IMatchResults<IInterfaceDefinition>>();

        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<IClassDefinition>>(),
        //        Arg.Any<IEnumerable<IClassDefinition>>(),
        //        Arg.Any<Func<IClassDefinition, IClassDefinition, bool>>()).Returns(classMatches);
        //    evaluator.MatchItems(
        //        Arg.Any<IEnumerable<IInterfaceDefinition>>(),
        //        Arg.Any<IEnumerable<IInterfaceDefinition>>(),
        //        Arg.Any<Func<IInterfaceDefinition, IInterfaceDefinition, bool>>()).Returns(interfaceMatches);

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    var actual = sut.CalculateChanges(oldTypes, newTypes, options);

        //    actual.ChangeType.Should().Be(SemVerChangeType.None);
        //    actual.ComparisonResults.Should().BeEmpty();
        //}

        //[Fact]
        //public void CalculateChangesThrowsExceptionWithNullNewTypes()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var oldTypes = Array.Empty<TestClassDefinition>();

        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    Action action = () => sut.CalculateChanges(oldTypes, null!, options);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //public void CalculateChangesThrowsExceptionWithNullOldTypes()
        //{
        //    var options = Model.UsingModule<ConfigurationModule>().Create<ComparerOptions>();
        //    var newTypes = Array.Empty<TestClassDefinition>();

        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    var sut = new ChangeCalculator(evaluator, comparer, _logger);

        //    Action action = () => sut.CalculateChanges(null!, newTypes, options);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //[SuppressMessage(
        //    "Usage",
        //    "CA1806:Do not ignore method results",
        //    Justification = "Testing constructor guard clause")]
        //public void DoesNotThrowExceptionWhenCreatedWithNullLogger()
        //{
        //    var evaluator = Substitute.For<IMatchEvaluator>();
        //    var comparer = Substitute.For<ITypeComparer>();

        //    // ReSharper disable once ObjectCreationAsStatement
        //    Action action = () => new ChangeCalculator(evaluator, comparer, null);

        //    action.Should().NotThrow();
        //}

        //[Fact]
        //[SuppressMessage(
        //    "Usage",
        //    "CA1806:Do not ignore method results",
        //    Justification = "Testing constructor guard clause")]
        //public void ThrowsExceptionWhenCreatedWithNullComparer()
        //{
        //    var evaluator = Substitute.For<IMatchEvaluator>();

        //    // ReSharper disable once ObjectCreationAsStatement
        //    Action action = () => new ChangeCalculator(evaluator, null!, _logger);

        //    action.Should().Throw<ArgumentNullException>();
        //}

        //[Fact]
        //[SuppressMessage(
        //    "Usage",
        //    "CA1806:Do not ignore method results",
        //    Justification = "Testing constructor guard clause")]
        //public void ThrowsExceptionWhenCreatedWithNullEvaluator()
        //{
        //    var comparer = Substitute.For<ITypeComparer>();

        //    // ReSharper disable once ObjectCreationAsStatement
        //    Action action = () => new ChangeCalculator(null!, comparer, _logger);

        //    action.Should().Throw<ArgumentNullException>();
        //}
    }
}