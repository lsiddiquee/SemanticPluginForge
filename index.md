---
_layout: landing
---

# SemanticPluginForge

> Dynamically enhance your Semantic Kernel plugins with powerful metadata capabilities

`SemanticPluginForge` adds functionality to dynamically alter the metadata for SemanticKernel plugins. This library introduces the `IPluginMetadataProvider` interface, allowing for real-time updates to plugin metadata, including descriptions, return value descriptions, and parameter descriptions, without the need for redeployment.

## 🚀 Key Features

- **🔄 Dynamic Metadata Updates**: Make real-time updates to plugin metadata without redeployment
- **🏗️ Extensible Architecture**: Implement custom metadata providers from databases, APIs, or configuration
- **🎯 Function & Parameter Suppression**: Hide functions or parameters while maintaining functionality
- **⚡ CLR Type Support**: Use any .NET class as a plugin without `KernelFunction` attributes
- **🏷️ Function Name Override**: Change function names without modifying source code
- **⚙️ Context-Aware Defaults**: Provide intelligent default values based on runtime conditions

## 📦 Quick Installation

```bash
dotnet add package SemanticPluginForge.Core
```

## 🎯 Quick Start

### 1. Create a Metadata Provider

```csharp
public class CustomMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name == "TimePlugin" ? new PluginMetadata
        {
            Description = "Enhanced time and date operations"
        } : null;

    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "TimePlugin" && metadata.Name == "Year"
            ? new FunctionMetadata(metadata.Name) 
            { 
                Description = "Get the current year in 4-digit format" 
            }
            : null;
}
```

### 2. Register and Use

```csharp
// Register the metadata provider
services.AddSingleton<IPluginMetadataProvider, CustomMetadataProvider>();

// Add plugins with enhanced metadata
var kernelBuilder = services.AddKernel();
kernelBuilder.Plugins.AddFromTypeWithMetadata<TimePlugin>();
```

## 📚 Documentation

| Section | Description |
|---------|-------------|
| [🎯 Getting Started](docs/getting-started.md) | Step-by-step setup and basic usage |
| [🧠 Core Concepts](docs/core-concepts.md) | Understand the fundamentals |
| [⚡ Advanced Features](docs/advanced-features.md) | Suppression, CLR types, and more |
| [📖 Samples](docs/samples.md) | Practical examples and tutorials |
| [📋 API Reference](xref:SemanticPluginForge) | Complete API documentation |

## 🎪 Live Examples

### Suppress Parameters with Smart Defaults

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    if (plugin.Name == "WeatherPlugin" && metadata.Name == "GetWeather")
    {
        return new FunctionMetadata(metadata.Name)
        {
            Description = "Gets weather for user's location (auto-detected)",
            Parameters = new List<ParameterMetadata>
            {
                new ParameterMetadata("location") 
                { 
                    Suppress = true, 
                    DefaultValue = "user_current_location"
                }
            }
        };
    }
    return null;
}
```

### Use Any Class as a Plugin

```csharp
public class MathUtils
{
    public double Add(double a, double b) => a + b;
    public double Multiply(double a, double b) => a * b;
}

// No attributes needed!
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<MathUtils>("MathPlugin");
```

### Override Function Names

```csharp
return new FunctionMetadata(metadata.Name)
{
    OverrideFunctionName = "CalculateSum", // Better name for AI
    Description = "Adds two numbers together"
};
```

## 🏆 Benefits

### For Developers

- **Legacy Integration**: Turn existing code into AI-callable functions
- **Clean Architecture**: Keep AI concerns separate from business logic
- **Rapid Prototyping**: Quickly expose any functionality to AI

### For Operations

- **Zero Downtime Updates**: Change plugin behavior without restarts
- **Environment-Specific Config**: Different metadata per environment
- **A/B Testing**: Test different function descriptions and parameters

### For AI Applications

- **Better Function Discovery**: More descriptive names and descriptions
- **Context-Aware Behavior**: Functions adapt to user context
- **Simplified Interfaces**: Hide technical complexity from AI models

## 🎯 Sample Projects

Explore our comprehensive samples:

| Sample | Focus | Key Learning |
|--------|-------|--------------|
| [**DefaultValue**](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/DefaultValue) | Parameter handling | Smart defaults and suppression |
| [**UseClrType**](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/UseClrType) | Legacy integration | CLR types as plugins |
| [**AzureAiSearchPlugin**](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/AzureAiSearchPlugin) | Production patterns | Multiple instances, different configs |

## 🚀 Getting Started

Ready to enhance your Semantic Kernel plugins?

1. **[📖 Read the Introduction](docs/introduction.md)** - Understand what SemanticPluginForge can do
2. **[🎯 Follow Getting Started](docs/getting-started.md)** - Get up and running in minutes
3. **[🧪 Try the Samples](docs/samples.md)** - Explore practical examples
4. **[⚡ Explore Advanced Features](docs/advanced-features.md)** - Unlock the full potential

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.
