# Using CLR Types as Plugins

The library allows you to use any CLR type or object as a plugin without requiring `KernelFunction` attributes. This enables you to create plugins from existing objects or types, making it easier to integrate with existing codebases.

## Why Use CLR Types as Plugins?

### Integration with Existing Code

- **Legacy Systems**: Convert existing business logic into AI-callable functions
- **Third-Party Libraries**: Expose library functionality to AI models
- **Rapid Prototyping**: Quickly turn any class into a plugin without modification
- **Clean Architecture**: Keep AI concerns separate from business logic

### No Attribute Requirements

Unlike traditional Semantic Kernel plugins, CLR types don't need:

- `[KernelFunction]` attributes on methods
- `[Description]` attributes for documentation
- Specific method signatures

## Basic Usage

### Example CLR Type

```csharp
public class ShortDate
{
    public string ToShortDateString()
    {
        return DateTime.Now.ToShortDateString();
    }
    
    public string ToLongDateString()
    {
        return DateTime.Now.ToLongDateString();
    }
    
    public string AddDays(int days)
    {
        return DateTime.Now.AddDays(days).ToShortDateString();
    }
}
```

### Metadata Provider for CLR Type

```csharp
public class DateMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name == "ShortDatePlugin" ? new PluginMetadata
        {
            Description = "This plugin returns date and time information."
        } : null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name != "ShortDatePlugin") return null;
        
        return metadata.Name switch
        {
            "ToShortDateString" => new FunctionMetadata(metadata.Name)
            {
                Description = "Returns the current date in short format (MM/dd/yyyy)."
            },
            "ToLongDateString" => new FunctionMetadata(metadata.Name)
            {
                Description = "Returns the current date in long format."
            },
            "AddDays" => new FunctionMetadata(metadata.Name)
            {
                Description = "Adds the specified number of days to the current date.",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("days")
                    {
                        Description = "Number of days to add (can be negative)"
                    }
                }
            },
            _ => null
        };
    }
}
```

## Registration Methods

### From CLR Type

```csharp
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<ShortDate>("ShortDatePlugin");
```

### From CLR Object Instance

```csharp
var dateInstance = new ShortDate();
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(dateInstance, "ShortDatePlugin");
```

## Advanced Examples

### Example 1: Math Utilities

```csharp
public class MathUtilities
{
    public double Add(double a, double b) => a + b;
    public double Subtract(double a, double b) => a - b;
    public double Multiply(double a, double b) => a * b;
    public double Divide(double a, double b) => b != 0 ? a / b : throw new DivideByZeroException();
    public double Power(double baseNum, double exponent) => Math.Pow(baseNum, exponent);
    public double SquareRoot(double number) => Math.Sqrt(number);
}

public class MathMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name == "MathPlugin" ? new PluginMetadata
        {
            Description = "Provides basic mathematical operations"
        } : null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name != "MathPlugin") return null;
        
        return metadata.Name switch
        {
            "Add" => new FunctionMetadata(metadata.Name)
            {
                Description = "Adds two numbers together",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("a") { Description = "First number" },
                    new ParameterMetadata("b") { Description = "Second number" }
                }
            },
            "Divide" => new FunctionMetadata(metadata.Name)
            {
                Description = "Divides the first number by the second number",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("a") { Description = "Dividend" },
                    new ParameterMetadata("b") { Description = "Divisor (cannot be zero)" }
                }
            },
            // ... other operations
            _ => null
        };
    }
}

// Registration
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<MathUtilities>("MathPlugin");
```

### Example 2: File System Operations

```csharp
public class FileSystemHelper
{
    public bool FileExists(string path) => File.Exists(path);
    public string ReadTextFile(string path) => File.ReadAllText(path);
    public void WriteTextFile(string path, string content) => File.WriteAllText(path, content);
    public string[] ListFiles(string directory) => Directory.GetFiles(directory);
    public long GetFileSize(string path) => new FileInfo(path).Length;
}

public class FileSystemMetadataProvider : IPluginMetadataProvider
{
    private readonly IConfiguration _configuration;
    
    public FileSystemMetadataProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name == "FileSystemPlugin" ? new PluginMetadata
        {
            Description = "Provides safe file system operations"
        } : null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name != "FileSystemPlugin") return null;
        
        var allowedPath = _configuration.GetValue<string>("AllowedBasePath", @"C:\SafeDirectory");
        
        return metadata.Name switch
        {
            "ReadTextFile" => new FunctionMetadata(metadata.Name)
            {
                Description = "Reads the contents of a text file",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("path") 
                    { 
                        Description = $"File path (must be within {allowedPath})" 
                    }
                }
            },
            "WriteTextFile" => _configuration.GetValue<bool>("AllowWrite", false) 
                ? new FunctionMetadata(metadata.Name)
                {
                    Description = "Writes content to a text file",
                    Parameters = new List<ParameterMetadata>
                    {
                        new ParameterMetadata("path") { Description = "File path" },
                        new ParameterMetadata("content") { Description = "Content to write" }
                    }
                }
                : new FunctionMetadata(metadata.Name) { Suppress = true }, // Hide in read-only mode
            _ => null
        };
    }
}
```

### Example 3: Database Operations

```csharp
public class DatabaseHelper
{
    private readonly string _connectionString;
    
    public DatabaseHelper(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<List<Dictionary<string, object>>> ExecuteQuery(string sql)
    {
        // Implementation for safe query execution
        // Note: In real scenarios, use parameterized queries and validation
        throw new NotImplementedException("Implement safe query execution");
    }
    
    public async Task<int> GetRecordCount(string tableName)
    {
        // Implementation for getting record counts
        throw new NotImplementedException("Implement record count logic");
    }
}

public class DatabaseMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name == "DatabasePlugin" ? new PluginMetadata
        {
            Description = "Provides safe database query operations"
        } : null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name != "DatabasePlugin") return null;
        
        return metadata.Name switch
        {
            "ExecuteQuery" => new FunctionMetadata(metadata.Name)
            {
                Description = "Executes a SELECT query against the database",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("sql") 
                    { 
                        Description = "SQL SELECT statement (READ-ONLY queries only)" 
                    }
                }
            },
            "GetRecordCount" => new FunctionMetadata(metadata.Name)
            {
                Description = "Gets the number of records in a table",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("tableName") 
                    { 
                        Description = "Name of the table to count records" 
                    }
                }
            },
            _ => null
        };
    }
}

// Registration with dependency injection
services.AddSingleton(provider => 
    new DatabaseHelper(provider.GetService<IConfiguration>().GetConnectionString("Default")));

kernelBuilder.Plugins.AddFromClrObjectWithMetadata(
    serviceProvider.GetService<DatabaseHelper>(), 
    "DatabasePlugin"
);
```

## Method Discovery Rules

### Public Methods Only

Only public methods are automatically discovered and converted to kernel functions:

```csharp
public class ExampleClass
{
    public string PublicMethod() => "Available"; // ✅ Will be available
    private string PrivateMethod() => "Hidden"; // ❌ Will not be available
    internal string InternalMethod() => "Hidden"; // ❌ Will not be available
    protected string ProtectedMethod() => "Hidden"; // ❌ Will not be available
}
```

### Supported Parameter Types

The following parameter types are automatically supported:

- **Primitive types**: `string`, `int`, `double`, `bool`, `DateTime`, etc.
- **Nullable types**: `int?`, `DateTime?`, etc.
- **Collections**: `List<T>`, `T[]`, `IEnumerable<T>`
- **Complex types**: Custom classes (serialized as JSON)

### Return Types

Supported return types include:

- **Synchronous**: Any serializable type
- **Asynchronous**: `Task<T>`, `ValueTask<T>`
- **Void**: `void`, `Task` (for side-effect operations)

## Best Practices

### 1. Keep Methods Simple

Design methods with clear, single responsibilities:

```csharp
// Good: Simple, focused methods
public class WeatherHelper
{
    public double CelsiusToFahrenheit(double celsius) => (celsius * 9/5) + 32;
    public double FahrenheitToCelsius(double fahrenheit) => (fahrenheit - 32) * 5/9;
}

// Avoid: Complex methods with multiple responsibilities
public class ComplicatedHelper
{
    public string DoEverything(string input, bool flag, int mode, double[] data)
    {
        // Too complex for AI to understand easily
    }
}
```

### 2. Use Descriptive Names

Choose method and parameter names that clearly indicate their purpose:

```csharp
public class EmailHelper
{
    // Good: Clear method and parameter names
    public bool SendEmail(string recipientEmail, string subject, string body) { /* ... */ }
    
    // Avoid: Unclear names
    public bool Process(string data1, string data2, string data3) { /* ... */ }
}
```

### 3. Provide Comprehensive Metadata

Always provide detailed descriptions through metadata providers:

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    return metadata.Name switch
    {
        "SendEmail" => new FunctionMetadata(metadata.Name)
        {
            Description = "Sends an email to the specified recipient with the given subject and body",
            Parameters = new List<ParameterMetadata>
            {
                new ParameterMetadata("recipientEmail") 
                { 
                    Description = "Valid email address of the recipient" 
                },
                new ParameterMetadata("subject") 
                { 
                    Description = "Email subject line (max 200 characters)" 
                },
                new ParameterMetadata("body") 
                { 
                    Description = "Email content (supports plain text and HTML)" 
                }
            }
        },
        _ => null
    };
}
```

### 4. Handle Errors Gracefully

Design methods to handle errors appropriately:

```csharp
public class SafeFileHelper
{
    public string ReadFileContent(string path)
    {
        try
        {
            if (!File.Exists(path))
                return "File not found";
                
            return File.ReadAllText(path);
        }
        catch (UnauthorizedAccessException)
        {
            return "Access denied";
        }
        catch (Exception ex)
        {
            return $"Error reading file: {ex.Message}";
        }
    }
}
```

### 5. Consider Security

Always validate inputs and limit capabilities:

```csharp
public class SecureFileHelper
{
    private readonly string _allowedBasePath;
    
    public SecureFileHelper(string allowedBasePath)
    {
        _allowedBasePath = Path.GetFullPath(allowedBasePath);
    }
    
    public string ReadFile(string relativePath)
    {
        var fullPath = Path.Combine(_allowedBasePath, relativePath);
        var normalizedPath = Path.GetFullPath(fullPath);
        
        // Ensure the path is within the allowed directory
        if (!normalizedPath.StartsWith(_allowedBasePath))
            throw new UnauthorizedAccessException("Path not allowed");
            
        return File.ReadAllText(normalizedPath);
    }
}
```
