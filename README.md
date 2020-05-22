# Neovolve.CodeAnalysis.ChangeTracking
C# code analysis tool for evaluating changes to contracts

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/blob/master/LICENSE)&nbsp;[![Nuget](https://img.shields.io/nuget/v/Neovolve.CodeAnalysis.ChangeTracking.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.CodeAnalysis.ChangeTracking.svg)](https://www.nuget.org/packages/Neovolve.CodeAnalysis.ChangeTracking)

[![Coverage Status](https://coveralls.io/repos/github/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/badge.svg?branch=master)](https://coveralls.io/github/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking?branch=master)&nbsp;
[![Actions Status](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/workflows/CI/badge.svg)](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/actions)

- [Installation](#installation)
- [Features](#features)
- [Usage](#usage)
- [Limitations](#limitations)

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
- Changes to class or interface property visibility
- Changes to field visibility

Changes not currently evaluated are:
- Changes to attribute usage
- Changes in class methods
- Changes in interface methods

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