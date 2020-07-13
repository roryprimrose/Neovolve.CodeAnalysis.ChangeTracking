namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using FluentAssertions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class IsVisibleExtensionsTests
    {
        [Fact]
        public void IsVisibleForTypeThrowsExceptionWithNullDeclaration()
        {
            Action action = () => ((TypeDeclarationSyntax) null!).IsVisible(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}