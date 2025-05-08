using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

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
