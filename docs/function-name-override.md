# Function Name Override

SemanticPluginForge allows you to override function names without modifying the original code. This is useful for providing better naming conventions, API versioning, or adapting legacy code for AI consumption.

## Why Override Function Names?

### Better Naming for AI

- **Descriptive Names**: Convert technical method names to AI-friendly descriptions
- **Natural Language**: Use names that better match how users might ask for functionality
- **Consistency**: Standardize naming across different plugins
- **Clarity**: Make function purposes more obvious

### API Evolution

- **Versioning**: Provide new names while maintaining backward compatibility
- **Migration**: Gradually transition from old to new naming conventions
- **Standardization**: Align with organizational naming standards

## Basic Usage

### Setting Override Function Name

Use the `OverrideFunctionName` property in `FunctionMetadata`:

```csharp
public class FunctionNameMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "UserPlugin" && metadata.Name == "GetCurrentUsername")
        {
            return new FunctionMetadata(metadata.Name) 
            { 
                OverrideFunctionName = "GetUser",
                Description = "Gets the current logged in user"
            };
        }
        return null;
    }
}
```

### Result

- **Original function name**: `GetCurrentUsername`
- **New function name**: `GetUser`
- The function is accessible as `GetUser` in the enhanced plugin
- The original `GetCurrentUsername` is no longer available

## Practical Examples

### Example 1: Making Names More Descriptive

```csharp
public class DescriptiveNamingProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "DataPlugin")
        {
            return metadata.Name switch
            {
                "GetData" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "GetCustomerData",
                    Description = "Retrieves customer data from the database"
                },
                "SaveData" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "SaveCustomerData",
                    Description = "Saves customer data to the database"
                },
                "DeleteData" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "DeleteCustomerData",
                    Description = "Removes customer data from the database"
                },
                _ => null
            };
        }
        return null;
    }
}
```

### Example 2: Natural Language Names

```csharp
public class NaturalLanguageNamingProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "WeatherPlugin")
        {
            return metadata.Name switch
            {
                "GetCurrentTemperature" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "WhatIsTheTemperature",
                    Description = "Gets the current temperature for a location"
                },
                "GetWeatherForecast" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "WhatWillTheWeatherBeLike",
                    Description = "Gets the weather forecast for upcoming days"
                },
                "IsItRaining" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "IsItRainingToday",
                    Description = "Checks if it's currently raining at a location"
                },
                _ => null
            };
        }
        return null;
    }
}
```

### Example 3: Standardizing Across Plugins

```csharp
public class StandardizedNamingProvider : IPluginMetadataProvider
{
    private readonly Dictionary<string, string> _standardNames = new()
    {
        // Standardize CRUD operations across all plugins
        { "Create", "Add" },
        { "Read", "Get" },
        { "Update", "Modify" },
        { "Delete", "Remove" },
        
        // Standardize common operations
        { "Retrieve", "Get" },
        { "Fetch", "Get" },
        { "Insert", "Add" },
        { "Destroy", "Remove" }
    };

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        // Apply standardization to any plugin
        foreach (var (oldName, newName) in _standardNames)
        {
            if (metadata.Name.StartsWith(oldName))
            {
                var standardizedName = metadata.Name.Replace(oldName, newName);
                return new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = standardizedName,
                    Description = metadata.Description
                };
            }
        }
        return null;
    }
}
```

## Advanced Scenarios

### Context-Aware Naming

Provide different names based on context or configuration:

```csharp
public class ContextAwareNamingProvider : IPluginMetadataProvider
{
    private readonly IConfiguration _configuration;
    
    public ContextAwareNamingProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        var useVerboseNames = _configuration.GetValue<bool>("UseVerboseNames", false);
        
        if (plugin.Name == "CalculatorPlugin")
        {
            return metadata.Name switch
            {
                "Add" when useVerboseNames => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "AddTwoNumbers"
                },
                "Subtract" when useVerboseNames => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "SubtractTwoNumbers"
                },
                "Add" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "Plus"
                },
                "Subtract" => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "Minus"
                },
                _ => null
            };
        }
        return null;
    }
}
```

### Localization Support

Provide localized function names:

```csharp
public class LocalizedNamingProvider : IPluginMetadataProvider
{
    private readonly IStringLocalizer _localizer;
    
    public LocalizedNamingProvider(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        // Get localized function name
        var localizedName = _localizer[$"Function.{plugin.Name}.{metadata.Name}"];
        
        if (!localizedName.ResourceNotFound)
        {
            return new FunctionMetadata(metadata.Name)
            {
                OverrideFunctionName = localizedName.Value,
                Description = _localizer[$"Function.{plugin.Name}.{metadata.Name}.Description"]
            };
        }
        
        return null;
    }
}
```

### Version-Based Naming

Provide different names for different API versions:

```csharp
public class VersionedNamingProvider : IPluginMetadataProvider
{
    private readonly string _apiVersion;
    
    public VersionedNamingProvider(IConfiguration configuration)
    {
        _apiVersion = configuration.GetValue<string>("ApiVersion", "v1");
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "UserPlugin")
        {
            return (_apiVersion, metadata.Name) switch
            {
                ("v1", "GetUserInfo") => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "GetUser" // Simple name for v1
                },
                ("v2", "GetUserInfo") => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "GetUserProfile" // More descriptive for v2
                },
                ("v1", "UpdateUserInfo") => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "UpdateUser"
                },
                ("v2", "UpdateUserInfo") => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "UpdateUserProfile"
                },
                _ => null
            };
        }
        return null;
    }
}
```

## Testing Function Name Overrides

### Unit Testing

```csharp
[Test]
public void FunctionNameOverride_ShouldUseNewName()
{
    // Arrange
    var metadataProvider = new Mock<IPluginMetadataProvider>();
    metadataProvider.Setup(p => p.GetFunctionMetadata(
        It.IsAny<KernelPlugin>(), 
        It.Is<KernelFunctionMetadata>(f => f.Name == "GetCurrentUsername")))
        .Returns(new FunctionMetadata("GetCurrentUsername") 
        { 
            OverrideFunctionName = "GetUser" 
        });

    var builder = new PluginBuilder(metadataProvider.Object);
    var originalPlugin = KernelPluginFactory.CreateFromObject(new TestPlugin());

    // Act
    var enhancedPlugin = builder.PatchKernelPluginWithMetadata(originalPlugin);

    // Assert
    Assert.True(enhancedPlugin.Any(f => f.Name == "GetUser"));
    Assert.False(enhancedPlugin.Any(f => f.Name == "GetCurrentUsername"));
}

[Test]
public async Task OverriddenFunction_ShouldWorkCorrectly()
{
    // Arrange
    var metadataProvider = new TestMetadataProvider();
    var builder = new PluginBuilder(metadataProvider);
    var originalPlugin = KernelPluginFactory.CreateFromObject(new TestPlugin());

    // Act
    var enhancedPlugin = builder.PatchKernelPluginWithMetadata(originalPlugin);
    var overriddenFunction = enhancedPlugin["GetUser"]; // New name
    var result = await overriddenFunction.InvokeAsync(new Kernel());

    // Assert
    Assert.Equal("expected_username", result.GetValue<string>());
}
```

### Integration Testing

```csharp
[Test]
public async Task EndToEnd_FunctionNameOverride_ShouldWork()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddSingleton<IPluginMetadataProvider, TestNamingProvider>();
    
    var kernelBuilder = services.AddKernel();
    kernelBuilder.Plugins.AddFromTypeWithMetadata<UserPlugin>();
    
    var serviceProvider = services.BuildServiceProvider();
    var kernel = serviceProvider.GetRequiredService<Kernel>();

    // Act
    var result = await kernel.InvokeAsync("UserPlugin", "GetUser"); // Using overridden name

    // Assert
    Assert.NotNull(result);
    Assert.Equal("test_user", result.GetValue<string>());
}
```

## Best Practices

### 1. Maintain Consistency

Choose a naming convention and apply it consistently:

```csharp
// Good: Consistent verb-noun pattern
"GetCustomer", "AddCustomer", "UpdateCustomer", "RemoveCustomer"

// Avoid: Inconsistent patterns
"GetCustomer", "CustomerAdd", "UpdateCust", "DeleteCustomer"
```

### 2. Use Meaningful Names

Choose names that clearly indicate what the function does:

```csharp
// Good: Clear and specific
"CalculateMonthlyInterest", "SendWelcomeEmail", "ValidateUserInput"

// Avoid: Vague or unclear
"DoStuff", "Process", "Handle"
```

### 3. Consider Your Audience

Tailor function names to your intended users:

```csharp
// For technical users
"ExecuteSQLQuery", "ParseJSON", "ValidateRegex"

// For business users
"GetSalesReport", "CalculateCommission", "SendInvoice"

// For general AI consumption
"WhatAreTheSalesThisMonth", "HowMuchCommissionIsOwed", "SendBillToCustomer"
```

### 4. Document the Mapping

Keep track of original names and their overrides:

```csharp
/// <summary>
/// Function name mappings:
/// - GetCurrentUsername -> GetUser
/// - GetUserInformation -> GetUserProfile
/// - AuthenticateUser -> LoginUser
/// </summary>
public class UserPluginNamingProvider : IPluginMetadataProvider
{
    // Implementation...
}
```

### 5. Plan for Migration

When changing existing function names, consider backward compatibility:

```csharp
public class MigrationNamingProvider : IPluginMetadataProvider
{
    private readonly bool _useNewNames;
    
    public MigrationNamingProvider(IConfiguration configuration)
    {
        _useNewNames = configuration.GetValue<bool>("UseNewFunctionNames", false);
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (!_useNewNames) return null; // Keep original names
        
        // Apply new names only when enabled
        return metadata.Name switch
        {
            "GetData" => new FunctionMetadata(metadata.Name)
            {
                OverrideFunctionName = "RetrieveInformation"
            },
            _ => null
        };
    }
}
```
