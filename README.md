# Neovolve.CodeAnalysis.ChangeTracking
C# code analysis tool for evaluating changes to contracts

[![Coverage Status](https://coveralls.io/repos/github/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/badge.svg?branch=master)](https://coveralls.io/github/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking?branch=master)

[![Actions Status](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/workflows/CI/badge.svg)](https://github.com/roryprimrose/Neovolve.CodeAnalysis.ChangeTracking/actions)

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
var oldCode = new List<string>
{
    @"
    public class Test
    {
        public string Value;
    }"
};
var newCode = new List<string>
{
    @"
    public class Test
    {
        public bool Value;
    }"
};

var calculator = ChangeCalculatorFactory.BuildCalculator();

var result = await calculator.CalculateChange(oldCode, newCode).ConfigureAwait(false);

if (result == ChangeType.None) 
{
    // Looks like there is no change in the members
}
else if (result == ChangeType.Feature)
{
    // Looks like members have been added to the code
}
else if (result == ChangeType.Breaking)
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