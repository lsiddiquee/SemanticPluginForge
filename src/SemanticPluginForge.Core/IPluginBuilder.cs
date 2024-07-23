using Microsoft.SemanticKernel;

namespace SemanticPluginForge.Core;

/// <summary>
/// Interface for a plugin builder.
/// </summary>
public interface IPluginBuilder
{
    /// <summary>
    /// Patch a KernelPlugin object with external metadata.
    /// </summary>
    /// <param name="plugin">The plugin which should be patched with external metadata.</param>
    /// <returns>Returns the patched plugin instance.</returns>
    KernelPlugin PatchKernelPluginWithMetadata(KernelPlugin plugin);
}