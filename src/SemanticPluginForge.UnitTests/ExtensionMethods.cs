using FluentAssertions;
using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests;

public static class ExtensionMethods
{
    public static void FunctionsMetaShouldBe(this KernelPlugin plugin, List<FunctionMetadata> expectedFunctionsMetadata)
    {
        var functionsMetadata = plugin.GetFunctionsMetadata();
        functionsMetadata.Should().HaveCount(expectedFunctionsMetadata.Count);
        foreach (var expectedFunctionMetadata in expectedFunctionsMetadata)
        {
            var functionMetadata = functionsMetadata.Single(f => f.Name == expectedFunctionMetadata.Name);
            functionMetadata.Description.Should().Be(expectedFunctionMetadata.Description);
            functionMetadata.Parameters.Should().HaveCount(expectedFunctionMetadata.Parameters!.Count);
            foreach (var expectedParameterMetadata in expectedFunctionMetadata.Parameters)
            {
                var parameterMetadata = functionMetadata.Parameters.Single(p => p.Name == expectedParameterMetadata.Name);
                parameterMetadata.Description.Should().Be(expectedParameterMetadata.Description);
                parameterMetadata.IsRequired.Should().Be(expectedParameterMetadata.IsRequired!.Value);
                parameterMetadata.DefaultValue.Should().Be(expectedParameterMetadata.DefaultValue);
            }
            functionMetadata.ReturnParameter.Description.Should().Be(expectedFunctionMetadata.ReturnParameter?.Description);
        }
    }
}
