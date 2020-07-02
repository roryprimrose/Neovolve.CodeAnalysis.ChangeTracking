namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IClassDefinition" />
    ///     interface defines the members that describe a class.
    /// </summary>
    public interface IClassDefinition : ITypeDefinition
    {
        /// <summary>
        ///     Gets the fields declared on the class.
        /// </summary>
        IReadOnlyCollection<IFieldDefinition> Fields { get; }

        /// <summary>
        ///     Gets whether the class is an abstract class.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        ///     Gets whether the class is a partial class.
        /// </summary>
        bool IsPartial { get; }

        /// <summary>
        ///     Gets whether the class is a sealed class.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        ///     Gets whether the class is a static class.
        /// </summary>
        bool IsStatic { get; }
    }
}