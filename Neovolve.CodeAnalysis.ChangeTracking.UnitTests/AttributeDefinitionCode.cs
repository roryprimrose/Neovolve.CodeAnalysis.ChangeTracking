namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    internal static class AttributeDefinitionCode
    {
        public const string AttributeWithMixedOrdinalAndNamedArguments = @"
namespace MyNamespace 
{
    [SimpleAttribute(""stringValue"", 123, first: true, second: SomeConstant]
    public class MyClass
    {
    }   
}
";

        public const string AttributeWithNamedArguments = @"
namespace MyNamespace 
{
    [SimpleAttribute(first: ""stringValue"", second: 123, third: true]
    public class MyClass
    {
    }   
}
";

        public const string AttributeWithOrdinalArguments = @"
namespace MyNamespace 
{
    [SimpleAttribute(""stringValue"", 123, true]
    public class MyClass
    {
    }   
}
";

        public const string SimpleAttribute = @"
namespace MyNamespace 
{
    [SimpleAttribute]
    public class MyClass
    {
    }   
}
";

        public const string SimpleAttributeWithBrackets = @"
namespace MyNamespace 
{
    [SimpleAttribute()]
    public class MyClass
    {
    }   
}
";
    }
}