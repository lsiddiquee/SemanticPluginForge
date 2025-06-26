using System.Collections.ObjectModel;

namespace SemanticPluginForge.Core;

/// <summary>
/// This model contains all necessary information to make changes to the KernelFunction.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FunctionMetadata"/> class with the name of the function.
/// </remarks>
/// <param name="name">The name of the function.</param>
public class FunctionMetadata(string name)
{
    /// <summary>
    /// Gets the name of the function.
    /// This name should match the name of the function in the plugin.
    /// It is used to identify the function in the plugin and to call it.
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// Gets the name of the function to override in the plugin.
    /// This is used to specify a different name for the function in the plugin than the one
    /// defined in the actual plugin.
    /// </summary>
    public string? OverrideFunctionName { get; init; }

    /// <summary>
    /// Gets if the function should be suppressed from the plugin.
    /// </summary>
    public bool Suppress { get; init; } = false;

    /// <summary>
    /// Gets the description of the function, suitable for use in describing the purpose to a model.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the parameters of the function.
    /// </summary>
    public IReadOnlyList<ParameterMetadata>? Parameters { get; init; }

    /// <summary>
    /// Gets the return parameter of the function.
    /// </summary>
    public ReturnParameterMetadata? ReturnParameter { get; init; }

    /// <summary>
    /// Gets any additional properties associated with the function.
    /// </summary>
    public ReadOnlyDictionary<string, object?>? AdditionalProperties { get; init; }
}
