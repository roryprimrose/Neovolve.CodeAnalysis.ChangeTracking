namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class MethodComparer : MemberComparer<IMethodDefinition>, IMethodComparer
    {
        private readonly IMethodModifiersChangeTable _methodModifiersChangeTable;
        private readonly IParameterComparer _parameterComparer;

        public MethodComparer(IMethodModifiersChangeTable methodModifiersChangeTable,
            IParameterComparer parameterComparer, IAccessModifiersComparer accessModifiersComparer,
            IAttributeMatchProcessor attributeProcessor) : base(
            accessModifiersComparer, attributeProcessor)
        {
            _parameterComparer = parameterComparer ?? throw new ArgumentNullException(nameof(parameterComparer));
            _methodModifiersChangeTable = methodModifiersChangeTable
                                          ?? throw new ArgumentNullException(nameof(methodModifiersChangeTable));
        }

        protected override void EvaluateMatch(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            RunComparisonStep(EvaluateMethodModifierChanges, match, options, aggregator);
            RunComparisonStep(EvaluateMethodNameChanges, match, options, aggregator);
            RunComparisonStep(EvaluateGenericTypeDefinitionChanges, match, options, aggregator);
            RunComparisonStep(EvaluateParameterChanges, match, options, aggregator);
        }

        private static void EvaluateGenericConstraints(
            ItemMatch<IMethodDefinition> match,
            IConstraintListDefinition? oldConstraintList,
            IConstraintListDefinition? newConstraintList,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldConstraintCount = oldConstraintList?.Constraints.Count ?? 0;
            var newConstraintCount = newConstraintList?.Constraints.Count ?? 0;

            if (oldConstraintCount == 0
                && newConstraintCount == 0)
            {
                // There are no generic constraints defined on either type
                return;
            }

            if (oldConstraintCount == 0)
            {
                var suffix = string.Empty;

                if (newConstraintCount != 1)
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has added {newConstraintCount} generic type constraint{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into the constraints themselves
                return;
            }

            if (newConstraintCount == 0)
            {
                var suffix = string.Empty;

                if (oldConstraintCount != 1)
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has removed {oldConstraintCount} generic type constraint{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into the constraints themselves
                return;
            }

            // Find the old constraints that have been removed
            var removedConstraints = oldConstraintList!.Constraints.Except(newConstraintList!.Constraints);

            foreach (var constraint in removedConstraints)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} generic type constraint",
                    match.NewItem.FullName,
                    constraint,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Feature, match, options.MessageFormatter, args);
            }

            // Find the new constraints that have been added
            var addedConstraints = newConstraintList!.Constraints.Except(oldConstraintList!.Constraints);

            foreach (var constraint in addedConstraints)
            {
                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} generic type constraint",
                    match.NewItem.FullName,
                    null,
                    constraint);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
            }
        }

        private static void EvaluateGenericTypeDefinitionChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldTypeParameters = match.OldItem.GenericTypeParameters.FastToList();
            var newTypeParameters = match.NewItem.GenericTypeParameters.FastToList();

            var typeParameterShift = oldTypeParameters.Count - newTypeParameters.Count;

            if (typeParameterShift > 0)
            {
                // One or more generic type parameters have been removed
                var suffix = typeParameterShift == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has removed {typeParameterShift} generic type parameter{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            if (typeParameterShift < 0)
            {
                // One or more generic type parameters have been added
                var shift = Math.Abs(typeParameterShift);
                var suffix = shift == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has added {shift} generic type parameter{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            // We have the same number of generic types, evaluate the constraints
            for (var index = 0; index < oldTypeParameters.Count; index++)
            {
                var oldName = oldTypeParameters[index];
                var newName = newTypeParameters[index];

                var oldConstraints = match.OldItem.GenericConstraints.FirstOrDefault(x => x.Name == oldName);
                var newConstraints = match.NewItem.GenericConstraints.FirstOrDefault(x => x.Name == newName);

                EvaluateGenericConstraints(match, oldConstraints, newConstraints, options, aggregator);
            }
        }

        private static void EvaluateMethodNameChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            if (match.OldItem.Name == match.NewItem.Name)
            {
                return;
            }

            var args = new FormatArguments(
                "{DefinitionType} {Identifier} has been renamed from {OldValue}",
                match.NewItem.FullName,
                match.OldItem.Name,
                null);

            aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);
        }

        private void EvaluateMethodModifierChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var change = _methodModifiersChangeTable.CalculateChange(match.OldItem.Modifiers, match.NewItem.Modifiers);

            if (change == SemVerChangeType.None)
            {
                return;
            }

            var newModifiers = match.NewItem.GetDeclaredModifiers();
            var oldModifiers = match.OldItem.GetDeclaredModifiers();

            if (string.IsNullOrWhiteSpace(oldModifiers))
            {
                // Modifiers have been added where there were previously none defined
                var suffix = string.Empty;

                if (newModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has added the {NewValue} modifier" + suffix,
                    match.NewItem.FullName,
                    null,
                    newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else if (string.IsNullOrWhiteSpace(newModifiers))
            {
                // All previous modifiers have been removed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    "{DefinitionType} {Identifier} has removed the {OldValue} modifier" + suffix,
                    match.NewItem.FullName,
                    oldModifiers,
                    null);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
            else
            {
                // Modifiers have been changed
                var suffix = string.Empty;

                if (oldModifiers.Contains(" "))
                {
                    // There is more than one modifier
                    suffix = "s";
                }

                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has changed the modifier{suffix} from {{OldValue}} to {{NewValue}}",
                    match.NewItem.FullName,
                    oldModifiers,
                    newModifiers);

                aggregator.AddElementChangedResult(change, match, options.MessageFormatter, args);
            }
        }

        private void EvaluateParameterChanges(
            ItemMatch<IMethodDefinition> match,
            ComparerOptions options,
            IChangeResultAggregator aggregator)
        {
            var oldParameters = match.OldItem.Parameters.FastToList();
            var newParameters = match.NewItem.Parameters.FastToList();

            var parameterShift = oldParameters.Count - newParameters.Count;

            if (parameterShift > 0)
            {
                // One or more generic type parameters have been removed
                var suffix = parameterShift == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has removed {parameterShift} parameter{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            if (parameterShift < 0)
            {
                // One or more generic type parameters have been added
                var shift = Math.Abs(parameterShift);
                var suffix = shift == 1 ? "" : "s";
                var args = new FormatArguments(
                    $"{{DefinitionType}} {{Identifier}} has added {shift} parameter{suffix}",
                    match.NewItem.FullName,
                    null,
                    null);

                aggregator.AddElementChangedResult(SemVerChangeType.Breaking, match, options.MessageFormatter, args);

                // No need to look into how the generic type has changed
                return;
            }

            // We have the same number of parameters, compare them
            for (var index = 0; index < oldParameters.Count; index++)
            {
                var oldParameter = oldParameters[index];
                var newParameter = newParameters[index];
                var parameterMatch = new ItemMatch<IParameterDefinition>(oldParameter, newParameter);

                var parameterChanges = _parameterComparer.CompareItems(parameterMatch, options);

                aggregator.AddResults(parameterChanges);
            }
        }
    }
}