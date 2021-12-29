namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;

    /// <summary>
    ///     The <see cref="ElementMatchOptions" />
    ///     enum defines the options for comparing <see cref="IElementDefinition" /> types to look for matching definitions.
    /// </summary>
    [Flags]
    public enum ElementMatchOptions
    {
        /// <summary>
        ///     Identifies that all aspects of the definition should be evaluated.
        /// </summary>
        All = 0,

        /// <summary>
        ///     Identifies that the definition type should be ignored when evaluating definitions.
        /// </summary>
        IgnoreType = 1,

        /// <summary>
        ///     Identifies that the namespace should be ignored when evaluating definitions.
        /// </summary>
        IgnoreNamespace = 1,

        /// <summary>
        ///     Identifies that the name should be ignored when evaluating definitions.
        /// </summary>
        IgnoreName = 2,

        /// <summary>
        ///     Identifies that the value should be ignored when evaluating definitions.
        /// </summary>
        IgnoreValue = 4
    }
}