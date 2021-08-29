namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;
    
    /// <summary>
    ///     The <see cref="IEnumDefinition" />
    ///     interface defines the members that describe a enum.
    /// </summary>
    public interface IEnumDefinition : IBaseTypeDefinition<EnumAccessModifiers>
    {
        /// <summary>
        ///     Gets the members defined on this type.
        /// </summary>
        IReadOnlyCollection<IEnumMemberDefinition> Members { get; }
    }
}