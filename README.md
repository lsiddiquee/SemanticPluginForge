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

### Implementing a Custom Metadata Provider

Implement the `IPluginMetadataProvider` interface. The following code is a sample provider that overrides the description of a function from the `TimePlugin`.

```csharp
public class CustomTimeYearMetadataProvider : IPluginMetadataProvider
{
    public string GetPluginDescription(KernelPlugin plugin) => plugin.Description;

    public KernelFunctionMetadata GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name == "TimePlugin" && metadata.Name == "Year"
            ? new KernelFunctionMetadata(metadata) { Description = "Get the current year in 4-digit number format." }
            : metadata;
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

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the LICENSE file for details.