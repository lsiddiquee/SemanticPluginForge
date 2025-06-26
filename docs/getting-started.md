# Getting Started

This guide will walk you through the basic setup and usage of SemanticPluginForge.

## Installation

Add `SemanticPluginForge` to your project using the .NET CLI:

```bash
dotnet add package SemanticPluginForge.Core
```

Or via Package Manager in Visual Studio:

```powershell
Install-Package SemanticPluginForge.Core
```

## Basic Usage

### Step 1: Implement a Custom Metadata Provider

Create a class that implements the `IPluginMetadataProvider` interface:

```csharp
using SemanticPluginForge.Core;
using Microsoft.SemanticKernel;

public class CustomTimeYearMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "TimePlugin" && metadata.Name == "Year"
            ? new FunctionMetadata(metadata.Name) 
            { 
                Description = "Get the current year in 4-digit number format." 
            }
            : null;
}
```

### Step 2: Register the Metadata Provider

Register your custom metadata provider with the dependency injection container:

```csharp
using Microsoft.Extensions.DependencyInjection;

// In your service configuration
services.AddSingleton<IPluginMetadataProvider, CustomTimeYearMetadataProvider>();
```

### Step 3: Add Plugins with Enhanced Metadata

Use the extension methods provided by SemanticPluginForge to add plugins with patched metadata:

```csharp
using SemanticPluginForge;

var kernelBuilder = services.AddKernel();

// Add a plugin from a type with metadata enhancement
kernelBuilder.Plugins.AddFromTypeWithMetadata<TimePlugin>();

// Add a plugin from an object with metadata enhancement
kernelBuilder.Plugins.AddFromObjectWithMetadata(new TimePlugin(), "TimePlugin");

// Add a plugin from CLR type without KernelFunction attributes
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<MyCustomClass>("CustomPlugin");
```

## Complete Example

Here's a complete working example:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using SemanticPluginForge;
using SemanticPluginForge.Core;

// Define your plugin class
public class TimePlugin
{
    [KernelFunction]
    public string Year() => DateTime.Now.Year.ToString();
    
    [KernelFunction]
    public string Month() => DateTime.Now.Month.ToString();
}

// Implement metadata provider
public class TimeMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name == "TimePlugin" ? new PluginMetadata
        {
            Description = "Enhanced time and date operations plugin"
        } : null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name != "TimePlugin") return null;
        
        return metadata.Name switch
        {
            "Year" => new FunctionMetadata(metadata.Name) 
            { 
                Description = "Gets the current year as a 4-digit string" 
            },
            "Month" => new FunctionMetadata(metadata.Name) 
            { 
                Description = "Gets the current month as a number (1-12)" 
            },
            _ => null
        };
    }
}

// Application setup
var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.AddSingleton<IPluginMetadataProvider, TimeMetadataProvider>();

// Configure Kernel
var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.Plugins.AddFromTypeWithMetadata<TimePlugin>();

var app = builder.Build();

// Use the kernel
var kernel = app.Services.GetRequiredService<Kernel>();
var result = await kernel.InvokeAsync("TimePlugin", "Year");
Console.WriteLine($"Current year: {result}");
```

## Next Steps

Now that you have the basics working, explore these advanced topics:

- [Core Concepts](core-concepts.md) - Understand the fundamental concepts
- [Advanced Features](advanced-features.md) - Learn about suppression, CLR types, and more
- [Samples](samples.md) - Explore practical examples
- [API Reference](xref:SemanticPluginForge) - Detailed API documentation

## Troubleshooting

### Common Issues

**Issue**: Plugin metadata is not being applied

- **Solution**: Ensure your metadata provider is registered in the DI container and that the plugin/function names match exactly

**Issue**: Required parameters cannot be suppressed

- **Solution**: When suppressing required parameters, provide a default value in your metadata provider

**Issue**: CLR types not working as plugins

- **Solution**: Ensure your methods are public and consider implementing metadata providers for better descriptions
