namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using NSubstitute;
    using Xunit;

    public class ConstructorEvaluatorTests : Tests<ConstructorEvaluator>
    {
        [Fact]
        public void FindMatchesReturnsAllConstructorsAsMatchesWhenNoChangesMade()
        {
            var oldItems = new List<IConstructorDefinition>
            {
                new TestConstructorDefinition
                {
                    Parameters = new []
                    {
                        new TestParameterDefinition()
                    }
                },
                new TestConstructorDefinition
                {
                    Parameters = new []
                    {
                        new TestParameterDefinition()
                    }
                },
                new TestConstructorDefinition
                {
                    Parameters = new []
                    {
                        new TestParameterDefinition()
                    }
                },
                new TestConstructorDefinition
                {
                    Parameters = new []
                    {
                        new TestParameterDefinition()
                    }
                }
            };
            var newItems = new List<IConstructorDefinition>(oldItems);

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Select(x => x.NewItem).Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Select(x => x.OldItem).Should().BeEquivalentTo(oldItems);
        }

        [Fact]
        public void
            FindMatchesReturnsConstructorAsMatchesWhenParametersMatchWithOtherConstructorAdded()
        {
            var matchingConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "List<string>"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };
            var otherConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                matchingConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                otherConstructor,
                matchingConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().HaveCount(1);
            actual.ItemsAdded.First().Should().BeEquivalentTo(otherConstructor);
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(matchingConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(matchingConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsConstructorAsMatchesWhenParametersMatchWithOtherConstructorRemoved()
        {
            var matchingConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "List<string>"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };
            var otherConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                otherConstructor,
                matchingConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                matchingConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().HaveCount(1);
            actual.ItemsRemoved.First().Should().BeEquivalentTo(otherConstructor);
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(matchingConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(matchingConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsEmptyWhenDefaultConstructorAddedAndNoOtherConstructorsDefined()
        {
            var oldItems = Array.Empty<IConstructorDefinition>();
            var newConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.None,
                Parameters = new List<IParameterDefinition>()
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsEmptyWhenDefaultConstructorRemovedAndNoOtherConstructorsDefined()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.None,
                Parameters = new List<IParameterDefinition>()
            };
            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = Array.Empty<IConstructorDefinition>();

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsEmptyWhenNoConstructorsProvided()
        {
            var oldItems = Array.Empty<IConstructorDefinition>();
            var newItems = Array.Empty<IConstructorDefinition>();

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsEmptyWhenStaticConstructorAddedAndNoOtherConstructorsDefined()
        {
            var oldItems = Array.Empty<IConstructorDefinition>();
            var newConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.Static,
                Parameters = new List<IParameterDefinition>()
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsEmptyWhenStaticConstructorRemovedAndNoOtherConstructorsDefined()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.Static,
                Parameters = new List<IParameterDefinition>()
            };
            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = Array.Empty<IConstructorDefinition>();

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnConstructorWhenParameterCountChanged()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var newConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnConstructorWhenParameterTypeChanged()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var newConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "DateTime"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnConstructorWithModifierChange()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.Static,
                Parameters = new List<IParameterDefinition>()
            };

            var newConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.None,
                Parameters = new List<IParameterDefinition>()
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnConstructorWithParameterCountChange()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var newConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnConstructorWithParameterTypeChange()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var newConstructor = new TestConstructorDefinition
            {
                RawName = oldConstructor.RawName,
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "int"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldConstructor);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newConstructor);
        }

        [Fact]
        public void
            FindMatchesReturnsNoMatchOnAddedConstructorWithParameters()
        {
            var newConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = Array.Empty<IConstructorDefinition>();
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().HaveCount(1);
            actual.ItemsAdded.First().Should().BeEquivalentTo(newConstructor);
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsNoMatchOnRemovedConstructorWithParameters()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = Array.Empty<IConstructorDefinition>();

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().HaveCount(1);
            actual.ItemsRemoved.First().Should().BeEquivalentTo(oldConstructor);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsNoMatchOnRemovedInstanceConstructorWithParametersAndAddedStaticConstructor()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.None,
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var newConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.Static,
                Parameters = new List<IParameterDefinition>()
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().Contain(newConstructor);
            actual.ItemsRemoved.Should().Contain(oldConstructor);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsNoMatchOnRemovedStaticConstructorAndAddedInstanceConstructorWithParameters()
        {
            var oldConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.Static,
                Parameters = new List<IParameterDefinition>()
            };

            var newConstructor = new TestConstructorDefinition
            {
                Modifiers = ConstructorModifiers.None,
                Parameters = new List<IParameterDefinition>
                {
                    new TestParameterDefinition
                    {
                        Type = "string"
                    },
                    new TestParameterDefinition
                    {
                        Type = "Guid"
                    },
                    new TestParameterDefinition
                    {
                        Type = "CancellationToken"
                    }
                }
            };

            var oldItems = new List<IConstructorDefinition>
            {
                oldConstructor
            };
            var newItems = new List<IConstructorDefinition>
            {
                newConstructor
            };

            var sut = new ConstructorEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().Contain(newConstructor);
            actual.ItemsRemoved.Should().Contain(oldConstructor);
            actual.MatchingItems.Should().BeEmpty();
        }
    }
}