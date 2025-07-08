# Advanced Features

SemanticPluginForge provides several advanced features that enable powerful customization scenarios.

## Overview

This section covers the advanced capabilities of SemanticPluginForge:

- [Suppressing Functions and Parameters](suppressing-functions-parameters.md)
- [Using CLR Types as Plugins](clr-types-as-plugins.md)
- [Function Name Override](function-name-override.md)

## Feature Summary

| Feature | Description | Use Cases |
|---------|-------------|-----------|
| Function Suppression | Hide entire functions from plugin consumers | Deprecating functions, conditional availability |
| Parameter Suppression | Hide specific parameters while providing defaults | Simplifying interfaces, context injection |
| CLR Type Support | Use any .NET class as a plugin | Legacy code integration, external libraries |
| Function Name Override | Change function names without code modification | Better AI naming conventions, method overload disambiguation, API versioning |
| Default Value Override | Set parameter defaults through metadata | Configuration-driven defaults, environment-specific values |

## Quick Examples

### Suppressing a Function

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    if (plugin.Name == "MyPlugin" && metadata.Name == "DeprecatedFunction")
    {
        return new FunctionMetadata(metadata.Name) { Suppress = true };
    }
    return null;
}
```

### Suppressing a Parameter with Default

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    if (plugin.Name == "MyPlugin" && metadata.Name == "MyFunction")
    {
        return new FunctionMetadata(metadata.Name)
        {
            Parameters = new List<ParameterMetadata>
            {
                new ParameterMetadata("hiddenParam") 
                { 
                    Suppress = true, 
                    DefaultValue = "automatic" 
                }
            }
        };
    }
    return null;
}
```

### Using CLR Type as Plugin

```csharp
public class DateUtility
{
    public string GetCurrentDate() => DateTime.Now.ToShortDateString();
    public string FormatDate(DateTime date) => date.ToString("yyyy-MM-dd");
}

// Register as plugin
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<DateUtility>("DateUtils");
```

### Overriding Function Name

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    if (plugin.Name == "RandomPlugin")
    {
        // Handle function overloads by examining parameter signature
        if (metadata.Name == "Next")
        {
            // Different overrides based on parameter count
            var paramCount = metadata.Parameters.Count;
            
            if (paramCount == 0)
            {
                return new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "GetRandomNumber",
                    Description = "Generates a random non-negative integer"
                };
            }
            else if (paramCount == 1 && metadata.Parameters[0].Name == "maxValue")
            {
                return new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "GetRandomNumberWithMaxValue",
                    Description = "Generates a random integer between 0 and maxValue (exclusive)"
                };
            }
            else if (paramCount == 2)
            {
                return new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "GetRandomNumberInRange",
                    Description = "Generates a random integer between minValue and maxValue"
                };
            }
        }
    }
    return null;
}
```

## Advanced Scenarios

### Conditional Metadata

Provide different metadata based on runtime conditions:

```csharp
public class ConditionalMetadataProvider : IPluginMetadataProvider
{
    private readonly IConfiguration _configuration;
    
    public ConditionalMetadataProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        var isDebugMode = _configuration.GetValue<bool>("DebugMode");
        
        if (plugin.Name == "AdminPlugin" && !isDebugMode)
        {
            // Suppress admin functions in production
            return new FunctionMetadata(metadata.Name) { Suppress = true };
        }
        
        return null;
    }
}
```

### Multi-Source Metadata

Combine metadata from multiple sources:

```csharp
public class CompositeMetadataProvider : IPluginMetadataProvider
{
    private readonly IEnumerable<IPluginMetadataProvider> _providers;
    
    public CompositeMetadataProvider(IEnumerable<IPluginMetadataProvider> providers)
    {
        _providers = providers;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        foreach (var provider in _providers)
        {
            var result = provider.GetFunctionMetadata(plugin, metadata);
            if (result != null)
                return result;
        }
        return null;
    }
}
```

### Database-Driven Metadata

Load metadata from a database:

```csharp
public class DatabaseMetadataProvider : IPluginMetadataProvider
{
    private readonly IMetadataRepository _repository;
    
    public DatabaseMetadataProvider(IMetadataRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<FunctionMetadata?> GetFunctionMetadataAsync(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        var dbMetadata = await _repository.GetFunctionMetadataAsync(plugin.Name, metadata.Name);
        
        if (dbMetadata == null) return null;
        
        return new FunctionMetadata(metadata.Name)
        {
            Description = dbMetadata.Description,
            Suppress = dbMetadata.IsHidden,
            Parameters = dbMetadata.Parameters?.Select(p => new ParameterMetadata(p.Name)
            {
                Description = p.Description,
                DefaultValue = p.DefaultValue,
                Suppress = p.IsHidden
            }).ToList()
        };
    }
}
```

## Performance Considerations

### Caching

For expensive metadata operations, consider caching:

```csharp
public class CachedMetadataProvider : IPluginMetadataProvider
{
    private readonly IMemoryCache _cache;
    private readonly IPluginMetadataProvider _innerProvider;
    
    public CachedMetadataProvider(IMemoryCache cache, IPluginMetadataProvider innerProvider)
    {
        _cache = cache;
        _innerProvider = innerProvider;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        var key = $"{plugin.Name}:{metadata.Name}";
        
        return _cache.GetOrCreate(key, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return _innerProvider.GetFunctionMetadata(plugin, metadata);
        });
    }
}
```

### Lazy Loading

For providers that need expensive initialization:

```csharp
public class LazyMetadataProvider : IPluginMetadataProvider
{
    private readonly Lazy<IExpensiveService> _expensiveService;
    
    public LazyMetadataProvider()
    {
        _expensiveService = new Lazy<IExpensiveService>(() => new ExpensiveService());
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        // Service is only created when first needed
        return _expensiveService.Value.GetMetadata(plugin.Name, metadata.Name);
    }
}
```

## Error Handling

### Graceful Degradation

Always provide fallbacks for metadata providers:

```csharp
public class SafeMetadataProvider : IPluginMetadataProvider
{
    private readonly IPluginMetadataProvider _innerProvider;
    private readonly ILogger<SafeMetadataProvider> _logger;
    
    public SafeMetadataProvider(IPluginMetadataProvider innerProvider, ILogger<SafeMetadataProvider> logger)
    {
        _innerProvider = innerProvider;
        _logger = logger;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        try
        {
            return _innerProvider.GetFunctionMetadata(plugin, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata for {PluginName}.{FunctionName}", plugin.Name, metadata.Name);
            return null; // Fall back to original metadata
        }
    }
}
```
