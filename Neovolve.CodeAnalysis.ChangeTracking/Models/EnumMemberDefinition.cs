namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    ///     The <see cref="EnumMemberDefinition" />
    ///     class describes a member defined on an enum.
    /// </summary>
    public class EnumMemberDefinition : ElementDefinition, IEnumMemberDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumMemberDefinition" /> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the field.</param>
        /// <param name="node">The node that defines the argument.</param>
        /// <param name="index">The index of the member in declaration order.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="node" /> parameter is <c>null</c>.</exception>
        public EnumMemberDefinition(IEnumDefinition declaringType, EnumMemberDeclarationSyntax node, int index) :
            base(node)
        {
            DeclaringType = declaringType;
            var name = node.Identifier.Text;

            Name = name;
            RawName = name;
            FullName = declaringType.FullName + "." + name;
            FullRawName = declaringType.FullRawName + "." + name;
            IsVisible = declaringType.IsVisible;
            Index = index;

            if (node.EqualsValue == null)
            {
                Value = string.Empty;
            }
            else
            {
                Value = node.EqualsValue.Value.ToString();
            }
        }

        public override bool Matches(IElementDefinition element, ElementMatchOptions options)
        {
            if (element is not IEnumMemberDefinition item)
            {
                return false;
            }
            
            if (options.HasFlag(ElementMatchOptions.IgnoreValue) == false)
            {
                if (Value != item.Value)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Value)
                    && Index != item.Index)
                {
                    // In this case both enum members do not define a value
                    // This means the value is implicit according to the index of the member in the enum
                    return false;
                }
            }

            if (base.Matches(element, options) == false)
            {
                return false;
            }

            return true;
        }

        public IEnumDefinition DeclaringType { get; set; }

        public override string FullName { get; }
        public override string FullRawName { get; }
        public int Index { get; set; }

        public override string Name { get; }
        public override string RawName { get; }
        public string Value { get; set; }
    }
}