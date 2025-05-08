using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

public class CustomMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name switch
        {
            "WeatherPlugin" => metadata.Name == "GetTemperatureByCity" ? new FunctionMetadata(metadata.Name)
            {
                Description = metadata.Description,
                Parameters = [
                    new ParameterMetadata("name")
                    {
                        Description = "The name of the city should be retrieved from the user context, if not in context, please ask the user.",
                        IsRequired = true,
                    },
                    new ParameterMetadata("unit")
                    {
                        Description = "This description does not matter as this will always be suppressed and the default will be used.",
                        IsRequired = false,
                        Suppress = true,
                        DefaultValue = "fahrenheit",
                    }
                ],
                ReturnParameter = new ReturnParameterMetadata
                {
                    Description = "The temperature of the city in the specified unit."
                },
            }
            : null,
            _ => null,
        };

    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name switch
        {
            "WeatherPlugin" => new PluginMetadata { Description = "This plugin can be used to retrieve information about the weather." },
            _ => null,
        };
}
