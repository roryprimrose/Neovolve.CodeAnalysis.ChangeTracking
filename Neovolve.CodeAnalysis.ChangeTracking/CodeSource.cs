namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class CodeSource
    {
        public CodeSource(string contents)
        {
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
            FilePath = string.Empty;
        }

        public CodeSource(string filePath, string contents)
        {
            FilePath = string.IsNullOrWhiteSpace(filePath)
                ? throw new ArgumentException("FilePath is null, empty or only contains whitespace", nameof(filePath))
                : filePath;
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
        }

        public string Contents { get; }

        public string? FilePath { get; }
    }
}