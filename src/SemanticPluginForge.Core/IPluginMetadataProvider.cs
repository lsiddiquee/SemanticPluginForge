using Microsoft.SemanticKernel;

namespace SemanticPluginForge.Core;

/// <summary>
/// Interface for a plugin metadata provider.
/// </summary>
public interface IPluginMetadataProvider
{
    /// <summary>
    /// Gets the description for a plugin.
    /// </summary>
    /// <param name="plugin">The plugin for which the description should be returned.</param>
    /// <returns>Returns plugin metadata or null.</returns>
    PluginMetadata? GetPluginMetadata(KernelPlugin plugin);

    /// <summary>
    /// Gets the patched metadata for a function.
    /// </summary>
    /// <param name="plugin">The plugin where which this function belongs to.</param>
    /// <param name="metadata">The original metadata for the function.</param>
    /// <returns>Returns a new instance of the function metadata with patched descriptions or null.</returns>
    FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata);
}
