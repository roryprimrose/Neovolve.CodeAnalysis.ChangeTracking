namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;
    using Xunit.Abstractions;

    public class AttributeEvaluatorTests
    {
        private readonly ITestOutputHelper _output;

        public AttributeEvaluatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void FindMatchesIdentifiesAttributesNotMatching()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var oldMatchingAttribute = Model.UsingModule<ConfigurationModule>().Create<IAttributeDefinition>();
            var oldAttributes = new[]
            {
                oldAttribute, oldMatchingAttribute
            };
            var newMatchingAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x =>
                {
                    x.Name = oldMatchingAttribute.Name;
                    x.Arguments = oldMatchingAttribute.Arguments;
                });
            var newAttributes = new[]
            {
                newMatchingAttribute, newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldMatchingAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newMatchingAttribute);
            results.ItemsAdded.Should().HaveCount(1);
            results.ItemsAdded.First().Should().Be(newAttribute);
            results.ItemsRemoved.Should().HaveCount(1);
            results.ItemsRemoved.First().Should().Be(oldAttribute);
        }

        [Theory]
        [InlineData("MyName", "MyName", true)]
        [InlineData("MyNameAttribute", "MyNameAttribute", true)]
        [InlineData("MyNameAttribute", "MyName", true)]
        [InlineData("MyName", "MyNameAttribute", true)]
        [InlineData("MyName", "myname", false)]
        [InlineData("MyName", "SomeOtherName", false)]
        public void FindMatchesReturnsSingleAttributeMatchingByName(string firstName, string secondName, bool expected)
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = firstName);
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => { x.Name = secondName; });
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            if (expected)
            {
                results.MatchingItems.Should().HaveCount(1);
                results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
                results.MatchingItems.First().NewItem.Should().Be(newAttribute);
                results.ItemsAdded.Should().BeEmpty();
                results.ItemsRemoved.Should().BeEmpty();
            }
            else
            {
                results.MatchingItems.Should().BeEmpty();
            }
        }

        [Fact]
        public void FindMatchesReturnsSingleAttributeMatchingByNameIgnoringAttributeSuffix()
        {
            var oldAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = "SomethingAttribute");
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newAttribute = Model.UsingModule<ConfigurationModule>().Create<TestAttributeDefinition>()
                .Set(x => x.Name = "Something");
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            results.MatchingItems.Should().HaveCount(1);
            results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
            results.MatchingItems.First().NewItem.Should().Be(newAttribute);
            results.ItemsAdded.Should().BeEmpty();
            results.ItemsRemoved.Should().BeEmpty();
        }

        [Theory]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")", true, "Arguments match")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true, second: \"changed\")", true, "Changed named argument value")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true, third: \"anothervalue\")", true, "Changed named argument parameter name")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, third: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true", true, "Removed named parameter")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true)",
            "SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")", true, "Added named argument")]
        [InlineData("SimpleAttribute(\"changed\", 123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\"", true, "Changed ordinal argument value")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")",
            "OtherAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")", false, "Different attribute name")]
        [InlineData("SimpleAttribute(123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")", true, "Added ordinal argument")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(123, first: true, second: \"anothervalue\"", true, "Removed ordinal argument")]
        [InlineData("SimpleAttribute(\"stringValue\", first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", 123, first: true)", true, "Added ordinal, removed named argument")]
        [InlineData("SimpleAttribute(\"stringValue\", 123, first: true, second: \"anothervalue\")",
            "SimpleAttribute(\"stringValue\", first: true, second: \"anothervalue\", third: 554)", true, "Removed ordinal, added named argument")]
        public async Task FindMatchesReturnsMatchesByArguments(string oldCode, string newCode,
            bool expected, string scenario)
        {
            _output.WriteLine(scenario);

            var oldNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute.Replace("SimpleAttribute", oldCode))
                .ConfigureAwait(false);
            var declaringElement = new TestClassDefinition();
            var oldAttribute = new AttributeDefinition(oldNode, declaringElement);
            var oldAttributes = new[]
            {
                oldAttribute
            };
            var newNode = await TestNode
                .FindNode<AttributeSyntax>(AttributeDefinitionCode.SimpleAttribute.Replace("SimpleAttribute", newCode))
                .ConfigureAwait(false);
            var newAttribute = new AttributeDefinition(newNode, declaringElement);
            var newAttributes = new[]
            {
                newAttribute
            };

            var sut = new AttributeEvaluator();

            var results = sut.FindMatches(oldAttributes, newAttributes);

            if (expected)
            {
                results.MatchingItems.Should().HaveCount(1);
                results.MatchingItems.First().OldItem.Should().Be(oldAttribute);
                results.MatchingItems.First().NewItem.Should().Be(newAttribute);
                results.ItemsAdded.Should().BeEmpty();
                results.ItemsRemoved.Should().BeEmpty();
            }
            else
            {
                results.MatchingItems.Should().BeEmpty();
            }
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullNewItems()
        {
            var oldItems = Array.Empty<AttributeDefinition>();

            var sut = new AttributeEvaluator();

            Action action = () => sut.FindMatches(oldItems, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void FindMatchesThrowsExceptionWithNullOldItems()
        {
            var newItems = Array.Empty<AttributeDefinition>();

            var sut = new AttributeEvaluator();

            Action action = () => sut.FindMatches(null!, newItems);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}