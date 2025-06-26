# Suppressing Functions and Parameters

The library supports suppressing functions and parameters in plugin metadata. This feature allows you to hide specific functions or parameters from the plugin's consumers while maintaining functionality.

## Suppressing Functions

To suppress a function, set the `Suppress` property to `true` in the `FunctionMetadata`.

### Example: Hiding a Deprecated Function

```csharp
public class CustomMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "SamplePlugin" && metadata.Name == "HiddenFunction"
            ? new FunctionMetadata(metadata.Name) { Suppress = true }
            : null;
}
```

### Use Cases for Function Suppression

- **Deprecating functions**: Hide old functions while maintaining backward compatibility
- **Environment-specific availability**: Show different functions in development vs. production
- **User role-based access**: Hide admin functions from regular users
- **Feature flags**: Control function availability through configuration

## Suppressing Parameters

To suppress a parameter, set the `Suppress` property to `true` in the `ParameterMetadata`. If a default value is provided, it will be used when the function is called.

### Example: Hiding a Parameter with Default Value

```csharp
public class CustomMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "SamplePlugin" && metadata.Name == "FunctionWithHiddenParameter"
            ? new FunctionMetadata(metadata.Name)
            {
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("hiddenParam") 
                    { 
                        Suppress = true, 
                        DefaultValue = "default" 
                    }
                }
            }
            : null;
}
```

## Important Rules for Parameter Suppression

### Required Parameters Must Have Defaults

When a parameter is set to `Suppress`, it must either be optional or have a default value provided. This default value can be specified in the underlying implementation or through the metadata provider.

```csharp
// This will throw an ArgumentException if the parameter is required and has no default
new ParameterMetadata("requiredParam") { Suppress = true } // ❌ Error!

// Correct approaches:
new ParameterMetadata("requiredParam") { Suppress = true, DefaultValue = "value" } // ✅ OK
new ParameterMetadata("optionalParam") { Suppress = true } // ✅ OK (if originally optional)
```

### Default Value Precedence

If a default value is provided through the metadata provider, it takes precedence over the default value specified in the original plugin implementation.

```csharp
// Original function signature:
public string MyFunction(string param = "original")

// Metadata override:
new ParameterMetadata("param") 
{ 
    Suppress = true, 
    DefaultValue = "overridden" // This value will be used
}
```

## Practical Examples

### Example 1: Context-Aware Parameters

Hide parameters that should be automatically resolved from context:

```csharp
public class ContextAwareMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "WeatherPlugin" && metadata.Name == "GetWeather")
        {
            return new FunctionMetadata(metadata.Name)
            {
                Description = "Gets weather information for the user's location",
                Parameters = new List<ParameterMetadata>
                {
                    // Hide the location parameter and auto-resolve from user context
                    new ParameterMetadata("location") 
                    { 
                        Suppress = true, 
                        DefaultValue = "user_location_from_context",
                        Description = "User location automatically resolved from context"
                    }
                }
            };
        }
        return null;
    }
}
```

### Example 2: Simplifying Complex APIs

Hide technical parameters from end users:

```csharp
public class SimplifiedAPIMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "DatabasePlugin" && metadata.Name == "QueryData")
        {
            return new FunctionMetadata(metadata.Name)
            {
                Description = "Queries data from the database",
                Parameters = new List<ParameterMetadata>
                {
                    // Hide technical parameters
                    new ParameterMetadata("connectionString") 
                    { 
                        Suppress = true, 
                        DefaultValue = "server_default_connection"
                    },
                    new ParameterMetadata("timeout") 
                    { 
                        Suppress = true, 
                        DefaultValue = 30
                    },
                    new ParameterMetadata("retryCount") 
                    { 
                        Suppress = true, 
                        DefaultValue = 3
                    }
                    // Keep user-facing parameters visible
                }
            };
        }
        return null;
    }
}
```

### Example 3: Environment-Specific Configuration

Use different defaults based on environment:

```csharp
public class EnvironmentMetadataProvider : IPluginMetadataProvider
{
    private readonly IConfiguration _configuration;
    
    public EnvironmentMetadataProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "LoggingPlugin" && metadata.Name == "LogMessage")
        {
            var logLevel = _configuration.GetValue<string>("DefaultLogLevel", "Info");
            
            return new FunctionMetadata(metadata.Name)
            {
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("logLevel") 
                    { 
                        Suppress = true, 
                        DefaultValue = logLevel // Environment-specific default
                    }
                }
            };
        }
        return null;
    }
}
```

## Testing Suppressed Elements

### Testing Function Suppression

```csharp
[Test]
public void SuppressedFunction_ShouldNotAppearInPlugin()
{
    var provider = new TestMetadataProvider();
    var builder = new PluginBuilder(provider);
    var originalPlugin = KernelPluginFactory.CreateFromObject(new TestPlugin());
    
    var enhancedPlugin = builder.PatchKernelPluginWithMetadata(originalPlugin);
    
    // Function should be suppressed
    Assert.False(enhancedPlugin.Any(f => f.Name == "SuppressedFunction"));
}
```

### Testing Parameter Suppression

```csharp
[Test]
public async Task SuppressedParameter_ShouldUseDefaultValue()
{
    var provider = new TestMetadataProvider();
    var builder = new PluginBuilder(provider);
    var originalPlugin = KernelPluginFactory.CreateFromObject(new TestPlugin());
    
    var enhancedPlugin = builder.PatchKernelPluginWithMetadata(originalPlugin);
    var function = enhancedPlugin["TestFunction"];
    
    // Call function without providing the suppressed parameter
    var result = await function.InvokeAsync(new Kernel(), new KernelArguments());
    
    // Should use the default value from metadata provider
    Assert.Equal("expected_default_value", result.GetValue<string>());
}
```

## Best Practices

### 1. Document Suppressed Elements

Always document why elements are suppressed:

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    if (plugin.Name == "AdminPlugin" && metadata.Name == "DeleteAllData")
    {
        // Suppress dangerous admin function in production for safety
        return new FunctionMetadata(metadata.Name) { Suppress = true };
    }
    return null;
}
```

### 2. Use Meaningful Default Values

Choose default values that make sense in context:

```csharp
new ParameterMetadata("userId") 
{ 
    Suppress = true, 
    DefaultValue = "current_authenticated_user", // Clear intent
    Description = "Automatically resolved from current user session"
}
```

### 3. Consider Gradual Migration

Use suppression for gradual API changes:

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    // Phase 1: Keep old parameter but suppress it
    if (plugin.Name == "APIv2" && metadata.Name == "ProcessData")
    {
        return new FunctionMetadata(metadata.Name)
        {
            Parameters = new List<ParameterMetadata>
            {
                // Gradually migrate from old parameter to new one
                new ParameterMetadata("oldFormat") 
                { 
                    Suppress = true, 
                    DefaultValue = false // Use new format by default
                }
            }
        };
    }
    return null;
}
```
