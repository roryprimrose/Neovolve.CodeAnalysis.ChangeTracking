namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    internal static class AttributeDefinitionCode
    {
        public const string AttributeWithMixedArgumentsWhereNamedParameterNameChanged = @"
namespace MyNamespace 
{
    [SimpleAttribute(""stringValue"", 123, first: true, third: SomeConstant]
    public class MyClass
    {
    }   
}
";

        public const string AttributeWithMixedArgumentsWhereNamedValueChanged = @"
namespace MyNamespace 
{
    [SimpleAttribute(""stringValue"", 123, first: true, second: ""changed""]
    public class MyClass
    {
    }   
}
";

        public const string AttributeWithMixedArgumentsWhereOrdinalValueChanged = @"
namespace MyNamespace 
{
    [SimpleAttribute(""otherValue"", 123, first: true, second: SomeConstant]
    public class MyClass
    {
    }   
}
";

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

        public const string AttributeWithOneOrdinalAndTwoNamedArguments = @"
namespace MyNamespace 
{
    [SimpleAttribute(""stringValue"", first: true, second: SomeConstant]
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

        public const string AttributeWithTwoOrdinalAndOneNamedArguments = @"
namespace MyNamespace 
{
    [SimpleAttribute(""stringValue"", true, second: SomeConstant]
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