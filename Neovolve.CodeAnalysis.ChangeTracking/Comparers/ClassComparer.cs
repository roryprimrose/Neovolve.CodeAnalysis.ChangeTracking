﻿namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class ClassComparer : TypeComparer<IClassDefinition>, IClassComparer
    {
        private readonly IClassModifiersComparer _classModifiersComparer;
        private readonly IFieldMatchProcessor _fieldProcessor;

        public ClassComparer(IAccessModifiersComparer accessModifiersComparer,
            IClassModifiersComparer classModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IMethodMatchProcessor methodProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer,
            genericTypeElementComparer,
            propertyProcessor,
            methodProcessor,
            attributeProcessor)
        {
            _classModifiersComparer =
                classModifiersComparer ?? throw new ArgumentNullException(nameof(classModifiersComparer));
            _fieldProcessor = fieldProcessor ?? throw new ArgumentNullException(nameof(fieldProcessor));
        }

        protected override void EvaluateChildElementChanges(ItemMatch<IClassDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateChildElementChanges(match, options, aggregator);

            RunComparisonStep(EvaluateFieldChanges, match, options, aggregator);
        }

        protected override void EvaluateModifierChanges(ItemMatch<IClassDefinition> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateModifierChanges(match, options, aggregator);

            RunComparisonStep(EvaluateClassModifierChanges, match, options, aggregator);
        }

        private void EvaluateClassModifierChanges(
            ItemMatch<IClassDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch =
                new ItemMatch<IModifiersElement<ClassModifiers>>(match.OldItem, match.NewItem);

            var results = _classModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }

        private void EvaluateFieldChanges(
            ItemMatch<IClassDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var changes = _fieldProcessor.CalculateChanges(match.OldItem.Fields, match.NewItem.Fields, options);

            aggregator.AddResults(changes);
        }
    }
}