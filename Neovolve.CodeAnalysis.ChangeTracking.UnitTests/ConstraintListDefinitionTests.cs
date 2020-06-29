namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public class ConstraintListDefinitionTests
    {
        [Fact]
        public async Task GenericConstraintsReturnsDeclaredConstraints()
        {
            var node = await TestNode
                .FindNode<TypeParameterConstraintClauseSyntax>(TypeDefinitionCode.ClassWithGenericConstraints)
                .ConfigureAwait(false);

            var actual = new ConstraintListDefinition(node);

            actual.Name.Should().Be("T");
            actual.Constraints.Should().HaveCount(2);
            actual.Constraints.First().Should().Be("Stream");
            actual.Constraints.Skip(1).First().Should().Be("new()");
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullNode()
        {
            Action action = () => new ConstraintListDefinition(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}