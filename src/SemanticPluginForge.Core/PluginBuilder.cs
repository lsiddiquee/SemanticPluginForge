using Microsoft.SemanticKernel;

namespace SemanticPluginForge.Core;

/// <summary>
/// Class for building a plugin with metadata.
/// </summary>
/// <param name="metadataProvider">The metadata provider to retrieve external metadata to use to update the plugin.</param>
public class PluginBuilder(IPluginMetadataProvider metadataProvider) : IPluginBuilder
{
    private readonly IPluginMetadataProvider _metadataProvider = metadataProvider;

    /// <inheritdoc/>>
    public KernelPlugin PatchKernelPluginWithMetadata(KernelPlugin plugin)
    {
        bool pluginAltered = false;

        var functions = new List<KernelFunction>();
        foreach (var function in plugin)
        {
            var functionMeta = _metadataProvider.GetFunctionMetadata(plugin, function.Metadata);
            if (functionMeta != null)
            {
                pluginAltered = true;

                var method = (Kernel kernel, KernelFunction currentFunction, KernelArguments arguments, CancellationToken cancellationToken) =>
                {
                    return function.InvokeAsync(kernel, arguments, cancellationToken);
                };

                var options = new KernelFunctionFromMethodOptions
                {
                    FunctionName = function.Name,
                    Description = functionMeta.Description,
                    Parameters = functionMeta.Parameters,
                    ReturnParameter = functionMeta.ReturnParameter
                };

                functions.Add(KernelFunctionFactory.CreateFromMethod(method, options));
            }
            else
            {
                functions.Add(function);
            }
        }

        var newDescription = _metadataProvider.GetPluginDescription(plugin);

        if (pluginAltered || (newDescription != null && newDescription != plugin.Description))
        {
            return KernelPluginFactory.CreateFromFunctions(plugin.Name, newDescription ?? plugin.Description, functions);
        }

        return plugin;
    }
}
