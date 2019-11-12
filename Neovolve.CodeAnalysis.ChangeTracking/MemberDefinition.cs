namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Globalization;

    public class MemberDefinition
    {
        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool includeMemberType)
        {
            var prefix = string.Empty;

            if (includeMemberType)
            {
                prefix = MemberType + ": ";
            }

            if (string.IsNullOrWhiteSpace(Namespace))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}", prefix, OwningType, Name);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}.{3}", prefix, Namespace,
                OwningType, Name);
        }

        public ICollection<AttributeDefinition> Attributes { get; } = new List<AttributeDefinition>();

        public bool IsPublic { get; set; }

        public string MemberType { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string OwningType { get; set; }

        public string ReturnType { get; set; }
    }
}