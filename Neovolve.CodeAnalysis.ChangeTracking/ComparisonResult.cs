﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class ComparisonResult
    {
        public ComparisonResult(SemVerChangeType changeType, IItemDefinition? oldItem, IItemDefinition? newItem,
            string message)
        {
            ChangeType = changeType;
            OldItem = oldItem;
            NewItem = newItem;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public SemVerChangeType ChangeType { get; }

        public string Message { get; }

        public IItemDefinition? NewItem { get; }

        public IItemDefinition? OldItem { get; }
    }
}