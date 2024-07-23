namespace SemanticPluginForge.Core;

/// <summary>
/// Base class for parameter metadata, containing common properties.
/// </summary>
public abstract class ParameterMetadataBase
{
    /// <summary>
    /// Gets a description of the parameter, suitable for use in describing the purpose to a model.
    /// </summary>
    public string? Description { get; init; }
}
