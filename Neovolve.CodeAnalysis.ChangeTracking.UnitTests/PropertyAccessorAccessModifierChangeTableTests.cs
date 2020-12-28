namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using FluentAssertions;
    using ModelBuilder;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.UnitTests.TestModels;
    using Xunit;

    public class PropertyAccessorAccessModifierChangeTableTests
    {
        [Theory]
        [InlineData(PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.None, SemVerChangeType.None)]
        [InlineData(PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.Internal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.Private, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifier.None, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.None, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.Internal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.Private, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifier.Internal, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.None, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.Internal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.Private, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.Protected, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifier.Private, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.Feature )]
        [InlineData(PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.None, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.Protected, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.Protected, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.None, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.Internal, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.Private, SemVerChangeType.Breaking )]
        [InlineData(PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.Protected, SemVerChangeType.None )]
        [InlineData(PropertyAccessorAccessModifier.ProtectedInternal, PropertyAccessorAccessModifier.ProtectedInternal, SemVerChangeType.None )]
        public void CalculateChangeReturnsExpectedValue(
            PropertyAccessorAccessModifier oldModifiers,
            PropertyAccessorAccessModifier newModifiers,
            SemVerChangeType expected)
        {
            var oldMember = new TestPropertyAccessorDefinition().Set(x => x.AccessModifier = oldModifiers);
            var newMember = new TestPropertyAccessorDefinition().Set(x => x.AccessModifier = newModifiers);
            var match = new ItemMatch<IPropertyAccessorDefinition>(oldMember, newMember);

            var actual = PropertyAccessorAccessModifierChangeTable.CalculateChange(match);

            actual.Should().Be(expected);
        }
    }
}