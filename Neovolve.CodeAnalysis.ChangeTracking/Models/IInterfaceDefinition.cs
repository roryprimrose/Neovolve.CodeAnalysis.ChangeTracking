namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    /// The <see cref="IInterfaceDefinition"/>
    /// interface defines the members that describe an interface.
    /// </summary>
    public interface IInterfaceDefinition : ITypeDefinition, IModifiersElement<InterfaceModifiers>
    {
    }
}