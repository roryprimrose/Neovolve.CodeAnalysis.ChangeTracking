﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.Models
{
    public static class MethodDefinitionCode
    {
        public const string ClassWithExplicitInterfaceMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass : IDisposable
    {
        public void IDisposable.Dispose()
        {
            return Guid.NewGuid().ToString();
        }
    }
}";

        public const string ClassWithGenericMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public T GetValue<T>() where T : new()
        {
            return new T();
        }
    }
}";

        public const string ClassWithMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public string GetValue()
        {
            return Guid.NewGuid().ToString();
        }
    }
}";

        public const string MethodWithAttributes = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        [Stuff]
        [Here(123, true, ""more"", named: 986]
        public string GetValue()
        {
            return Guid.NewGuid().ToString();
        }
    }
}";

        public const string ClassWithMethodInGenericType = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass<T> where T : new()
    {
        public T GetValue()
        {
            return new T();
        }
    }
}";

        public const string MethodWithMultipleGenericTypes = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public T GetValue<T, V>(V value) where V : class, where T : new()
        {
            return new T();
        }
    }
}";

        public const string MethodWithMultipleParameters = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public T GetValue<T, V>(V value, params object[] otherValues) where V : class, where T : new()
        {
            return new T();
        }
    }
}";

        public const string MethodWithMultipleGenericConstraints = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public T GetValue<T, V>(V value) where V : class, IDisposable where T : new()
        {
            return new T();
        }
    }
}";

        public const string ClassWithTaskMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public async Task<string> GetValue()
        {
            await Task.Delay(1000).ConfigureAwait(false);

            return Guid.NewGuid().ToString();
        }
    }
}";

        public const string ClassWithVoidMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public class MyClass
    {
        public void DoSomething()
        {
            // Something done
        }
    }
}";

        public const string InterfaceWithMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public interface MyClass
    {
        string GetValue();
    }
}";

        public const string InterfaceWithDefaultMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public interface MyClass
    {
        void DoSomething() { System.Diagnostics.Debug.WriteLine(""IA.M""); }
    }
}";

        public const string StructWithMethod = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.IO;

    public struct MyClass
    {
        public string GetValue()
        {
            return Guid.NewGuid().ToString();
        }
    }
}";
    }
}