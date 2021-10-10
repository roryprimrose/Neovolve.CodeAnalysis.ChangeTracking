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

        public const string GetInitProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value { get; init; }
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

        public const string InitOnlyProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value { init; }
    }   
}
";

        public const string InitProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value { init; }
    }   
}
";

        public const string PropertyAccessorWithMultipleAttributesInMultipleLists = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value 
        {
            [First, Second(123)]
            [Third, Fourth(true, named: ""stuff""]
            get; 
            set; 
        }
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

        public const string ReadOnlyProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value { get; }
    }   
}
";

        public const string WriteOnlyProperty = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string Value { set; }
    }   
}
";

        public static string BuildClassPropertyWithModifiers(string modifiers)
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

        public static string BuildInterfacePropertyWithModifiers(string modifiers)
        {
            return @$"
namespace MyNamespace 
{{
    public interface MyClass
    {{
        {modifiers} string Value {{ get; set; }}
    }}  
}}
";
        }

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
    }
}