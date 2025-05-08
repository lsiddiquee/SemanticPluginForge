using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;
using System.Reflection;

namespace SemanticPluginForge;

/// <summary>
/// Factory class for creating plugins with metadata.
/// </summary>
public static class KernelPluginForge
{
    /// <summary>
    /// Creates a plugin that wraps the specified target object and updates any metadata
    /// that it can find using IPluginMetadataProvider.
    /// </summary>
    /// <param name="target">The instance of the class to be wrapped.</param>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection and prompt templates. If the value 
    /// is null, a plugin name is derived from the type of the target.</param>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static KernelPlugin CreateFromObjectWithMetadata(object target, IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = KernelPluginFactory.CreateFromObject(target, pluginName, serviceProvider.GetService<ILoggerFactory>());

        return serviceProvider.PatchKernelPluginWithMetadata(kernelPlugin);
    }

    /// <summary>
    /// Creates a plugin that wraps the specified target object and updates any metadata
    /// that it can find using IPluginMetadataProvider.
    /// </summary>
    /// <param name="target">The instance of the class to be wrapped.</param>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection.</param>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the metadata defined using IPluginMetadataProvider will be included in the plugin.
    /// </remarks>
    public static KernelPlugin CreateFromClrObjectWithMetadata(object target, IServiceProvider serviceProvider, string pluginName)
    {
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger(target.GetType());

        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        var metadataProvider = serviceProvider.GetRequiredService<IPluginMetadataProvider>();

        var temporaryPlugin = KernelPluginFactory.CreateFromFunctions(pluginName);
        var pluginMetadata = metadataProvider.GetPluginMetadata(temporaryPlugin);
        if (pluginMetadata is null)
        {
            throw new ArgumentException($"The plugin with name '{pluginName}' doesn't have any metadata defined.");
        }

        // Include only functions that have the metadata defined.
        var functions = new List<KernelFunction>();
        foreach (MethodInfo method in methods)
        {
            // TODO: Deal with overloads
            var kernelFunction = KernelFunctionFactory.CreateFromMethod(method, target, loggerFactory: loggerFactory);
            var functionMetadata = metadataProvider.GetFunctionMetadata(temporaryPlugin, kernelFunction.Metadata);
            if (functionMetadata is not null)
            {
                functions.Add(kernelFunction);
            }
        }
        if (functions.Count == 0)
        {
            throw new ArgumentException($"The plugin '{pluginName}' with type {target.GetType()} doesn't have any function metadata defined, or was no function was able to be resolved.");
        }

        if (logger is not null && logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Created plugin {PluginName} with {IncludedFunctions} [KernelFunction] methods out of {TotalMethods} methods found.", pluginName, functions.Count, methods.Length);
        }

        var kernelPlugin = KernelPluginFactory.CreateFromFunctions(pluginName, pluginMetadata?.Description, functions);
        return PatchKernelPluginWithMetadata(kernelPlugin, metadataProvider);
    }

    /// <summary>
    /// Creates a plugin that wraps a new instance of the specified type T and updates any metadata
    /// that it can find using IPluginMetadataProvider.
    /// </summary>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection. If the value 
    /// is null, a plugin name is derived from the type of T.</param>
    /// <typeparam name="T">Specifies the type of the object to wrap.</typeparam>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static KernelPlugin CreateFromTypeWithMetadata<T>(IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = KernelPluginFactory.CreateFromType<T>(pluginName, serviceProvider);

        return serviceProvider.PatchKernelPluginWithMetadata(kernelPlugin);
    }

    /// <summary>
    /// Creates a plugin that wraps a new instance of the specified type T and updates any metadata
    /// that it can find using IPluginMetadataProvider.
    /// </summary>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection.</param>
    /// <typeparam name="T">Specifies the type of the object to wrap.</typeparam>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the metadata defined using IPluginMetadataProvider will be included in the plugin.
    /// </remarks>
    public static KernelPlugin CreateFromClrTypeWithMetadata<T>(IServiceProvider serviceProvider, string pluginName)
    {
        return CreateFromClrObjectWithMetadata(ActivatorUtilities.CreateInstance<T>(serviceProvider)!, serviceProvider, pluginName);
    }

    /// <summary>
    /// Creates a plugin that wraps the specified functions and updates any metadata
    /// that it can find using IPluginMetadataProvider.
    /// </summary>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection and prompt templates. If the value 
    /// is null, a plugin name is derived from the type of the target.</param>
    /// <param name="functions">The initial functions to be available as part of the plugin.</param>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static KernelPlugin CreateFromFunctionsWithMetadata(IServiceProvider serviceProvider, string pluginName, IEnumerable<KernelFunction>? functions)
    {
        var kernelPlugin = KernelPluginFactory.CreateFromFunctions(pluginName, functions);

        return serviceProvider.PatchKernelPluginWithMetadata(kernelPlugin);
    }

    /// <summary>
    /// Using the IPluginMetadataProvider if there are any updates to the plugin, creates a new plugin or returns
    /// the same instance.
    /// </summary>
    /// <param name="plugin">A Microsoft.SemanticKernel.KernelPlugin containing the original functions wrapped.</param>
    /// <param name="metadataProvider">Implementation of IPluginMetadataProvider which can be used to update the plugin.</param>
     /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
   public static KernelPlugin PatchKernelPluginWithMetadata(KernelPlugin plugin, IPluginMetadataProvider metadataProvider)
    {
        var builder = new PluginBuilder(metadataProvider);
        return builder.PatchKernelPluginWithMetadata(plugin);
    }
}