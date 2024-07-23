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
                    Description = functionMeta.Description ?? function.Metadata.Description,
                    Parameters = function.Metadata.Parameters.Select(p =>
                    {
                        var paramMeta = functionMeta.Parameters?.FirstOrDefault(paramMeta => paramMeta.Name == p.Name);
                        return paramMeta == null ? p : new KernelParameterMetadata(p)
                        {
                            Description = paramMeta.Description ?? p.Description,
                            IsRequired = paramMeta.IsRequired ?? p.IsRequired,
                            DefaultValue = paramMeta.DefaultValue ?? p.DefaultValue
                        };
                    }),
                    ReturnParameter = functionMeta.ReturnParameter == null ? function.Metadata.ReturnParameter : new KernelReturnParameterMetadata(function.Metadata.ReturnParameter)
                    {
                        Description = functionMeta.ReturnParameter.Description ?? function.Metadata.Description,
                    }
                };

                functions.Add(KernelFunctionFactory.CreateFromMethod(method, options));
            }
            else
            {
                functions.Add(function);
            }
        }

        var pluginMetadata = _metadataProvider.GetPluginMetadata(plugin);

        if (pluginAltered || (pluginMetadata?.Description != null && pluginMetadata?.Description != plugin.Description))
        {
            return KernelPluginFactory.CreateFromFunctions(plugin.Name, pluginMetadata?.Description ?? plugin.Description, functions);
        }

        return plugin;
    }
}
