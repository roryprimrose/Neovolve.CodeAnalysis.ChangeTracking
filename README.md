# Neovolve.CodeAnalysis.ChangeTracking
C# code analysis tool for evaluating changes to contracts

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/blob/master/LICENSE)&nbsp;[![Nuget](https://img.shields.io/nuget/v/Neovolve.CodeAnalysis.ChangeTracking.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.CodeAnalysis.ChangeTracking.svg)](https://www.nuget.org/packages/Neovolve.CodeAnalysis.ChangeTracking)&nbsp;[![Actions Status](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/workflows/CI/badge.svg)](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/actions)

- [Introduction](#introduction)
- [Installation](#installation)
- [Features](#features)
- [Usage](#usage)
- [Limitations](#limitations)
- [Frequently asked questions](#frequently-asked-questions)

## Introduction

Neovolve.CodeAnalysis.ChangeTracking is a package that calculates changes between two versions of C# code to identify what has changed such that there could be a change in Semantic Versioning. The outcome will be an overall Semantic Version change result (None, Feature, Breaking) as well as a collection of changes. Each change in the collection identifies the source and target change along with a message indicating the change and the SemVer impact of that change.

## Installation

Production and beta versions are available on [NuGet](https://www.nuget.org/packages/Neovolve.CodeAnalysis.ChangeTracking/).

```
Install-Package Neovolve.CodeAnalysis.ChangeTracking
```

Continous Integration builds are available on [MyGet](https://www.myget.org/feed/neovolve/package/nuget/Neovolve.CodeAnalysis.ChangeTracking).

```
Install-Package Neovolve.CodeAnalysis.ChangeTracking -Source https://www.myget.org/F/neovolve/api/v3/index.json
```

## Features

This library will determine whether C# code has changed in a way that introduces a new feature or a breaking change. 

Changes currently evaluated are:
- Adding or removing class or interface properties
- Adding or removing fields
- Adding, removing or changing attributes
- Changes to class or interface property visibility
- Changes to field visibility

Changes not currently evaluated are:
- Changes in class methods
- Changes in interface methods
- net5.0 record types
- net5.0 init properties

## Usage
```csharp
var oldCode = new List<CodeSource>
{
    new CodeSource(@"
        public class Test
        {
            public string Value;
        }", "Test.cs")
};
var newCode = new List<CodeSource>
{
    new CodeSource(@"
        public class Test
        {
            public bool Value;
        }", "Test.cs")
};

var calculator = ChangeCalculatorFactory.BuildCalculator();

var result = await calculator.CalculateChange(oldCode, newCode).ConfigureAwait(false);

if (result.ChangeType == ChangeType.None) 
{
    // Looks like there is no change in the members
}
else if (result.ChangeType == ChangeType.Feature)
{
    // Looks like members have been added to the code
}
else if (result.ChangeType == ChangeType.Breaking)
{
    // Looks like members have been removed or changed
}
```

## Limitations
The library evaluates code changes by parsing the code text. The advantage of this method is that the code does not need to be compiled, dependencies do not need to be resolved and any C# project usage is supported. In fact the csproj itself is not even supported as the library only parses cs files. The disadvantage is that evaluating code changes can produce false positivies because all differences are evaluated as string values. 

For example, the following two code blocks when evaluated will indicate a breaking change. In reality, it is likely that the code will compile to the same outcome.

```csharp
public MyNamespace
{
    public class MyClass
    {
        public System.DateTime MyValue { get; set; }
    }
}
```

```csharp
using System;

public MyNamespace
{
    public class MyClass
    {
        public DateTime MyValue { get; set; }
    }
}
```

## Frequently asked questions

**Why does SemVerChangeType define None instead of Patch?**

This package attempts to calculate the Semantic Version impact on a C# binary based on changes to the public API surface (see attribute comparison below). Adding new signatures would be calculated as a feature and removing or modifying signatures would result in a breaking change. 

There are only two other types of changes that could occur between two sets of C# code. There is either no change or there is a change to internal logic where there is no impact on the public API surface. No change really should not even be identified as a Patch change while changes to internal logic should be a patch change. As the library does not look at internal code, it can't actually determine the difference between these two scenarios so it does not attempt to identify a patch change.

**Why do attribute changes identify a feature or breaking change when the public API signature hasn't changed?**

There is an edge case to the idea that the library only compares the public API surface between two versions of C# code. The library also evaluates changes to attributes based on the `ComparerOptions` class in order to determine a Semantic Version impact of attributes. 

What this means is that the C# signatures could be the same, but attribute changes regarding XML and JSON serialization could cause a potential breaking change to consumers of the library. So signatures compiled into the binary are the same, but the attributes are telling the runtime to handle serialization differently.

For example, consider this change. Here is the old code.

```csharp
using System;

public MyNamespace
{
    public class MyClass
    {
        public DateTime MyValue { get; set; }
    }
}
```

Here is the new code.

```csharp
using System;
using System.Text.Json.Serialization;

public MyNamespace
{
    [JsonPropertyName("otherValue")]
    public class MyClass
    {
        public DateTime MyValue { get; set; }
    }
}
```

This is a breaking change to consumers because the json serialization of this class has changed the name of the property from `MyValue` to `otherValue`.