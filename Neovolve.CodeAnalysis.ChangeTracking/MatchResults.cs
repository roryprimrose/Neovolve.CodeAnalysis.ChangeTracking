﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    /// <summary>
    ///     The <see cref="MatchResults{T}" />
    ///     class defines items that have been added or removed as well as items that can be matched.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MatchResults<T> : IMatchResults<T> where T : IItemDefinition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MatchResults{T}" /> class.
        /// </summary>
        /// <param name="itemsRemoved">The items that have been removed.</param>
        /// <param name="itemsAdded">The items that have been added.</param>
        public MatchResults(IEnumerable<T> itemsRemoved, IEnumerable<T> itemsAdded)
        {
            itemsRemoved = itemsRemoved ?? throw new ArgumentNullException(nameof(itemsRemoved));
            itemsAdded = itemsAdded ?? throw new ArgumentNullException(nameof(itemsAdded));

            MatchingItems = Array.Empty<ItemMatch<T>>();
            ItemsRemoved = new ReadOnlyCollection<T>(itemsRemoved.FastToList());
            ItemsAdded = new ReadOnlyCollection<T>(itemsAdded.FastToList());
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatchResults{T}" /> class.
        /// </summary>
        /// <param name="matchingItems">The set of old items that match to new items.</param>
        /// <param name="itemsRemoved">The items that have been removed.</param>
        /// <param name="itemsAdded">The items that have been added.</param>
        public MatchResults(
            IEnumerable<ItemMatch<T>> matchingItems,
            IEnumerable<T> itemsRemoved,
            IEnumerable<T> itemsAdded)
        {
            matchingItems = matchingItems ?? throw new ArgumentNullException(nameof(matchingItems));
            itemsRemoved = itemsRemoved ?? throw new ArgumentNullException(nameof(itemsRemoved));
            itemsAdded = itemsAdded ?? throw new ArgumentNullException(nameof(itemsAdded));

            MatchingItems = new ReadOnlyCollection<ItemMatch<T>>(matchingItems.FastToList());
            ItemsRemoved = new ReadOnlyCollection<T>(itemsRemoved.FastToList());
            ItemsAdded = new ReadOnlyCollection<T>(itemsAdded.FastToList());
        }

        /// <inheritdoc />
        public IReadOnlyCollection<T> ItemsAdded { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<T> ItemsRemoved { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ItemMatch<T>> MatchingItems { get; }
    }
}