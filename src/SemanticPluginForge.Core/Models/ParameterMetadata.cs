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
    /// Gets if the parameter should be suppressed from the function.
    /// </summary>
    /// <remarks>
    /// If the parameter is suppressed, but <see cref="DefaultValue"/> is set, the parameter won't be visible
    /// to LLM to resolve the value, but, the default value will be used.
    /// </remarks>
    public bool Suppress { get; init; } = false;

    /// <summary>
    /// Gets the default value of the parameter.
    /// </summary>
    /// <remarks>
    /// If it is set and <see cref="Suppress"/> is set to <see langword="true"/>, the parameter won't be visible
    /// to LLM to resolve the value, but, the default value will be used.
    /// </remarks>
    public object? DefaultValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether the parameter is required.
    /// </summary>
    public bool? IsRequired { get; init; }
}
