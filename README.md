![Logo](Documentation/Images/Eslon.png)

Eslon (S-LON, "Single Line Object Notation") is a hybrid serialization library for .NET, combining JSON and the Windows-themed Eslon syntax.

- [Syntax](Documentation/Syntax.md)
- [Test cases](Documentation/Output.md)

──────────────────────────────────────────────────

## Examples
Serialize to JSON:
```csharp
JavaAccess.Write(DateTime.Now); // "2017-03-21T17:04:19.818+01:00"
```

Deserialize from JSON:
```csharp
JavaAccess.Read("2017-03-21T17:04:19.818+01:00"); // {3/21/2017 5:04:19 PM}
```

### Architecture
![Logo](Documentation/Images/EslonCycle.png)

### Benchmark
![Benchmark](Documentation/Images/Benchmark.png)
