namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public enum ParameterModifiers
    {
        None = 0,
        Ref = 1,
        Out = 2,
        This = 4,
        Params = 8
    }
}