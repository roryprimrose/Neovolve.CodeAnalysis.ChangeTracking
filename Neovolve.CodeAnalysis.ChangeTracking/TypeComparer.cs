namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeComparer : ElementComparer<ITypeDefinition>, ITypeComparer
    {
        private readonly IPropertyMatchProcessor _propertyProcessor;

        public TypeComparer(IPropertyMatchProcessor propertyProcessor, IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _propertyProcessor = propertyProcessor ?? throw new ArgumentNullException(nameof(propertyProcessor));
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<ITypeDefinition> match,
            ComparerOptions options)
        {
            Ensure.Any.IsNotNull(match, nameof(match));
            Ensure.Any.IsNotNull(options, nameof(options));

            // Check for a change in type
            if (match.OldItem.GetType() != match.NewItem.GetType())
            {
                var newType = DetermineTypeName(match.NewItem);

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{match.OldItem.Description} has changed to {newType}");

                // This is a fundamental change to the type. No point continuing to identify differences
                yield break;
            }

            foreach (var comparisonResult in EvaluateModifierChanges(match))
            {
                yield return comparisonResult;
            }

            foreach (var result in EvaluatePropertyChanges(match, options))
            {
                yield return result;
            }

            // Compare the following:
            // generic type definitions
            // implemented types
            // generic constraints
            // attributes
            // fields

            yield return ComparisonResult.NoChange(match);
        }

        private static string DetermineTypeName(ITypeDefinition item)
        {
            if (item is IClassDefinition)
            {
                return "class";
            }

            if (item is IInterfaceDefinition)
            {
                return "interface";
            }

            throw new NotSupportedException("Unknown type provided");
        }

        private static IEnumerable<ComparisonResult> EvaluateModifierChanges(ItemMatch<ITypeDefinition> match)
        {
            var oldClass = match.OldItem as IClassDefinition;

            if (oldClass == null)
            {
                yield break;
            }

            var newClass = (IClassDefinition) match.NewItem;

            if (oldClass.IsAbstract
                && newClass.IsAbstract == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{oldClass.Description} has removed the abstract keyword");
            }
            else if (oldClass.IsAbstract == false
                     && newClass.IsAbstract)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has added the abstract keyword");
            }

            if (oldClass.IsSealed
                && newClass.IsSealed == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{oldClass.Description} has removed the sealed keyword");
            }
            else if (oldClass.IsSealed == false
                     && newClass.IsSealed)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has added the sealed keyword");
            }

            if (oldClass.IsStatic
                && newClass.IsStatic == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has removed the static keyword");
            }
            else if (oldClass.IsStatic == false
                     && newClass.IsStatic)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{oldClass.Description} has added the static keyword");
            }
        }

        private IEnumerable<ComparisonResult> EvaluatePropertyChanges(ItemMatch<ITypeDefinition> match,
            ComparerOptions options)
        {
            var oldProperties = match.OldItem.Properties;
            var newProperties = match.NewItem.Properties;

            return _propertyProcessor.CalculateChanges(oldProperties, newProperties, options);
        }
    }
}