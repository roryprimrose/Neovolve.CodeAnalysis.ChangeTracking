namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class AggregateTypeComparer : IBaseTypeComparer
    {
        private readonly IClassComparer _classComparer;
        private readonly IEnumComparer _enumComparer;
        private readonly IInterfaceComparer _interfaceComparer;
        private readonly IStructComparer _structComparer;

        public AggregateTypeComparer(IClassComparer classComparer, IInterfaceComparer interfaceComparer,
            IStructComparer structComparer, IEnumComparer enumComparer)
        {
            _classComparer = classComparer ?? throw new ArgumentNullException(nameof(classComparer));
            _interfaceComparer = interfaceComparer ?? throw new ArgumentNullException(nameof(interfaceComparer));
            _structComparer = structComparer ?? throw new ArgumentNullException(nameof(structComparer));
            _enumComparer = enumComparer ?? throw new ArgumentNullException(nameof(enumComparer));
        }

        public IEnumerable<ComparisonResult> CompareMatch(ItemMatch<IBaseTypeDefinition> match, ComparerOptions options)
        {
            match = match ?? throw new ArgumentNullException(nameof(match));
            options = options ?? throw new ArgumentNullException(nameof(options));

            // Check for a change in type
            if (match.OldItem.GetType() != match.NewItem.GetType())
            {
                var newType = DetermineTypeChangeDescription(match.NewItem);

                var args = new FormatArguments(
                    "has changed to {NewValue}",
                    match.OldItem.FullName,
                    null,
                    newType);

                var message = options.MessageFormatter.FormatMatch(match, ItemFormatType.ItemChanged, args);

                var result = new ComparisonResult(
                    SemVerChangeType.Breaking,
                    match.OldItem, match.NewItem,
                    message);

                // We are not going to process any other changes
                return new[]
                {
                    result
                };
            }

            if (match.OldItem is IClassDefinition oldClass
                && match.NewItem is IClassDefinition newClass)
            {
                var itemMatch = new ItemMatch<IClassDefinition>(oldClass, newClass);

                return _classComparer.CompareMatch(itemMatch, options);
            }

            if (match.OldItem is IStructDefinition oldStruct
                && match.NewItem is IStructDefinition newStruct)
            {
                var itemMatch = new ItemMatch<IStructDefinition>(oldStruct, newStruct);

                return _structComparer.CompareMatch(itemMatch, options);
            }

            if (match.OldItem is IInterfaceDefinition oldInterface
                && match.NewItem is IInterfaceDefinition newInterface)
            {
                var itemMatch = new ItemMatch<IInterfaceDefinition>(oldInterface, newInterface);

                return _interfaceComparer.CompareMatch(itemMatch, options);
            }

            if (match.OldItem is IEnumDefinition oldEnum
                && match.NewItem is IEnumDefinition newEnum)
            {
                var itemMatch = new ItemMatch<IEnumDefinition>(oldEnum, newEnum);

                return _enumComparer.CompareMatch(itemMatch, options);
            }

            throw new NotSupportedException(
                $"There is no {nameof(IBaseTypeComparer<IBaseTypeDefinition>)} implementation for {match.OldItem.GetType()}");
        }

        private static string DetermineTypeChangeDescription(IBaseTypeDefinition item)
        {
            if (item is IClassDefinition)
            {
                return "class";
            }

            if (item is IStructDefinition)
            {
                return "struct";
            }

            if (item is IInterfaceDefinition)
            {
                return "interface";
            }

            if (item is IEnumDefinition)
            {
                return "enum";
            }

            throw new NotSupportedException("Unknown type provided");
        }
    }
}