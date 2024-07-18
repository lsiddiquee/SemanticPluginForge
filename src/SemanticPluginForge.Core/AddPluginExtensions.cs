using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SemanticPluginForge;

public static class AddPluginExtensions
{
    public static IKernelBuilderPlugins AddFromObjectWithMetadata(this IKernelBuilderPlugins plugins, object target, string? pluginName = null)
    {
        plugins.Services.AddSingleton(sp => CreateFromObjectWithMetadata(target, sp, pluginName));

        return plugins;
    }

    public static KernelPlugin AddFromObjectWithMetadata(this ICollection<KernelPlugin> plugins, object target, IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = CreateFromObjectWithMetadata(target, serviceProvider, pluginName);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    private static KernelPlugin CreateFromObjectWithMetadata(object target, IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = KernelPluginFactory.CreateFromObject(target, pluginName, serviceProvider.GetService<ILoggerFactory>());

        return serviceProvider.PatchKernelPluginWithMetadata(kernelPlugin);
    }

    public static IKernelBuilderPlugins AddFromTypeWithMetadata<T>(this IKernelBuilderPlugins plugins, string? pluginName = null)
    {
        plugins.Services.AddSingleton(sp => CreateFromTypeWithMetadata<T>(sp, pluginName));

        return plugins;
    }

    public static KernelPlugin AddFromTypeWithMetadata<T>(this ICollection<KernelPlugin> plugins, IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = CreateFromTypeWithMetadata<T>(serviceProvider, pluginName);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    private static KernelPlugin CreateFromTypeWithMetadata<T>(IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = KernelPluginFactory.CreateFromType<T>(pluginName, serviceProvider);

        return serviceProvider.PatchKernelPluginWithMetadata(kernelPlugin);
    }

    public static IKernelBuilderPlugins AddFromFunctionsWithMetadata(this IKernelBuilderPlugins plugins, string pluginName, IEnumerable<KernelFunction>? functions)
    {
        plugins.Services.AddSingleton(sp => CreateFromFunctionsWithMetadata(sp, pluginName, functions));

        return plugins;
    }

    public static KernelPlugin AddFromFunctionsWithMetadata(this ICollection<KernelPlugin> plugins, IServiceProvider serviceProvider, string pluginName, IEnumerable<KernelFunction>? functions)
    {
        var kernelPlugin = CreateFromFunctionsWithMetadata(serviceProvider, pluginName, functions);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    private static KernelPlugin CreateFromFunctionsWithMetadata(IServiceProvider serviceProvider, string pluginName, IEnumerable<KernelFunction>? functions)
    {
        var kernelPlugin = KernelPluginFactory.CreateFromFunctions(pluginName, functions);

        return serviceProvider.PatchKernelPluginWithMetadata(kernelPlugin);
    }

    public static KernelPlugin PatchKernelPluginWithMetadata(this IServiceProvider serviceProvider, KernelPlugin kernelPlugin)
    {
        var metadataProvider = serviceProvider.GetRequiredService<IPluginMetadataProvider>();
        var builder = new PluginBuilder(metadataProvider);
        return builder.PatchKernelPluginWithMetadata(kernelPlugin);
    }
}
