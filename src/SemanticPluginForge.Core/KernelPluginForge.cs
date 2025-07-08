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

        var metadataProvider = serviceProvider.GetRequiredService<IPluginMetadataProvider>();

        var temporaryPlugin = KernelPluginFactory.CreateFromFunctions(pluginName);
        var pluginMetadata = metadataProvider.GetPluginMetadata(temporaryPlugin);
        if (pluginMetadata is null)
        {
            throw new ArgumentException($"The plugin with name '{pluginName}' doesn't have any metadata defined.");
        }

        var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).ToList();

        // Filter out methods with open generic parameters.
        FilterMethodsWithOpenGeneric(methods, logger);
        // Filter out methods with TAP.
        FilterMethodsWithAsyncOverloads(methods, logger);

        // Include only functions that have the metadata defined.
        var functions = new List<KernelFunction>();
        var pluginBuilder = new PluginBuilder(metadataProvider);
        foreach (MethodInfo method in methods)
        {
            var kernelFunction = KernelFunctionFactory.CreateFromMethod(method, target, loggerFactory: loggerFactory);
            var patchedFunction = pluginBuilder.PatchKernelFunctionWithMetadata(temporaryPlugin, kernelFunction);
            if (patchedFunction is not null && patchedFunction != kernelFunction)
            {
                functions.Add(patchedFunction);
            }
        }
        if (functions.Count == 0)
        {
            throw new ArgumentException($"The plugin '{pluginName}' with type {target.GetType()} doesn't have any function metadata defined, or was no function was able to be resolved.");
        }

        if (logger is not null && logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Created plugin {PluginName} with {IncludedFunctions} [KernelFunction] methods out of {TotalMethods} methods found.", pluginName, functions.Count, methods.Count);
        }

        return KernelPluginFactory.CreateFromFunctions(pluginName, pluginMetadata?.Description, functions);
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

    /// <summary>
    /// Filters out methods with open generic parameters.
    /// </summary>
    /// <param name="methods">The list of methods to filter.</param>
    /// <param name="logger">The logger.</param>
    private static void FilterMethodsWithOpenGeneric(List<MethodInfo> methods, ILogger? logger)
    {
        // First, filter out methods with open generic parameters
        var methodsToRemove = methods.Where(m => m.ContainsGenericParameters || m.IsGenericMethodDefinition).ToList();
        foreach (var method in methodsToRemove)
        {
            logger?.LogInformation("Suppressing generic method {MethodName} with generic parameters", method.Name);
            methods.Remove(method);
        }
    }

    /// <summary>
    /// Filters out methods that have overloads with only difference being a CancellationToken parameter and/or Async suffix.
    /// </summary>
    /// <param name="methods">The list of methods to filter.</param>
    /// <param name="logger">The logger.</param>
    private static void FilterMethodsWithAsyncOverloads(List<MethodInfo> methods, ILogger? logger)
    {
        // Create a normalized list of methods where we remove the CancellationToken parameter and Async suffix.
        var methodMetaList = methods.Select(m => new MethodMeta(m)).ToArray();

        for (int i = 0; i < methodMetaList.Length; i++)
        {
            for (int j = i + 1; j < methodMetaList.Length; j++)
            {
                if (methodMetaList[i].Name == methodMetaList[j].Name &&
                    methodMetaList[i].Parameters.Length == methodMetaList[j].Parameters.Length)
                {
                    int k = 0;
                    for (; k < methodMetaList[i].Parameters.Length; k++)
                    {
                        if (methodMetaList[i].Parameters[k].Name != methodMetaList[j].Parameters[k].Name ||
                            methodMetaList[i].Parameters[k].ParameterType != methodMetaList[j].Parameters[k].ParameterType)
                        {
                            // If the parameters are not the same, we can skip this pair.
                            break;
                        }
                    }

                    if (k == methodMetaList[i].Parameters.Length)
                    {
                        // If we reached here, it means the methods are identical except for the async suffix and/or cancellation token parameter.
                        // Order of choice, cancellation token parameter then Async suffix.
                        int removeIndex = j;
                        if (methodMetaList[i].HasCancellationTokenParameter != methodMetaList[j].HasCancellationTokenParameter)
                        {
                            removeIndex = methodMetaList[i].HasCancellationTokenParameter ? j : i;
                        }
                        else if (methodMetaList[i].HasAsyncSuffix != methodMetaList[j].HasAsyncSuffix)
                        {
                            removeIndex = methodMetaList[i].HasAsyncSuffix ? j : i;
                        }

                        logger?.LogInformation("Suppressing method {MethodName} with parameters: {Parameters}",
                            methodMetaList[removeIndex].MethodInfo.Name,
                            string.Join(", ", methodMetaList[removeIndex].MethodInfo.GetParameters().Select(p => p.Name)));
                        methods.Remove(methodMetaList[removeIndex].MethodInfo);
                    }
                }
            }
        }
    }

    internal class MethodMeta
    {
        internal string Name { get; private set; }

        internal ParameterInfo[] Parameters { get; private set; }

        internal bool HasAsyncSuffix { get; private set; }

        internal bool HasCancellationTokenParameter { get; private set; }

        internal MethodInfo MethodInfo { get; private set; }

        internal MethodMeta(MethodInfo method)
        {
            HasAsyncSuffix = method.Name.EndsWith("Async");
            Name = HasAsyncSuffix ? method.Name.Substring(0, method.Name.Length - "Async".Length) : method.Name;
            Parameters = [.. method.GetParameters().Where(p => p.ParameterType != typeof(CancellationToken))];
            HasCancellationTokenParameter = method.GetParameters().Any(p => p.ParameterType == typeof(CancellationToken));
            MethodInfo = method;
        }
    }
}