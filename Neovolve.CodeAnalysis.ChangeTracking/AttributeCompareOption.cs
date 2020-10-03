namespace Neovolve.CodeAnalysis.ChangeTracking
{
    /// <summary>
    ///     The <see cref="AttributeCompareOption" />
    ///     enum defines the options for how attributes are to be compared.
    /// </summary>
    public enum AttributeCompareOption
    {
        /// <summary>
        ///     Identifies that no attributes will be compared.
        /// </summary>
        Skip = 0,

        /// <summary>
        ///     Identifies that attributes that have names matching an expression in
        ///     <see cref="ComparerOptions.AttributeNamesToCompare" /> will be compared.
        /// </summary>
        ByExpression,

        /// <summary>
        ///     Identifies that all attributes will be compared.
        /// </summary>
        All
    }
}