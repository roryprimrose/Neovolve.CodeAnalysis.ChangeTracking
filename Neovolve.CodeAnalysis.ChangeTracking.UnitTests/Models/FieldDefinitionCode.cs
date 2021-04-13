namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    public static class FieldDefinitionCode
    {
        public const string GenericField = @"
namespace MyNamespace 
{
    public class MyClass<T>
    {
        public T Value;
    }   
}
";

        public const string SingleField = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value;
    }   
}
";

        public const string FieldWithMultipleAttributesInMultipleLists = @"
namespace MyNamespace 
{
    public class MyClass
    {
        [First, Second(123)]
        [Third, Fourth(true, named: ""stuff""]
        public string Value;
    }   
}
";

        public static string BuildClassFieldWithModifiers(string scope)
        {
            return @$"
namespace MyNamespace 
{{
    public class MyClass
    {{
        {scope} string Value;
    }}  
}}
";
        }

        public static string BuildStructFieldWithModifiers(string scope)
        {
            return @$"
namespace MyNamespace 
{{
    public struct MyClass
    {{
        {scope} string Value;
    }}  
}}
";
        }
    }
}