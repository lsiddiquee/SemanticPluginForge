﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

namespace SemanticPluginForge;

/// <summary>
/// Extension methods for adding plugins to the plugin collection.
/// </summary>
public static class AddPluginExtensions
{
    /// <summary>
    /// Creates a plugin that wraps the specified target object and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="target">The instance of the class to be wrapped.</param>
    /// <param name="pluginName">Name of the plugin for function collection and prompt templates. If the value 
    /// is null, a plugin name is derived from the type of the target.</param>
    /// <returns>The same instance as plugins</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static IKernelBuilderPlugins AddFromObjectWithMetadata(this IKernelBuilderPlugins plugins, object target, string? pluginName = null)
    {
        plugins.Services.AddSingleton(sp => KernelPluginForge.CreateFromObjectWithMetadata(target, sp, pluginName));

        return plugins;
    }

    /// <summary>
    /// Creates a plugin that wraps the specified target object and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
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
    public static KernelPlugin AddFromObjectWithMetadata(this ICollection<KernelPlugin> plugins, object target, IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = KernelPluginForge.CreateFromObjectWithMetadata(target, serviceProvider, pluginName);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    /// <summary>
    /// Creates a plugin that wraps the specified target object and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="target">The instance of the class to be wrapped.</param>
    /// <param name="pluginName">Name of the plugin for function collection.</param>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the metadata defined using IPluginMetadataProvider will be included in the plugin.
    /// </remarks>
    public static IKernelBuilderPlugins AddFromClrObjectWithMetadata(this IKernelBuilderPlugins plugins, object target, string pluginName )
    {
        plugins.Services.AddSingleton(sp => KernelPluginForge.CreateFromClrObjectWithMetadata(target, sp, pluginName));

        return plugins;
    }

    /// <summary>
    /// Creates a plugin that wraps the specified target object and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="target">The instance of the class to be wrapped.</param>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection.</param>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the metadata defined using IPluginMetadataProvider will be included in the plugin.
    /// </remarks>
    public static KernelPlugin AddFromClrObjectWithMetadata(this ICollection<KernelPlugin> plugins, object target, IServiceProvider serviceProvider, string pluginName)
    {
        var kernelPlugin = KernelPluginForge.CreateFromClrObjectWithMetadata(target, serviceProvider, pluginName);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    /// <summary>
    /// Creates a plugin that wraps a new instance of the specified type T and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="pluginName">Name of the plugin for function collection and prompt templates. If the value 
    /// is null, a plugin name is derived from the type of T.</param>
    /// <typeparam name="T">Specifies the type of the object to wrap.</typeparam>
    /// <returns>The same instance as plugins</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static IKernelBuilderPlugins AddFromTypeWithMetadata<T>(this IKernelBuilderPlugins plugins, string? pluginName = null)
    {
        plugins.Services.AddSingleton(sp => KernelPluginForge.CreateFromTypeWithMetadata<T>(sp, pluginName));

        return plugins;
    }

    /// <summary>
    /// Creates a plugin that wraps a new instance of the specified type T and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection and prompt templates. If the value 
    /// is null, a plugin name is derived from the type of T.</param>
    /// <typeparam name="T">Specifies the type of the object to wrap.</typeparam>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static KernelPlugin AddFromTypeWithMetadata<T>(this ICollection<KernelPlugin> plugins, IServiceProvider serviceProvider, string? pluginName = null)
    {
        var kernelPlugin = KernelPluginForge.CreateFromTypeWithMetadata<T>(serviceProvider, pluginName);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    /// <summary>
    /// Creates a plugin that wraps a new instance of the specified type T and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="pluginName">Name of the plugin for function collection.</param>
    /// <typeparam name="T">Specifies the type of the object to wrap.</typeparam>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the metadata defined using IPluginMetadataProvider will be included in the plugin.
    /// </remarks>
    public static IKernelBuilderPlugins AddFromClrTypeWithMetadata<T>(this IKernelBuilderPlugins plugins, string pluginName)
    {
        plugins.Services.AddSingleton(sp => KernelPluginForge.CreateFromClrTypeWithMetadata<T>(sp, pluginName));

        return plugins;
    }

    /// <summary>
    /// Creates a plugin that wraps a new instance of the specified type T and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as Microsoft.Extensions.Logging.ILoggerFactory.</param>
    /// <param name="pluginName">Name of the plugin for function collection.</param>
    /// <typeparam name="T">Specifies the type of the object to wrap.</typeparam>
    /// <returns>A Microsoft.SemanticKernel.KernelPlugin containing Microsoft.SemanticKernel.KernelFunctions
    /// for all relevant members of target.</returns>
    /// <remarks>
    /// Methods that have the metadata defined using IPluginMetadataProvider will be included in the plugin.
    /// </remarks>
    public static KernelPlugin AddFromClrTypeWithMetadata<T>(this ICollection<KernelPlugin> plugins, IServiceProvider serviceProvider, string pluginName)
    {
        var kernelPlugin = KernelPluginForge.CreateFromClrTypeWithMetadata<T>(serviceProvider, pluginName);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    /// <summary>
    /// Creates a plugin that wraps the specified functions and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
    /// <param name="pluginName">Name of the plugin for function collection and prompt templates. If the value 
    /// is null, a plugin name is derived from the type of the target.</param>
    /// <param name="functions">The initial functions to be available as part of the plugin.</param>
    /// <returns>The same instance as plugins</returns>
    /// <remarks>
    /// Methods that have the Microsoft.SemanticKernel.KernelFunctionAttribute attribute
    /// will be included in the plugin. See Microsoft.SemanticKernel.KernelFunctionAttribute
    /// attribute for details.
    /// </remarks>
    public static IKernelBuilderPlugins AddFromFunctionsWithMetadata(this IKernelBuilderPlugins plugins, string pluginName, IEnumerable<KernelFunction>? functions)
    {
        plugins.Services.AddSingleton(sp => KernelPluginForge.CreateFromFunctionsWithMetadata(sp, pluginName, functions));

        return plugins;
    }

    /// <summary>
    /// Creates a plugin that wraps the specified functions and updates any metadata
    /// that it can find using IPluginMetadataProvider and adds it into the plugin collection.
    /// </summary>
    /// <param name="plugins">The plugin collection to which the new plugin should be added.</param>
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
    public static KernelPlugin AddFromFunctionsWithMetadata(this ICollection<KernelPlugin> plugins, IServiceProvider serviceProvider, string pluginName, IEnumerable<KernelFunction>? functions)
    {
        var kernelPlugin = KernelPluginForge.CreateFromFunctionsWithMetadata(serviceProvider, pluginName, functions);
        plugins.Add(kernelPlugin);

        return kernelPlugin;
    }

    /// <summary>
    /// Using the IPluginMetadataProvider if there are any updates to the plugin, creates a new plugin or returns
    /// the same instance.
    /// </summary>
    /// <param name="serviceProvider">Service provider from which to resolve dependencies,
    /// such as IPluginMetadataProvider.</param>
    /// <param name="plugin">A Microsoft.SemanticKernel.KernelPlugin containing the original functions wrapped.</param>
    /// <returns>A new instance of Microsoft.SemanticKernel.KernelPlugin if any change was needed in the plugin 
    /// otherwise the same instance is returned.</returns>
    public static KernelPlugin PatchKernelPluginWithMetadata(this IServiceProvider serviceProvider, KernelPlugin plugin)
    {
        var metadataProvider = serviceProvider.GetRequiredService<IPluginMetadataProvider>();
        return KernelPluginForge.PatchKernelPluginWithMetadata(plugin, metadataProvider);
    }
}
