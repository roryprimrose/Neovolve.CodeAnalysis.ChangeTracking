namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    public static class ArgumentDefinitionCode
    {
        public const string NamedArgument = @"
namespace MyNamespace 
{
    [SimpleAttribute(first: 123)]
    public class MyClass
    {
    }   
}
";

        public const string OrdinalArgument = @"
namespace MyNamespace 
{
    [SimpleAttribute(123)]
    public class MyClass
    {
    }   
}
";
    }
}