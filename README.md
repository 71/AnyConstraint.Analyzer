AnyConstraint.Analyzer
======================

A simple C# analyzer that suppresses the **CS0702**: "Constraint cannot be special class 'Delegate' / 'Enum' / '...' " error.

## Installation
Simply install the NuGet package, and the analyzer will be automatically added to your project.
```powershell
Install-Package AnyConstraint.Analyzer
```

## Usage
There is no usage. Having the analyzer referenced is enough. The following code should compile just fine:
```csharp
public static void Invoke<TDelegate>(TDelegate del) where TDelegate : Delegate { ... }
public static void GetValues<T>(T @enum) where T : Enum { ... }
```
Nonetheless, an error will be shown in Visual Studio. To hide it from the 'Error List' panel, you can add the following code above your method:
```csharp
[SuppressMessage("Compiler", "CS0702")]
```
Or, at the global level:
```csharp
[assembly: SuppressMessage("Compiler", "CS0702")]
```

## How does it work?
Using [Ryder](https://github.com/6A/Ryder), the internal call to [`Binder.IsValidConstraintType`](http://source.roslyn.io/#Microsoft.CodeAnalysis.CSharp/Binder/Binder_Constraints.cs,14be8263fd49892c) is replaced by a simple `return true` statement, thus allowing any type to be used as a constraint.