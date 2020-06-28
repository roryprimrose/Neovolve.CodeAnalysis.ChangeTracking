namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    /// <summary>
    ///     The <see cref="DefinitionLocation" />
    ///     class describes the location of a code definition.
    /// </summary>
    public class DefinitionLocation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefinitionLocation" /> class.
        /// </summary>
        /// <param name="filePath">The filepath that defines the definition.</param>
        /// <param name="lineIndex">The line index where the definition begins.</param>
        /// <param name="characterIndex">The character index where the definition begins.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="filePath"/> parameter is <c>null</c>.</exception>
        public DefinitionLocation(string filePath, int lineIndex, int characterIndex)
        {
            FilePath = filePath?.Trim() ?? throw new ArgumentNullException(nameof(filePath));
            LineIndex = lineIndex;
            CharacterIndex = characterIndex;
        }

        /// <summary>
        ///     Gets the character index that the declaration starts on in the containing code.
        /// </summary>
        public int CharacterIndex { get; }

        /// <summary>
        ///     Gets the file path that contains the declaration.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        ///     Gets the line index that the declaration starts on in the containing code.
        /// </summary>
        public int LineIndex { get; }
    }
}