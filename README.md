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

### CLR Type Discovery and Method Selection

When registering CLR types or objects as plugins, the library follows these rules:

1. **Open Generic Methods**: Open generic methods are filtered out and not exposed as plugin functions.

2. **TAP Method Normalization**: Methods following the Task-based Asynchronous Pattern (TAP) are normalized by:
   - Removing any `CancellationToken` parameter
   - Removing the `Async` suffix from method names
   - Preferred selection order is: method with `CancellationToken` parameter, then method with `Async` suffix

3. **Handling Overloaded Methods**: For methods with the same name but different parameters, you can use `FunctionMetadata.OverrideFunctionName` in your metadata provider to give each overload a unique name:

```csharp
// Handling the Random.Next() overloads
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    return plugin.Name switch
    {
        "RandomPlugin" => metadata.Name switch
        {
            "Next" when metadata.Parameters.Count == 0 => new FunctionMetadata(metadata.Name)
            {
                Description = "Returns a non-negative random integer."
            },
            "Next" when metadata.Parameters.Count == 1 => new FunctionMetadata(metadata.Name)
            {
                OverrideFunctionName = "NextWithUpperBound",
                Description = "Returns a random integer within a specified range.",
                Parameters = [
                    new ParameterMetadata("maxValue") 
                    { 
                        Description = "The exclusive upper bound of the random number returned." 
                    }
                ]
            },
            "Next" when metadata.Parameters.Count == 2 => new FunctionMetadata(metadata.Name)
            {
                OverrideFunctionName = "NextWithRange",
                Description = "Returns a random integer within a specified range.",
                Parameters = [
                    new ParameterMetadata("minValue") { Description = "The inclusive lower bound of the random number returned." },
                    new ParameterMetadata("maxValue") { Description = "The exclusive upper bound of the random number returned." }
                ]
            },
            _ => null
        },
        _ => null
    };
}
```

**Sample Type and Metadata Provider:**

```csharp
public class DateTimeWrapper
{
  public string ToShortDateString()
  {
    return DateTime.Now.ToShortDateString();
  }
  
  public string ToLongDateString()
  {
    return DateTime.Now.ToLongDateString();
  }
  
  public string CurrentTime()
  {
    return DateTime.Now.ToString("T");
  }
}

public class CustomMetadataProvider : IPluginMetadataProvider
{
  public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
    plugin.Name == "DateTimeWrapper" ? new PluginMetadata
    {
      Description = "This plugin returns date and time information."
    } : null;

  public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
    plugin.Name == "DateTimeWrapper" && metadata.Name == "ToShortDateString" ? new FunctionMetadata(metadata.Name)
    {
      Description = "Returns the current date in short format (MM/dd/yyyy)."
    } : null;
}
```

### CreateFromClrObjectWithMetadata: Using an existing object to create a plugin

**Usage Example:**

```csharp
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(new DateTimeWrapper(), "DateTimeWrapper");
```

### CreateFromClrTypeWithMetadata: Using an existing type to create a plugin

**Usage Example:**

```csharp
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<DateTimeWrapper>("DateTimeWrapper");
```

### Example with Azure SDK Client

**Usage Example:**

```csharp
var qc = new QueueClient(new Uri("https://your-storage.queue.core.windows.net/your-queue"), new DefaultAzureCredential());
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(qc, "Queue");
```

## Samples

Explore the [`samples`](https://github.com/lsiddiquee/SemanticPluginForge/samples/) directory for practical examples of using `SemanticPluginForge` in different scenarios. Each sample includes comprehensive documentation, setup instructions, and focuses on specific framework concepts.

### Available Samples

- **[DefaultValue](https://github.com/lsiddiquee/SemanticPluginForge/samples/DefaultValue/)**: Demonstrates advanced parameter handling including suppression, default values, and context-aware metadata. Shows how to override parameter descriptions and ensure parameters are never resolved from context when suppressed.

- **[UseClrType](https://github.com/lsiddiquee/SemanticPluginForge/samples/UseClrType/)**: Shows how to use existing .NET classes as Semantic Kernel plugins without requiring `KernelFunction` attributes. Demonstrates multiple plugin types including custom classes, standard .NET classes like `Random`, and direct Azure SDK integration through `QueueClient`. Features function name overriding for handling method overloads.

- **[AzureAiSearchPlugin](https://github.com/lsiddiquee/SemanticPluginForge/samples/AzureAiSearchPlugin/)**: Comprehensive example showing how to create multiple instances of the same plugin class with different metadata configurations for various data sources. Uses mocked data for learning without external dependencies.

Navigate to the [`samples`](https://github.com/lsiddiquee/SemanticPluginForge/samples/) folder to get started with these examples.

## Documentation

For comprehensive documentation, API reference, and advanced usage examples, visit:

**[https://lsiddiquee.github.io/SemanticPluginForge/](https://lsiddiquee.github.io/SemanticPluginForge/)**

## Support

Need help or have questions? Check out our [Support Guide](https://github.com/lsiddiquee/SemanticPluginForge/support.md) for information on:

- Getting help and reporting issues
- Community discussions
- Contributing guidelines
- Additional resources

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
