namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class AccessModifiersElementComparer<T> : ModifiersComparerBase<T>, IAccessModifiersElementComparer<T>
        where T : struct, Enum
    {
        protected AccessModifiersElementComparer(IChangeTable<T> changeTable) : base(changeTable)
        {
        }

        public IEnumerable<ComparisonResult> CompareMatch(
            ItemMatch<IAccessModifiersElement<T>> match,
            ComparerOptions options)
        {
            var convertedMatch = new ItemMatch<IElementDefinition>(match.OldItem, match.NewItem);

            return CompareMatch(convertedMatch, match.OldItem.AccessModifiers, match.NewItem.AccessModifiers, options);
        }

        protected override string GetDeclaredModifiers(IElementDefinition element)
        {
            return element.GetDeclaredAccessModifiers();
        }

        protected override string ModifierLabel { get; } = "access modifier";
    }
}