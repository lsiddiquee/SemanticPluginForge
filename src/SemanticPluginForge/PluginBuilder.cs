using Microsoft.SemanticKernel;

namespace SemanticPluginForge;

public class PluginBuilder(IPluginMetadataProvider metadataProvider) : IPluginBuilder
{
    private readonly IPluginMetadataProvider _metadataProvider = metadataProvider;

    public KernelPlugin PatchKernelPluginWithMetadata(KernelPlugin kernelPlugin)
    {
        bool pluginAltered = false;

        var functions = new List<KernelFunction>();
        foreach (var function in kernelPlugin)
        {
            var functionMeta = _metadataProvider.GetFunctionMetadata(kernelPlugin, function.Metadata);
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

        string description = _metadataProvider.GetPluginDescription(kernelPlugin);

        if (pluginAltered || description != kernelPlugin.Description)
        {
            return KernelPluginFactory.CreateFromFunctions(kernelPlugin.Name, description, functions);
        }

        return kernelPlugin;
    }
}
