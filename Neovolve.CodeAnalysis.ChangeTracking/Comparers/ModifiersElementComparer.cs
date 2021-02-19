namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using System;
    using System.Collections.Generic;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public abstract class ModifiersElementComparer<T> : ModifiersComparerBase<T>, IModifiersElementComparer<T>
        where T : struct, Enum
    {
        protected ModifiersElementComparer(IChangeTable<T> changeTable) : base(changeTable)
        {
        }

        public IEnumerable<ComparisonResult> CompareItems(
            ItemMatch<IModifiersElement<T>> match,
            ComparerOptions options)
        {
            var convertedMatch = new ItemMatch<IElementDefinition>(match.OldItem, match.NewItem);

            return CompareItems(convertedMatch, match.OldItem.Modifiers, match.NewItem.Modifiers, options);
        }

        protected override string GetDeclaredModifiers(IElementDefinition element)
        {
            return element.GetDeclaredModifiers();
        }

        protected override string ModifierLabel { get; } = "modifier";
    }
}