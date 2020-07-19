namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class FormatArguments
    {
        public FormatArguments(string messageFormat, string identifier, string? oldValue, string? newValue)
        {
            MessageFormat = messageFormat ?? throw new ArgumentNullException(nameof(messageFormat));
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string Identifier { get; }
        public string MessageFormat { get; }
        public string? OldValue { get; }
        public string? NewValue { get; }
    }
}