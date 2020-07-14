﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    /// <summary>
    ///     The <see cref="IFieldDefinition" />
    ///     interface defines the members that describe a field.
    /// </summary>
    public interface IFieldDefinition : IMemberDefinition
    {
        /// <summary>
        ///     Gets the property modifiers.
        /// </summary>
        FieldModifiers Modifiers { get; }
    }
}