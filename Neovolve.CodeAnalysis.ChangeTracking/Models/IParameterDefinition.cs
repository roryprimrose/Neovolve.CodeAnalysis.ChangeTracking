namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public interface IParameterDefinition : IModifiersElement<ParameterModifiers>
    {
        /// <summary>
        ///     Gets the method that declares this parameter.
        /// </summary>
        IMethodDefinition DeclaringMethod { get; }

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