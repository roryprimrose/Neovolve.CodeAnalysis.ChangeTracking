namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class PropertyAccessorComparer : ElementComparer<IPropertyAccessorDefinition>, IPropertyAccessorComparer
    {
        private readonly IPropertyAccessorAccessModifiersComparer _propertyAccessorAccessModifiersComparer;

        public PropertyAccessorComparer(
            IPropertyAccessorAccessModifiersComparer propertyAccessorAccessModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _propertyAccessorAccessModifiersComparer = propertyAccessorAccessModifiersComparer
                                                       ?? throw new ArgumentNullException(
                                                           nameof(propertyAccessorAccessModifiersComparer));
        }

        protected override void EvaluateAccessModifierChanges(
            ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IAccessModifiersElement<PropertyAccessorAccessModifiers>>(match.OldItem, match.NewItem);

            var results = _propertyAccessorAccessModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }

        protected override void EvaluateTypeDefinitionChanges(ItemMatch<IPropertyAccessorDefinition> match,
            ComparerOptions options, IChangeResultAggregator aggregator)
        {
            // Check for switches between set and init
            base.EvaluateTypeDefinitionChanges(match, options, aggregator);

            if (match.OldItem.AccessorPurpose != PropertyAccessorPurpose.Write)
            {
                // This is a get accessor
                return;
            }

            if (match.OldItem.AccessorType == match.NewItem.AccessorType)
            {
                // These accessors are the same
                return;
            }

            // Here we have a switch between set and init
            if (match.OldItem.AccessorType == PropertyAccessorType.Set
                && match.NewItem.AccessorType == PropertyAccessorType.Init)
            {
                // This has changed from an open write to a write on initialization
                var args = new FormatArguments(
                    $"has changed from {MessagePart.OldValue} to {MessagePart.NewValue}",
                    "set",
                    "init");

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
            else if (match.OldItem.AccessorType == PropertyAccessorType.Init
                     && match.NewItem.AccessorType == PropertyAccessorType.Set)
            {
                // This has changed from an open write to a write on initialization
                var args = new FormatArguments(
                    $"has changed from {MessagePart.OldValue} to {MessagePart.NewValue}",
                    "init",
                    "set");

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }
        }
    }
}