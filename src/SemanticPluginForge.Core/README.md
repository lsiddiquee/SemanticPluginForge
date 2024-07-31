# SemanticPluginForge

SemanticPluginForge is a .NET library for extending Semantic Kernel plugins with metadata support.

## Installation

You can install the SemanticPluginForge package via NuGet:

```sh
dotnet add package SemanticPluginForge
```

## Usage

### Implementing a Custom Metadata Provider

Implement the `IPluginMetadataProvider` interface. The following code is a sample provider that overrides the description of a function from the `TimePlugin`.

```csharp
public class CustomTimeYearMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "TimePlugin" && metadata.Name == "Year"
            ? new FunctionMetadata(metadata.Name) { Description = "Get the current year in 4-digit number format." }
            : null;
}
```

### Registering the Metadata Provider

Register the custom metadata provider with the service collection.

```csharp
services.AddSingleton<IPluginMetadataProvider, CustomTimeYearMetadataProvider>();
```

### Adding Plugins with Patched Metadata

Use the extension methods from this library to add plugins with patched metadata.

```csharp
var kernelBuilder = services.AddKernel();
kernelBuilder.Plugins.AddFromTypeWithMetadata<TimePlugin>();
```
