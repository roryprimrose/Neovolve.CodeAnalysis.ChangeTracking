namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
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

        public const string ClassWithComplexNamespace = @"
namespace MyNamespace.OtherNamespace.FinalNamespace
{
    public class MyClass
    {
    }   
}
";

        public const string ClassWithGenericConstraints = @"
namespace MyNamespace 
{
    public class MyClass<T> : IEnumerable<T> where T : Stream, new()
    {
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

        public const string ClassWithMultipleAttributes = @"
namespace MyNamespace 
{
    [First]
    [Second]
    public class MyClass
    {
    }   
}
";

        public const string ClassWithMultipleAttributesInMultipleLists = @"
namespace MyNamespace 
{
    [First, Second(123)]
    [Third, Fourth(true, named: ""stuff""]
    public class MyClass
    {
    }   
}
";

        public const string ClassWithMultipleAttributesInSingleList = @"
namespace MyNamespace 
{
    [First, Second]
    public class MyClass
    {
    }   
}
";

        public const string ClassWithMultipleGenericConstraints = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass<TKey, TValue> : IEnumerable<TKey> where TKey : Stream, new() where TValue : struct
    {
        public TKey DefaultKey;
        public TValue GetValue(TKey key);
        public TValue RandomValue { get; set; }

        public class MyChildClass
        {
            public TValue Value;
        }
    }
}";

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

        public const string ClassWithProperties = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public string First { get; set; }
        public DateTimeOffset Second { get; set; }
    }   
}
";

        public const string ClassWithSingleAttribute = @"
namespace MyNamespace 
{
    [MyAttribute]
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

        public const string InterfaceWithGenericConstraints = @"
namespace MyNamespace 
{
    public interface MyInterface<T> : IEnumerable<T> where T : Stream, new()
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

        public const string InterfaceWithMultipleGenericConstraints = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public interface MyInterface<TKey, TValue> : IEnumerable<TKey> where TKey : Stream, new() where TValue : struct
    {
    }
}";

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

        public const string InterfaceWithProperties = @"
namespace MyNamespace 
{
    public interface IMyInterface
    {
        string First { get; set; }
        DateTimeOffset Second { get; set; }
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

        public const string MultipleChildTypes = @"
namespace MyNamespace 
{
    public class MyClass
    {
        public class FirstClass
        {
        }

        public class SecondClass
        {
        }

        public interface FirstInterface
        {
        }

        public interface SecondInterface
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
    }
}