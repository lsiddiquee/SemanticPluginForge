#pragma warning disable SKEXP0040
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;

namespace SemanticPluginForge;

/// <summary>
/// Extension methods for adding plugins from OpenAPI sources with metadata to the kernel's plugin collection.
/// </summary>
public static class AddPluginExtensions
{
    /// <summary>
    /// Creates a plugin from an OpenAPI definition file, updates its metadata, and adds it to the kernel's plugin collection.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> containing services, plugins, and other state for use throughout the operation.</param>
    /// <param name="pluginName">The name of the plugin.</param>
    /// <param name="filePath">The file path to the OpenAPI definition file.</param>
    /// <param name="executionParameters">Optional execution parameters for the plugin.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the added <see cref="KernelPlugin"/>.</returns>
    public async static Task<KernelPlugin> AddFromOpenApiWithMetadataAsync(this Kernel kernel, string pluginName, string filePath, OpenApiFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenApiAsync(pluginName, filePath, executionParameters, cancellationToken);
        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    /// <summary>
    /// Creates a plugin from an OpenAPI definition URI, updates its metadata, and adds it to the kernel's plugin collection.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> containing services, plugins, and other state for use throughout the operation.</param>
    /// <param name="pluginName">The name of the plugin.</param>
    /// <param name="uri">The URI to the OpenAPI definition.</param>
    /// <param name="executionParameters">Optional execution parameters for the plugin.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the added <see cref="KernelPlugin"/>.</returns>
    public async static Task<KernelPlugin> AddFromOpenApiWithMetadataAsync(this Kernel kernel, string pluginName, Uri uri, OpenApiFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenApiAsync(pluginName, uri, executionParameters, cancellationToken);
        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    /// <summary>
    /// Creates a plugin from an OpenAPI definition stream, updates its metadata, and adds it to the kernel's plugin collection.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> containing services, plugins, and other state for use throughout the operation.</param>
    /// <param name="pluginName">The name of the plugin.</param>
    /// <param name="stream">The stream representing the OpenAPI definition.</param>
    /// <param name="executionParameters">Optional execution parameters for the plugin.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the added <see cref="KernelPlugin"/>.</returns>
    public async static Task<KernelPlugin> AddFromOpenApiWithMetadataAsync(this Kernel kernel, string pluginName, Stream stream, OpenApiFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenApiAsync(pluginName, stream, executionParameters, cancellationToken);
        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    /// <summary>
    /// Adds the provided kernel plugin to the kernel's plugin collection after patching it with metadata.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> containing services, plugins, and other state for use throughout the operation.</param>
    /// <param name="kernelPlugin">The kernel plugin to be patched and added.</param>
    /// <returns>The patched and added <see cref="KernelPlugin"/>.</returns>
    public static KernelPlugin AddPatchedKernelPluginWithMetadata(this Kernel kernel, KernelPlugin kernelPlugin)
    {
        var patchedPlugin = kernel.Services.PatchKernelPluginWithMetadata(kernelPlugin);
        kernel.Plugins.Add(patchedPlugin);
        return patchedPlugin;
    }
}
