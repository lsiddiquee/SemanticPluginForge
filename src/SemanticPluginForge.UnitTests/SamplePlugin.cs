using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticPluginForge.UnitTests;

[Description("Sample plugin for testing")]
public class SamplePlugin
{
    [KernelFunction, Description("Returns the current username")]
    public string GetUsername() => "my_user";
}
