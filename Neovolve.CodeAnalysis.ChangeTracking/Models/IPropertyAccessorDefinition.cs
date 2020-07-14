namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public interface IPropertyAccessorDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets the access modifier for the property accessor.
        /// </summary>
        PropertyAccessorAccessModifier AccessModifier { get; }
    }
}