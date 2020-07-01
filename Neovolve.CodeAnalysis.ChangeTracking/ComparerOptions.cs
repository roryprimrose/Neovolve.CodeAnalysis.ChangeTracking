namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public class ComparerOptions
    {
        public static readonly ComparerOptions Default = BuildDefaultOptions();

        private static ComparerOptions BuildDefaultOptions()
        {
            return new ComparerOptions();
        }
    }
}