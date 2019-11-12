namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Globalization;

    public class MemberDefinition
    {
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Namespace))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}: {1}.{2}", GetType().Name, OwningType, Name);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}: {1}.{2}.{3}", GetType().Name, Namespace,
                OwningType, Name);
        }

        public ICollection<AttributeDefinition> Attributes { get; } = new List<AttributeDefinition>();

        public bool IsPublic { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string OwningType { get; set; }

        public string ReturnType { get; set; }
    }
}