using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests
{
    public partial class KernelPluginForgeTests
    {
        private class StubPluginWithoutAttribute
        {
            public string ToShortDateString()
            {
                return DateTime.Now.ToShortDateString();
            }
        }

        private class StubPluginMetadataProvider : IPluginMetadataProvider
        {
            public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
                plugin.Name == "DateTimePlugin" ? new PluginMetadata
                {
                    Description = "This plugin returns date and time information."
                } : null;

            public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
                plugin.Name == "DateTimePlugin" && metadata.Name == "ToShortDateString" ? new FunctionMetadata(metadata.Name)
                {
                    Description = "Returns the date in short format."
                } : null;
        }
    }
}