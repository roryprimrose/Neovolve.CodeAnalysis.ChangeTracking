namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Evaluators;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class MethodMatchEvaluatorTests
    {
        [Fact]
        public void
            FindMatchesDoesNotReturnsMatchOnRenamedMethodWhenMultipleSourceOptionsAvailable()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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
            var newMethod = new TestMethodDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldMethod.GenericTypeParameters;
                x.Parameters = oldMethod.Parameters;
            });
            var otherOldMethod = new TestMethodDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldMethod.GenericTypeParameters;
                x.Parameters = oldMethod.Parameters;
            });

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod,
                otherOldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().HaveCount(1);
            actual.ItemsAdded.First().Should().BeEquivalentTo(newMethod);
            actual.ItemsRemoved.Should().HaveCount(2);
            actual.ItemsRemoved.First().Should().BeEquivalentTo(oldMethod);
            actual.ItemsRemoved.Skip(1).First().Should().BeEquivalentTo(otherOldMethod);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesDoesNotReturnsMatchOnRenamedMethodWhenMultipleTargetOptionsAvailable()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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
            var newMethod = new TestMethodDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldMethod.GenericTypeParameters;
                x.Parameters = oldMethod.Parameters;
            });
            var otherNewMethod = new TestMethodDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldMethod.GenericTypeParameters;
                x.Parameters = oldMethod.Parameters;
            });

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod,
                otherNewMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().HaveCount(2);
            actual.ItemsAdded.First().Should().BeEquivalentTo(newMethod);
            actual.ItemsAdded.Skip(1).First().Should().BeEquivalentTo(otherNewMethod);
            actual.ItemsRemoved.Should().HaveCount(1);
            actual.ItemsRemoved.First().Should().BeEquivalentTo(oldMethod);
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void FindMatchesReturnsAllMethodsAsMatchesWhenNoChangesMade()
        {
            var oldItems = new List<IMethodDefinition>
            {
                new TestMethodDefinition(),
                new TestMethodDefinition(),
                new TestMethodDefinition(),
                new TestMethodDefinition()
            };
            var newItems = new List<IMethodDefinition>(oldItems);

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Select(x => x.NewItem).Should().BeEquivalentTo(newItems);
            actual.MatchingItems.Select(x => x.OldItem).Should().BeEquivalentTo(oldItems);
        }

        [Fact]
        public void FindMatchesReturnsEmptyWhenNoMethodsProvided()
        {
            var oldItems = Array.Empty<IMethodDefinition>();
            var newItems = Array.Empty<IMethodDefinition>();

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().BeEmpty();
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnMethodWithGenericTypeConstraints()
        {
            var method = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var oldItems = new List<IMethodDefinition>
            {
                method
            };
            var newItems = new List<IMethodDefinition>
            {
                method
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(method);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(method);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnMethodWithGenericTypeConstraintsChanged()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey"
                },
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

            var newMethod = new TestMethodDefinition
            {
                RawName = oldMethod.RawName,
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldMethod);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newMethod);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnMethodWithGenericTypeParameterAndParameterCountChange()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var newMethod = new TestMethodDefinition
            {
                RawName = oldMethod.RawName
            };

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldMethod);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newMethod);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnMethodWithoutGenericTypeConstraintsOrParameters()
        {
            var method = new TestMethodDefinition
            {
                GenericTypeParameters = Array.Empty<string>(),
                Parameters = Array.Empty<IParameterDefinition>()
            };

            var oldItems = new List<IMethodDefinition>
            {
                method
            };
            var newItems = new List<IMethodDefinition>
            {
                method
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(method);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(method);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnMethodWithParameterCountChange()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var newMethod = new TestMethodDefinition
            {
                RawName = oldMethod.RawName,
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldMethod);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newMethod);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnMethodWithParameterTypeChange()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var newMethod = new TestMethodDefinition
            {
                RawName = oldMethod.RawName,
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldMethod);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newMethod);
        }

        [Fact]
        public void
            FindMatchesReturnsMatchOnRenamedMethod()
        {
            var oldMethod = new TestMethodDefinition
            {
                GenericTypeParameters = new List<string>
                {
                    "TKey",
                    "TValue"
                },
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
            var newMethod = new TestMethodDefinition().Set(x =>
            {
                x.GenericTypeParameters = oldMethod.GenericTypeParameters;
                x.Parameters = oldMethod.Parameters;
            });

            var oldItems = new List<IMethodDefinition>
            {
                oldMethod
            };
            var newItems = new List<IMethodDefinition>
            {
                newMethod
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(oldMethod);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(newMethod);
        }

        [Fact]
        public void
            FindMatchesReturnsOverloadMethodAsMatchesWhenParametersMatchWithOtherOverloadAdded()
        {
            var matchingOverload = new TestMethodDefinition
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
            var otherOverload = new TestMethodDefinition
            {
                RawName = matchingOverload.RawName,
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

            var oldItems = new List<IMethodDefinition>
            {
                matchingOverload
            };
            var newItems = new List<IMethodDefinition>
            {
                otherOverload,
                matchingOverload
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().HaveCount(1);
            actual.ItemsAdded.First().Should().BeEquivalentTo(otherOverload);
            actual.ItemsRemoved.Should().BeEmpty();
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(matchingOverload);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(matchingOverload);
        }

        [Fact]
        public void
            FindMatchesReturnsOverloadMethodAsMatchesWhenParametersMatchWithOtherOverloadRemoved()
        {
            var matchingOverload = new TestMethodDefinition
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
            var otherOverload = new TestMethodDefinition
            {
                RawName = matchingOverload.RawName,
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

            var oldItems = new List<IMethodDefinition>
            {
                otherOverload,
                matchingOverload
            };
            var newItems = new List<IMethodDefinition>
            {
                matchingOverload
            };

            var sut = new MethodMatchEvaluator();

            var actual = sut.FindMatches(oldItems, newItems);

            actual.ItemsAdded.Should().BeEmpty();
            actual.ItemsRemoved.Should().HaveCount(1);
            actual.ItemsRemoved.First().Should().BeEquivalentTo(otherOverload);
            actual.MatchingItems.Should().HaveCount(1);
            actual.MatchingItems.First().OldItem.Should().BeEquivalentTo(matchingOverload);
            actual.MatchingItems.First().NewItem.Should().BeEquivalentTo(matchingOverload);
        }
    }
}