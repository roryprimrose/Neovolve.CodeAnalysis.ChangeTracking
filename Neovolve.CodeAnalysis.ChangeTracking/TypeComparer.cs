﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class TypeComparer : ElementComparer<ITypeDefinition>, ITypeComparer
    {
        private readonly IFieldMatchProcessor _fieldProcessor;
        private readonly IPropertyMatchProcessor _propertyProcessor;

        public TypeComparer(IFieldMatchProcessor fieldProcessor,
            IPropertyMatchProcessor propertyProcessor,
            IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
            _propertyProcessor = propertyProcessor ?? throw new ArgumentNullException(nameof(propertyProcessor));
            _fieldProcessor = fieldProcessor ?? throw new ArgumentNullException(nameof(fieldProcessor));
        }

        protected override IEnumerable<ComparisonResult> EvaluateMatch(ItemMatch<ITypeDefinition> match,
            ComparerOptions options)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

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

            foreach (var result in EvaluateGenericTypeDefinitionChanges(match, options))
            {
                yield return result;
            }

            foreach (var result in EvaluateFieldChanges(match, options))
            {
                yield return result;
            }

            foreach (var result in EvaluatePropertyChanges(match, options))
            {
                yield return result;
            }

            foreach (var result in EvaluateImplementedTypeChanges(match, options))
            {
                yield return result;
            }

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

        private static IEnumerable<ComparisonResult> EvaluateGenericConstraints(ItemMatch<ITypeDefinition> match,
            IConstraintListDefinition oldConstraintList,
            IConstraintListDefinition newConstraintList)
        {
            var oldConstraintCount = oldConstraintList?.Constraints.Count ?? 0;
            var newConstraintCount = newConstraintList?.Constraints.Count ?? 0;

            if (oldConstraintCount == 0
                && newConstraintCount == 0)
            {
                // There are no generic constraints defined on either type
                yield break;
            }

            if (oldConstraintCount == 0)
            {
                var message = $"{match.NewItem.Description} has added {newConstraintCount} generic type constraint";

                if (newConstraintCount != 1)
                {
                    message += "s";
                }

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                // No need to look into the constraints themselves
                yield break;
            }

            if (newConstraintCount == 0)
            {
                var message = $"{match.NewItem.Description} has removed {oldConstraintCount} generic type constraint";

                if (oldConstraintCount != 1)
                {
                    message += "s";
                }

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                // No need to look into the constraints themselves
                yield break;
            }

            // Find the old constraints that have been removed
            var removedConstraints = oldConstraintList!.Constraints.Except(newConstraintList!.Constraints);

            foreach (var constraint in removedConstraints)
            {
                var message = $"{match.NewItem.Description} has removed the generic type constraint {constraint}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match, message);
            }

            // Find the new constraints that have been added
            var addedConstraints = newConstraintList!.Constraints.Except(oldConstraintList!.Constraints);

            foreach (var constraint in addedConstraints)
            {
                var message = $"{match.NewItem.Description} has added the generic type constraint {constraint}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
        }

        private static IEnumerable<ComparisonResult> EvaluateGenericTypeDefinitionChanges(
            ItemMatch<ITypeDefinition> match, ComparerOptions options)
        {
            var oldTypeParameters = match.OldItem.GenericTypeParameters.ToList();
            var newTypeParameters = match.NewItem.GenericTypeParameters.ToList();

            var typeParameterShift =
                oldTypeParameters.Count - newTypeParameters.Count;

            if (typeParameterShift > 0)
            {
                // One or more generic type parameters have been removed
                var suffix = typeParameterShift == 1 ? "" : "s";
                var message =
                    $"{match.NewItem.Description} has removed {typeParameterShift} generic type parameter{suffix}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);

                // No need to look into how the generic type has changed
                yield break;
            }

            if (typeParameterShift < 0)
            {
                // One or more generic type parameters have been removed
                var shift = Math.Abs(typeParameterShift);
                var suffix = shift == 1 ? "" : "s";
                var message =
                    $"{match.NewItem.Description} has added {shift} generic type parameter{suffix}";

                // No need to look into how the generic type has changed
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }

            // We have the same number of generic types, evaluate the constraints
            for (var index = 0; index < oldTypeParameters.Count; index++)
            {
                var oldName = oldTypeParameters[index];
                var newName = newTypeParameters[index];

                var oldConstraints = match.OldItem.GenericConstraints.FirstOrDefault(x => x.Name == oldName);
                var newConstraints = match.NewItem.GenericConstraints.FirstOrDefault(x => x.Name == newName);

                foreach (var result in EvaluateGenericConstraints(match, oldConstraints, newConstraints))
                {
                    yield return result;
                }
            }
        }

        private static IEnumerable<ComparisonResult> EvaluateImplementedTypeChanges(ItemMatch<ITypeDefinition> match,
            ComparerOptions options)
        {
            // Find the old types that have been removed
            var removedTypes = match.OldItem.ImplementedTypes.Except(match.NewItem.ImplementedTypes);

            foreach (var removedType in removedTypes)
            {
                var message = $"{match.NewItem.Description} has removed the implemented type {removedType}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }

            // Find the new types that have been added
            var addedTypes = match.NewItem.ImplementedTypes.Except(match.OldItem.ImplementedTypes);

            foreach (var addedType in addedTypes)
            {
                var message = $"{match.NewItem.Description} has added the implemented type {addedType}";

                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match, message);
            }
        }

        private static IEnumerable<ComparisonResult> EvaluateModifierChanges(ItemMatch<ITypeDefinition> match)
        {
            var oldItem = match.OldItem as IClassDefinition;

            if (oldItem == null)
            {
                yield break;
            }

            var newItem = (IClassDefinition) match.NewItem;

            if (oldItem.IsAbstract
                && newItem.IsAbstract == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has removed the abstract keyword");
            }
            else if (oldItem.IsAbstract == false
                     && newItem.IsAbstract)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has added the abstract keyword");
            }

            if (oldItem.IsSealed
                && newItem.IsSealed == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Feature, match,
                    $"{newItem.Description} has removed the sealed keyword");
            }
            else if (oldItem.IsSealed == false
                     && newItem.IsSealed)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has added the sealed keyword");
            }

            if (oldItem.IsStatic
                && newItem.IsStatic == false)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has removed the static keyword");
            }
            else if (oldItem.IsStatic == false
                     && newItem.IsStatic)
            {
                yield return ComparisonResult.ItemChanged(SemVerChangeType.Breaking, match,
                    $"{newItem.Description} has added the static keyword");
            }
        }

        private IEnumerable<ComparisonResult> EvaluateFieldChanges(ItemMatch<ITypeDefinition> match,
            ComparerOptions options)
        {
            var oldClass = match.OldItem as IClassDefinition;

            if (oldClass == null)
            {
                return Array.Empty<ComparisonResult>();
            }

            var newClass = (IClassDefinition) match.NewItem;

            return _fieldProcessor.CalculateChanges(oldClass.Fields, newClass.Fields, options);
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