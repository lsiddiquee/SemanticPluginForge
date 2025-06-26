# SemanticPluginForge

`SemanticPluginForge` adds functionality to dynamically alter the metadata for SemanticKernel plugins. This library introduces the `IPluginMetadataProvider` interface, allowing for real-time updates to plugin metadata, including descriptions, return value descriptions, and parameter descriptions, without the need for redeployment.

## Benefits

- **Dynamic Metadata Updates**:
  - Make real-time updates to plugin metadata, enhancing flexibility and reducing downtime.
  - Implement changes without redeployment, ensuring a seamless update process.

- **Extensible Architecture**:
  - Implement new metadata providers, such as a database-backed provider, to enable metadata changes without requiring a service restart.
  - Support various use cases and future expansions.

- **Dynamic Tuning**:
  - Fine-tune plugin descriptions and parameters based on evolving requirements or user feedback.
  - Quickly respond to changes in business logic or user expectations without interrupting service availability.

- **Custom Metadata Providers**:
  - Develop custom providers that fetch metadata from different sources, such as databases, remote services, or configuration management systems.
  - Achieve higher levels of customization and control over plugin behavior.

## Usage

### Adding the Library to Your Project
  
To add `SemanticPluginForge` to your project, use the following command:

```bash
dotnet add package SemanticPluginForge.Core
```

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

## Suppressing Functions and Parameters

The library now supports suppressing functions and parameters in plugin metadata. This feature allows you to hide specific functions or parameters from the plugin's consumers while maintaining functionality. For parameters, if a default value is provided, it will be used even when the parameter is suppressed.

### Suppressing a Function

To suppress a function, set the `Suppress` property to `true` in the `FunctionMetadata`.

```csharp
public class CustomMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "SamplePlugin" && metadata.Name == "HiddenFunction"
            ? new FunctionMetadata(metadata.Name) { Suppress = true }
            : null;
}
```

### Suppressing a Parameter

To suppress a parameter, set the `Suppress` property to `true` in the `ParameterMetadata`. If a default value is provided, it will be used.

```csharp
public class CustomMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) => null;

    public FunctionMetadata GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "SamplePlugin" && metadata.Name == "FunctionWithHiddenParameter"
            ? new FunctionMetadata(metadata.Name)
            {
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("hiddenParam") { Suppress = true, DefaultValue = "default" }
                }
            }
            : null;
}
```

#### Additional Notes on Suppressing Parameters

When a parameter is set to `Suppress`, it must either be optional or have a default value provided. This default value can be specified in the underlying implementation or through the metadata provider.

- **Default Value Precedence**: If a default value is provided through the metadata provider, it takes precedence over the default value specified in the original plugin implementation.

This ensures that suppressed parameters are handled gracefully without causing runtime errors.

## Using any CLR types and objects as plugins
  
The library allows you to use any CLR type or object as a plugin without requiring `KernelFunction` attribute. This enables you to create plugins from existing objects or types, making it easier to integrate with existing codebases.

**Sample Type and Metadata Provider:**

```csharp
public class ShortDate
{
  public string ToShortDateString()
  {
    return DateTime.Now.ToShortDateString();
  }
}

public class CustomMetadataProvider : IPluginMetadataProvider
{
  public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
    plugin.Name == "ShortDatePlugin" ? new PluginMetadata
    {
      Description = "This plugin returns date and time information."
    } : null;

  public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
    plugin.Name == "ShortDatePlugin" && metadata.Name == "ToShortDateString" ? new FunctionMetadata(metadata.Name)
    {
      Description = "Returns the date in short format."
    } : null;
}
```

### CreateFromClrObjectWithMetadata: Using an existing object to create a plugin

**Usage Example:**

```csharp
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(new ShortDate(), "ShortDatePlugin");
```

### CreateFromClrTypeWithMetadata: Using an existing type to create a plugin

**Usage Example:**

```csharp
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<ShortDate>("ShortDatePlugin");
```

## Samples

Explore the [`samples`](./samples/) directory for practical examples of using `SemanticPluginForge` in different scenarios. Each sample includes comprehensive documentation, setup instructions, and focuses on specific framework concepts.

### Available Samples

- **[DefaultValue](./samples/DefaultValue/)**: Demonstrates advanced parameter handling including suppression, default values, and context-aware metadata. Shows how to override parameter descriptions and ensure parameters are never resolved from context when suppressed.

- **[UseClrType](./samples/UseClrType/)**: Shows how to use existing .NET classes as Semantic Kernel plugins without requiring `KernelFunction` attributes. Demonstrates both type-based and object-based registration approaches.

- **[AzureAiSearchPlugin](./samples/AzureAiSearchPlugin/)**: Comprehensive example showing how to create multiple instances of the same plugin class with different metadata configurations for various data sources. Uses mocked data for learning without external dependencies.

Navigate to the [`samples`](./samples/) folder to get started with these examples.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.