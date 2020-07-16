namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class ComparerOptions
    {
        public static readonly ComparerOptions Default = BuildDefaultOptions();

        private static ComparerOptions BuildDefaultOptions()
        {
            return new ComparerOptions();
        }

        /// <summary>
        ///     Gets or sets the message formatter creates the type change messages.
        /// </summary>
        public IMessageFormatter MessageFormatter { get; set; } = new DefaultMessageFormatter();

        /// <summary>
        ///     Determines whether attribute changes should be evaluated.
        /// </summary>
        public bool SkipAttributes { get; set; } = false;
    }
}