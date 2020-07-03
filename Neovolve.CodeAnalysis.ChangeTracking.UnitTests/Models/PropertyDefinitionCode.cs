namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    public static class PropertyDefinitionCode
    {
        public const string GenericProperty = @"
namespace MyNamespace 
{
    public class MyClass<T>
    {
        public T Value { get; set; }
    }   
}
";

        public const string GetSetProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value { get; set; }
    }   
}
";

        public const string PropertyWithMultipleAttributesInMultipleLists = @"
namespace MyNamespace 
{
    public class MyClass
    {
        [First, Second(123)]
        [Third, Fourth(true, named: ""stuff""]
        public string Value { get; set; }
    }   
}
";

        public static string BuildPropertyAndGetAccessorWithScope(string propertyScope, string accessorScope)
        {
            return @$"
namespace MyNamespace 
{{
    public class MyClass
    {{
        {propertyScope} string Value {{ {accessorScope} get; set; }}
    }}  
}}
";
        }

        public static string BuildPropertyAndSetAccessorWithScope(string propertyScope, string accessorScope)
        {
            return @$"
namespace MyNamespace 
{{
    public class MyClass
    {{
        {propertyScope} string Value {{ get; {accessorScope} set; }}
    }}  
}}
";
        }

        public static string BuildPropertyWithModifiers(string modifiers)
        {
            return @$"
namespace MyNamespace 
{{
    public class MyClass
    {{
        {modifiers} string Value {{ get; set; }}
    }}  
}}
";
        }
    }
}