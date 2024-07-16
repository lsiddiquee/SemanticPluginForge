using Microsoft.SemanticKernel;

namespace SemanticPluginForge;

// TODO: Add use case support, to allow different metadata for different use cases.
public interface IPluginMetadataProvider
{
    string GetPluginDescription(KernelPlugin plugin);

    KernelFunctionMetadata GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata);
}
