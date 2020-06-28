namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Globalization;

    public class MemberDefinition
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        ///     Returns the string representation of the type.
        /// </summary>
        /// <param name="includeMemberType"><c>true</c> to include the member type as a prefix; otherwise <c>false</c>.</param>
        /// <returns>The string representation of the type.</returns>
        public string ToString(bool includeMemberType)
        {
            var prefix = string.Empty;

            if (includeMemberType)
            {
                prefix = MemberType + " ";
            }

            if (string.IsNullOrWhiteSpace(Namespace))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}", prefix, OwningType, Name);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}.{3}", prefix, Namespace,
                OwningType, Name);
        }

        /// <summary>
        ///     Gets or sets the attributes declared on the member.
        /// </summary>
        public ICollection<OldAttributeDefinition> Attributes { get; } = new List<OldAttributeDefinition>();

        /// <summary>
        ///     Gets or sets the character index that the member declaration starts on in the containing code.
        /// </summary>
        public int CharacterIndex { get; set; }

        /// <summary>
        ///     Gets or sets the file path that contains the member.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets whether the member is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        ///     Gets or sets the line index that the member declaration starts on in the containing code.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        ///     Gets or sets the value that identifies the type of member (such as Property, Field or Attribute).
        /// </summary>
        public MemberType MemberType { get; set; } = MemberType.Unknown;

        /// <summary>
        ///     Gets or sets the name of the member.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the namespace of the member.
        /// </summary>
        public string? Namespace { get; set; }

        /// <summary>
        ///     Gets or sets the type that declares the member.
        /// </summary>
        public string OwningType { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the type returned by the member.
        /// </summary>
        public string ReturnType { get; set; } = string.Empty;
    }
}