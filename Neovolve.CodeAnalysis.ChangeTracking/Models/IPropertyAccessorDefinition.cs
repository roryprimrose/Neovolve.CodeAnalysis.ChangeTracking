namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public interface IPropertyAccessorDefinition : IAccessModifiersElement<PropertyAccessorAccessModifiers>
    {
        PropertyAccessorPurpose AccessorPurpose { get; }
        PropertyAccessorType AccessorType { get; }
    }
}