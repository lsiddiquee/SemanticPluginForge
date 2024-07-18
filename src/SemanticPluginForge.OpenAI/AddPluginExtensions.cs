#pragma warning disable SKEXP0040
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;

namespace SemanticPluginForge;

public static class AddPluginExtensions
{
    public async static Task<KernelPlugin> AddFromOpenApiWithMetadataAsync(this Kernel kernel, string pluginName, string filePath, OpenApiFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenApiAsync(pluginName, filePath, executionParameters, cancellationToken);

        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    public async static Task<KernelPlugin> AddFromOpenApiWithMetadataAsync(this Kernel kernel, string pluginName, Uri uri, OpenApiFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenApiAsync(pluginName, uri, executionParameters, cancellationToken);

        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    public async static Task<KernelPlugin> AddFromOpenApiWithMetadataAsync(this Kernel kernel, string pluginName, Stream stream, OpenApiFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenApiAsync(pluginName, stream, executionParameters, cancellationToken);

        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    public async static Task<KernelPlugin> AddFromOpenAIWithMetadataAsync(this Kernel kernel, string pluginName, string filePath, OpenAIFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenAIAsync(pluginName, filePath, executionParameters, cancellationToken);

        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    public async static Task<KernelPlugin> AddFromOpenAIWithMetadataAsync(this Kernel kernel, string pluginName, Uri uri, OpenAIFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenAIAsync(pluginName, uri, executionParameters, cancellationToken);

        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    public async static Task<KernelPlugin> AddFromOpenAIWithMetadataAsync(this Kernel kernel, string pluginName, Stream stream, OpenAIFunctionExecutionParameters? executionParameters = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var kernelPlugin = await kernel.CreatePluginFromOpenAIAsync(pluginName, stream, executionParameters, cancellationToken);

        return kernel.AddPatchedKernelPluginWithMetadata(kernelPlugin);
    }

    public static KernelPlugin AddPatchedKernelPluginWithMetadata(this Kernel kernel, KernelPlugin kernelPlugin)
    {
        var patchedPlugin = kernel.Services.PatchKernelPluginWithMetadata(kernelPlugin);

        kernel.Plugins.Add(patchedPlugin);

        return patchedPlugin;
    }
}
