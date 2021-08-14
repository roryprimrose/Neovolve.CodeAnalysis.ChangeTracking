namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public interface IEnumMemberDefinition : IElementDefinition
    {
        /// <summary>
        ///     Gets or sets the type that declares the member.
        /// </summary>
        public IEnumDefinition DeclaringType { get; set; }
        
        /// <summary>
        ///     Gets or sets the value of the member.
        /// </summary>
        public string Value { get; set; }
    }
}