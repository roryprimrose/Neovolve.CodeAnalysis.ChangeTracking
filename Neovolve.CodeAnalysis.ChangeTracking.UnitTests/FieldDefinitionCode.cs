namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
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

        public const string GetSetField = @"
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

        public static string BuildFieldWithScope(string scope)
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
    }
}