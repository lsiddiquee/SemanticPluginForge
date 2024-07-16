using Microsoft.SemanticKernel;

namespace SemanticPluginForge;

public interface IPluginBuilder
{
    KernelPlugin PatchKernelPluginWithMetadata(KernelPlugin kernelPlugin);
}