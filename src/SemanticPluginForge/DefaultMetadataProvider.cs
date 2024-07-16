using Microsoft.SemanticKernel;

namespace SemanticPluginForge;

public class DefaultMetadataProvider : IMetadataProvider
{
    public string GetPluginDescription(KernelPlugin plugin) => plugin.Description;

    public KernelFunctionMetadata GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) => metadata;
}