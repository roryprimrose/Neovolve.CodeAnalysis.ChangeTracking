namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public abstract class MemberComparer<T> : ElementComparer<T>, IMemberComparer<T> where T : IMemberDefinition
    {
        private readonly IAccessModifiersComparer _accessModifiersComparer;

        protected MemberComparer(
            IAccessModifiersComparer accessModifiersComparer, IAttributeMatchProcessor attributeProcessor) : base(
            attributeProcessor)
        {
            _accessModifiersComparer = accessModifiersComparer
                                       ?? throw new ArgumentNullException(nameof(accessModifiersComparer));
        }

        protected override void EvaluateAccessModifierChanges(
            ItemMatch<T> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var convertedMatch = new ItemMatch<IAccessModifiersElement<AccessModifiers>>(match.OldItem, match.NewItem);

            var results = _accessModifiersComparer.CompareMatch(convertedMatch, options);

            aggregator.AddResults(results);
        }

        protected override void EvaluateSignatureChanges(ItemMatch<T> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            base.EvaluateSignatureChanges(match, options, aggregator);

            RunComparisonStep(EvaluateReturnTypeChanges, match, options, aggregator, true);
        }

        private static void EvaluateReturnTypeChanges(ItemMatch<T> match, ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldType = match.OldItem.ReturnType;
            var newType = match.NewItem.ReturnType;

            IGenericTypeElement deepestNewGenericTypeElement;
            IGenericTypeElement deepestOldGenericTypeElement;

            // We need to check whether the element itself can declare generic type parameters
            // If not, the declaring type will be used for generic type parameter mapping
            if (match.OldItem is IGenericTypeElement oldElement
                && match.NewItem is IGenericTypeElement newElement)
            {
                deepestOldGenericTypeElement = oldElement;
                deepestNewGenericTypeElement = newElement;
            }
            else
            {
                deepestOldGenericTypeElement = match.OldItem.DeclaringType;
                deepestNewGenericTypeElement = match.NewItem.DeclaringType;
            }

            var oldMappedType =
                deepestOldGenericTypeElement.GetMatchingGenericType(oldType, deepestNewGenericTypeElement);

            if (oldMappedType == newType)
            {
                return;
            }

            // The return type has changed but we need to figure out how
            // If the member previously returned void then this is a feature because binary compatibility hasn't been broken
            // Any other change would be breaking however

            var changeType = oldMappedType == "void" ? SemVerChangeType.Feature : SemVerChangeType.Breaking;

            var args = new FormatArguments(
                "return type has changed from {OldValue} to {NewValue}", match.OldItem.ReturnType,
                match.NewItem.ReturnType);

            aggregator.AddElementChangedResult(changeType, match, options.MessageFormatter, args);
        }
    }
}