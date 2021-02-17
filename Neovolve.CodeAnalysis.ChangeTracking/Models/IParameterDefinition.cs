namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public interface IParameterDefinition : IModifiersElement<ParameterModifiers>
    {
        /// <summary>
        ///     Gets the member that declares this parameter.
        /// </summary>
        IMemberDefinition DeclaringMember { get; }

        /// <summary>
        ///     Gets the default value declared on the parameter.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        ///     Gets the parameter type.
        /// </summary>
        string Type { get; }
    }
}