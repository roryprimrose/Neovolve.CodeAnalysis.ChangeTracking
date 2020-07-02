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
        ///     Determines whether attribute changes should be evaluated.
        /// </summary>
        public bool SkipAttributes { get; set; } = false;
    }
}