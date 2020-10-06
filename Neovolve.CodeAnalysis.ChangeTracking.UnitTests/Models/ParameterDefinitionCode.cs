namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    public static class ParameterDefinitionCode
    {
        public const string SingleParameter = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public Guid DoSomething(string value)
        {
            return Guid.NewGuid();
        }
    }   
}
";
    }
}