namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="ComparerOptions" />
    ///     class describes the options that control how code is compared.
    /// </summary>
    public class ComparerOptions
    {
        private readonly List<Regex> _attributesNamesToCompare = new List<Regex>();

        /// <summary>
        ///     Adds an expression to the list of attribute names to compare.
        /// </summary>
        /// <param name="nameExpression">The name expression to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="nameExpression" /> parameter is <c>null</c>.</exception>
        public void AddAttributeNameToCompare(Regex nameExpression)
        {
            nameExpression = nameExpression ?? throw new ArgumentNullException(nameof(nameExpression));

            _attributesNamesToCompare.Add(nameExpression);
        }

        private static ComparerOptions BuildDefaultOptions()
        {
            var options = new ComparerOptions
            {
                CompareAttributes = AttributeCompareOption.ByExpression
            };

            // Register System.Text.Json attributes that impact how json data is serialized
            options.AddAttributeNameToCompare("((((System\\.)?Text\\.)?Json\\.)?Serialization\\.)?JsonConverter");
            options.AddAttributeNameToCompare("((((System\\.)?Text\\.)?Json\\.)?Serialization\\.)?JsonExtensionData");
            options.AddAttributeNameToCompare("((((System\\.)?Text\\.)?Json\\.)?Serialization\\.)?JsonIgnore");
            options.AddAttributeNameToCompare("((((System\\.)?Text\\.)?Json\\.)?Serialization\\.)?JsonPropertyName");

            // Register Newtonsoft.Json attributes that impact how json data is serialized
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonObject");
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonArray");
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonDictionary");
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonProperty");
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonConverter");
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonExtensionData");
            options.AddAttributeNameToCompare("((Newtonsoft\\.)?Json\\.)?JsonConstructor");

            // Register System.Xml.Serialization attributes that impact how xml data is serialized
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlAnyAttribute");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlAnyElement");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlArray");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlArrayItem");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlAttribute");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlChoiceIdentifier");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlElement");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlEnum");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlIgnore");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlInclude");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlRoot");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlText");
            options.AddAttributeNameToCompare("(((System\\.)?Xml\\.)?Serialization\\.)?XmlType");

            // Register System.Runtime.Serialization attributes that impact how xml data is serialized
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?CollectionDataContract");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?ContractNamespace");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?DataContract");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?DataMember");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?EnumMember");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?IgnoreDataMember");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?KnownType");
            options.AddAttributeNameToCompare("(((System\\.)?Runtime\\.)?Serialization\\.)?OptionalField");

            return options;
        }

        private void AddAttributeNameToCompare(string expression)
        {
            var regex = new Regex(expression);

            AddAttributeNameToCompare(regex);
        }

        /// <summary>
        ///     Gets the default comparer options.
        /// </summary>
        public static ComparerOptions Default => BuildDefaultOptions();

        /// <summary>
        ///     Gets the name expressions that identify attributes to compare.
        /// </summary>
        public IEnumerable<Regex> AttributeNamesToCompare => _attributesNamesToCompare.AsReadOnly();

        /// <summary>
        ///     Gets or sets the message formatter creates the type change messages.
        /// </summary>
        public IMessageFormatter MessageFormatter { get; set; } = new DefaultMessageFormatter();

        /// <summary>
        ///     Determines whether attribute changes should be evaluated.
        /// </summary>
        public AttributeCompareOption CompareAttributes { get; set; } = AttributeCompareOption.Skip;
    }
}