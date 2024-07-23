namespace SemanticPluginForge.Core;

/// <summary>
/// This model contains necessary information to make changes to the KernelPlugin.
/// </summary>
public class PluginMetadata
{
    /// <summary>
    /// Gets the description of the plugin, suitable for use in describing the purpose to a model.
    /// </summary>
    public string? Description { get; init; }
}
