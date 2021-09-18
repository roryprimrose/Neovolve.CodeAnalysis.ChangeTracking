namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public enum PropertyAccessorAccessModifiers
    {
        /// <summary>
        ///     No access modifier has been defined so the property access inherits the access modifier of the property.
        /// </summary>
        None = 0,

        Private,
        Internal,
        Protected,
        ProtectedInternal
    }
}