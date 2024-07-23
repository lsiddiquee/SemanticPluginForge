namespace SemanticPluginForge.Core;

/// <summary>
/// Metadata for a parameter of a function.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ParameterMetadata"/> class with the name of the parameter.
/// </remarks>
/// <param name="name">The name of the parameter.</param>
public class ParameterMetadata(string name) : ParameterMetadataBase
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Gets the default value of the parameter.
    /// </summary>
    public object? DefaultValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether the parameter is required.
    /// </summary>
    public bool? IsRequired { get; init; }
}
