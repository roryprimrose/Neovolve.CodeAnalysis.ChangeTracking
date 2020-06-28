namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    internal static class TypeDefinitionCode
    {
        public const string ClassImplementsMultipleTypes = @"
namespace MyNamespace 
{
    public class MyClass : MyBase, IEnumerable<string>
    {
    }  
}
";

        public const string ClassImplementsSingleType = @"
namespace MyNamespace 
{
    public class MyClass : MyBase
    {
    }  
}
";

        public const string ClassInGrandparentClass = @"
namespace MyNamespace 
{
    public class MyGrandparentClass
    {
        public class MyParentClass
        {
            public class MyClass
            {
            }  
        }  
    }   
}
";

        public const string ClassInParentClass = @"
namespace MyNamespace 
{
    public class MyParentClass
    {
        public class MyClass
        {
        }  
    }   
}
";

        public const string ClassInParentClassAndInterface = @"
namespace MyNamespace 
{
    public class MyGrandparentClass
    {
        public interface IMyInterface
        {
            public class MyParentClass
            {
                public class MyClass
                {
                    public interface IChildInterface
                    {
                    }
                }  
            }
       }  
    }   
}
";

        public const string ClassWithGenericType = @"
namespace MyNamespace 
{
    public class MyClass<T>
    {
    }  
}
";

        public const string ClassWithoutNamespace = @"
public class MyClass
{
}
";

        public const string ClassWithoutParent = @"
namespace MyNamespace 
{
    public class MyClass
    {
    }   
}
";

        public const string InterfaceImplementsMultipleTypes = @"
namespace MyNamespace 
{
    public interface IMyInterface<T> : IDisposable, IEnumerable<T>
    {
    }  
}
";

        public const string InterfaceWithGenericType = @"
namespace MyNamespace 
{
    public interface IMyInterface<T>
    {
    }  
}
";

        public const string InterfaceWithMultipleGenericTypes = @"
namespace MyNamespace 
{
    public interface IMyInterface<T, V>
    {
    }  
}
";

        public const string InterfaceWithoutParent = @"
namespace MyNamespace 
{
    public interface MyInterface
    {
    }   
}
";

        public const string MultipleChildClasses = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public class FirstChild
        {
        }

        public class SecondChild
        {
        }
    }    
}
";

        public const string MultipleChildInterfaces = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public interface FirstChild
        {
        }

        public interface SecondChild
        {
        }
    }    
}
";

        public static string BuildClassWithScope(string scope)
        {
            return @$"
namespace MyNamespace 
{{
    {scope} class MyClass
    {{
    }}  
}}  
";
        }

        public static string BuildHierarchyWithScope(string grandparentScope, string parentScope, string scope)
        {
            return @$"
namespace MyNamespace 
{{
    {grandparentScope} class MyGrandparentClass
    {{
        {parentScope} class MyParentClass
        {{
            {scope} class MyClass
            {{
            }}  
        }}  
    }}   
}}
";
        }
    }
}