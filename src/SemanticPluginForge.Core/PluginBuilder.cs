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

                if (functionMeta.Suppress)
                {
                    // This function should be suppressed from the plugin.
                    continue;
                }

                var method = BuildMethodInvocation(function, functionMeta);

                List<KernelParameterMetadata> parameters = BuildKernelParameters(function, functionMeta);

                var options = new KernelFunctionFromMethodOptions
                {
                    FunctionName = functionMeta.OverrideFunctionName ?? functionMeta.Name,
                    Description = functionMeta.Description ?? function.Metadata.Description,
                    Parameters = parameters,
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

    private static List<KernelParameterMetadata> BuildKernelParameters(KernelFunction function, FunctionMetadata functionMeta)
    {
        var parameters = new List<KernelParameterMetadata>();
        foreach (var param in function.Metadata.Parameters)
        {
            var paramMeta = functionMeta.Parameters?.FirstOrDefault(p => p.Name == param.Name);
            if (paramMeta != null)
            {
                if (paramMeta.Suppress)
                {
                    if (param.IsRequired && paramMeta.DefaultValue == null && param.DefaultValue == null)
                    {
                        // This parameter is required and cannot be suppressed without a default value.
                        // Throw an exception to indicate that the parameter cannot be suppressed.
                        // This is to ensure that the plugin does not break when the function is called.
                        // The user should be aware of this and handle it accordingly.
                        throw new ArgumentException($"Parameter '{param.Name}' is required and cannot be suppressed without a default value.");
                    }

                    // This parameter should be suppressed from the function.
                    continue;
                }

                parameters.Add(new KernelParameterMetadata(param)
                {
                    Description = paramMeta.Description ?? param.Description,
                    IsRequired = paramMeta.IsRequired ?? param.IsRequired,
                    DefaultValue = paramMeta.DefaultValue ?? param.DefaultValue
                });
            }
            else
            {
                parameters.Add(param);
            }
        }

        return parameters;
    }

    private static Func<Kernel, KernelFunction, KernelArguments, CancellationToken, Task<FunctionResult>> BuildMethodInvocation(KernelFunction function, FunctionMetadata functionMeta)
    {
        return (Kernel kernel, KernelFunction currentFunction, KernelArguments arguments, CancellationToken cancellationToken) =>
        {
            foreach (var param in functionMeta.Parameters ?? Enumerable.Empty<ParameterMetadata>())
            {
                var kernelParameterMetadata = function.Metadata.Parameters.FirstOrDefault(p => p.Name == param.Name);
                if (param.Suppress)
                {
                    // Setting suppressed parameters to their default values if available. The default value from the metadata provider has precedence.
                    arguments[param.Name] = param.DefaultValue ?? kernelParameterMetadata?.DefaultValue;
                }
            }

            return function.InvokeAsync(kernel, arguments, cancellationToken);
        };
    }
}
