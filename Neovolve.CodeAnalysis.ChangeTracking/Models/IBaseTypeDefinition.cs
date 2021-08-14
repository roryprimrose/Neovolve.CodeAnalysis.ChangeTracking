namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public interface IBaseTypeDefinition : IAccessModifiersElement<AccessModifiers>
    {
        /// <summary>
        ///     Gets or sets the type that declares the member.
        /// </summary>
        ITypeDefinition? DeclaringType { get; set; }

        /// <summary>
        ///     Gets the namespace of the type.
        /// </summary>
        string Namespace { get; }
    }
}