namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ComparerOptionsTests
    {
        [Fact]
        public void AddAttributeNameToCompareAddsNewExpression()
        {
            var expression = new Regex("JsonProperty");

            var sut = new ComparerOptions();

            sut.AddAttributeNameToCompare(expression);

            var actual = sut.AttributeNamesToCompare;

            actual.Should().Contain(expression);
        }

        [Fact]
        public void AddAttributeNameToCompareReturnsEmptyWhenNoExpressionsAdded()
        {
            var sut = new ComparerOptions();

            var actual = sut.AttributeNamesToCompare;

            actual.Should().BeEmpty();
        }

        [Fact]
        public void AddAttributeNameToCompareThrowsExceptionWithNullNameExpression()
        {
            var sut = new ComparerOptions();

            Action action = () => sut.AddAttributeNameToCompare(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanSetCompareAttributes()
        {
            var sut = new ComparerOptions {CompareAttributes = AttributeCompareOption.All};

            sut.CompareAttributes.Should().Be(AttributeCompareOption.All);
        }

        [Fact]
        public void CanSetNewMessageFormatter()
        {
            var messageFormatter = Substitute.For<IMessageFormatter>();

            var sut = new ComparerOptions {MessageFormatter = messageFormatter};

            sut.MessageFormatter.Should().Be(messageFormatter);
        }

        [Fact]
        public void DefaultExpressionsDoNotMatchUnsupportedName()
        {
            var name = Guid.NewGuid().ToString();

            var sut = ComparerOptions.Default;

            var expressions = sut.AttributeNamesToCompare;

            expressions.Any(x => x.IsMatch(name)).Should().BeFalse();
        }

        [Theory]
        [InlineData("System.Text.Json.Serialization.JsonConverter")]
        [InlineData("System.Text.Json.Serialization.JsonExtensionData")]
        [InlineData("System.Text.Json.Serialization.JsonIgnore")]
        [InlineData("System.Text.Json.Serialization.JsonPropertyName")]
        [InlineData("Text.Json.Serialization.JsonConverter")]
        [InlineData("Text.Json.Serialization.JsonExtensionData")]
        [InlineData("Text.Json.Serialization.JsonIgnore")]
        [InlineData("Text.Json.Serialization.JsonPropertyName")]
        [InlineData("Json.Serialization.JsonConverter")]
        [InlineData("Json.Serialization.JsonExtensionData")]
        [InlineData("Json.Serialization.JsonIgnore")]
        [InlineData("Json.Serialization.JsonPropertyName")]
        [InlineData("Serialization.JsonConverter")]
        [InlineData("Serialization.JsonExtensionData")]
        [InlineData("Serialization.JsonIgnore")]
        [InlineData("Serialization.JsonPropertyName")]
        [InlineData("JsonConverter")]
        [InlineData("JsonExtensionData")]
        [InlineData("JsonIgnore")]
        [InlineData("JsonPropertyName")]
        [InlineData("Newtonsoft.Json.JsonObject")]
        [InlineData("Newtonsoft.Json.JsonArray")]
        [InlineData("Newtonsoft.Json.JsonDictionary")]
        [InlineData("Newtonsoft.Json.JsonProperty")]
        [InlineData("Newtonsoft.Json.JsonConverter")]
        [InlineData("Newtonsoft.Json.JsonExtensionData")]
        [InlineData("Newtonsoft.Json.JsonConstructor")]
        [InlineData("Json.JsonObject")]
        [InlineData("Json.JsonArray")]
        [InlineData("Json.JsonDictionary")]
        [InlineData("Json.JsonProperty")]
        [InlineData("Json.JsonConverter")]
        [InlineData("Json.JsonExtensionData")]
        [InlineData("Json.JsonConstructor")]
        [InlineData("JsonObject")]
        [InlineData("JsonArray")]
        [InlineData("JsonDictionary")]
        [InlineData("JsonProperty")]
        [InlineData("JsonConstructor")]
        [InlineData("System.Xml.Serialization.XmlAnyAttribute")]
        [InlineData("System.Xml.Serialization.XmlAnyElement")]
        [InlineData("System.Xml.Serialization.XmlArray")]
        [InlineData("System.Xml.Serialization.XmlArrayItem")]
        [InlineData("System.Xml.Serialization.XmlAttribute")]
        [InlineData("System.Xml.Serialization.XmlChoiceIdentifier")]
        [InlineData("System.Xml.Serialization.XmlElement")]
        [InlineData("System.Xml.Serialization.XmlEnum")]
        [InlineData("System.Xml.Serialization.XmlIgnore")]
        [InlineData("System.Xml.Serialization.XmlInclude")]
        [InlineData("System.Xml.Serialization.XmlRoot")]
        [InlineData("System.Xml.Serialization.XmlText")]
        [InlineData("System.Xml.Serialization.XmlType")]
        [InlineData("Xml.Serialization.XmlAnyAttribute")]
        [InlineData("Xml.Serialization.XmlAnyElement")]
        [InlineData("Xml.Serialization.XmlArray")]
        [InlineData("Xml.Serialization.XmlArrayItem")]
        [InlineData("Xml.Serialization.XmlAttribute")]
        [InlineData("Xml.Serialization.XmlChoiceIdentifier")]
        [InlineData("Xml.Serialization.XmlElement")]
        [InlineData("Xml.Serialization.XmlEnum")]
        [InlineData("Xml.Serialization.XmlIgnore")]
        [InlineData("Xml.Serialization.XmlInclude")]
        [InlineData("Xml.Serialization.XmlRoot")]
        [InlineData("Xml.Serialization.XmlText")]
        [InlineData("Xml.Serialization.XmlType")]
        [InlineData("Serialization.XmlAnyAttribute")]
        [InlineData("Serialization.XmlAnyElement")]
        [InlineData("Serialization.XmlArray")]
        [InlineData("Serialization.XmlArrayItem")]
        [InlineData("Serialization.XmlAttribute")]
        [InlineData("Serialization.XmlChoiceIdentifier")]
        [InlineData("Serialization.XmlElement")]
        [InlineData("Serialization.XmlEnum")]
        [InlineData("Serialization.XmlIgnore")]
        [InlineData("Serialization.XmlInclude")]
        [InlineData("Serialization.XmlRoot")]
        [InlineData("Serialization.XmlText")]
        [InlineData("Serialization.XmlType")]
        [InlineData("XmlAnyAttribute")]
        [InlineData("XmlAnyElement")]
        [InlineData("XmlArray")]
        [InlineData("XmlArrayItem")]
        [InlineData("XmlAttribute")]
        [InlineData("XmlChoiceIdentifier")]
        [InlineData("XmlElement")]
        [InlineData("XmlEnum")]
        [InlineData("XmlIgnore")]
        [InlineData("XmlInclude")]
        [InlineData("XmlRoot")]
        [InlineData("XmlText")]
        [InlineData("XmlType")]
        [InlineData("System.Runtime.Serialization.CollectionDataContract")]
        [InlineData("System.Runtime.Serialization.ContractNamespace")]
        [InlineData("System.Runtime.Serialization.DataContract")]
        [InlineData("System.Runtime.Serialization.DataMember")]
        [InlineData("System.Runtime.Serialization.EnumMember")]
        [InlineData("System.Runtime.Serialization.IgnoreDataMember")]
        [InlineData("System.Runtime.Serialization.KnownType")]
        [InlineData("System.Runtime.Serialization.OptionalField")]
        [InlineData("Runtime.Serialization.CollectionDataContract")]
        [InlineData("Runtime.Serialization.ContractNamespace")]
        [InlineData("Runtime.Serialization.DataContract")]
        [InlineData("Runtime.Serialization.DataMember")]
        [InlineData("Runtime.Serialization.EnumMember")]
        [InlineData("Runtime.Serialization.IgnoreDataMember")]
        [InlineData("Runtime.Serialization.KnownType")]
        [InlineData("Runtime.Serialization.OptionalField")]
        [InlineData("Serialization.CollectionDataContract")]
        [InlineData("Serialization.ContractNamespace")]
        [InlineData("Serialization.DataContract")]
        [InlineData("Serialization.DataMember")]
        [InlineData("Serialization.EnumMember")]
        [InlineData("Serialization.IgnoreDataMember")]
        [InlineData("Serialization.KnownType")]
        [InlineData("Serialization.OptionalField")]
        [InlineData("CollectionDataContract")]
        [InlineData("ContractNamespace")]
        [InlineData("DataContract")]
        [InlineData("DataMember")]
        [InlineData("EnumMember")]
        [InlineData("IgnoreDataMember")]
        [InlineData("KnownType")]
        [InlineData("OptionalField")]
        public void DefaultIncludesSerializationAttributeNamesToCompare(string name)
        {
            var sut = ComparerOptions.Default;

            var expressions = sut.AttributeNamesToCompare;

            expressions.Any(x => x.IsMatch(name)).Should().BeTrue();
        }

        [Fact]
        public void DefaultReturnsDefaultOptions()
        {
            var sut = ComparerOptions.Default;

            sut.MessageFormatter.Should().BeOfType<DefaultMessageFormatter>();
            sut.CompareAttributes.Should().Be(AttributeCompareOption.ByExpression);
        }

        [Fact]
        public void DefaultReturnsNewInstanceOnEachCall()
        {
            var first = ComparerOptions.Default;
            var second = ComparerOptions.Default;

            first.Should().NotBeSameAs(second);
        }
    }
}