using Microsoft.SemanticKernel;

namespace SemanticPluginForge.Core;

/// <summary>
/// Interface for a function builder.
/// </summary>
public interface IFunctionBuilder
{
    /// <summary>
    /// Patch a KernelFunction object.
    /// </summary>
    /// <param name="plugin">The plugin which contains the function.</param>
    /// <param name="function">The function which should be patched.</param>
    /// <returns>Returns null if the function should be suppressed, <paramref name="function"/> if the function should not be patched otherwise the patched function.</returns>
    KernelFunction? PatchKernelFunctionWithMetadata(KernelPlugin plugin, KernelFunction function);
}