namespace Neovolve.CodeAnalysis.ChangeTracking.Models
{
    public enum ParameterModifier
    {
        None = 0,
        Ref = 1,
        Out = 2,
        This = 4,
        Params = 8
    }
}