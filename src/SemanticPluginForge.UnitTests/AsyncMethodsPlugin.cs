using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests
{
    public partial class KernelPluginForgeTests
    {
        private class AsyncMethodsPlugin
        {
            // Normal method
            public string GetData() 
            {
                return "Some data";
            }
            
            // Normal method with CancellationToken
            public string GetData(CancellationToken cancellationToken = default) 
            {
                return "Some data";
            }
            
            // Async version of the same method
            public Task<string> GetDataAsync() 
            {
                return Task.FromResult("Some data");
            }
            
            // Async version of the same method with CancellationToken
            public Task<string> GetDataAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult("Some data");
            }
        }

        private class AsyncMethodsMetadataProvider : IPluginMetadataProvider
        {
            public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
                plugin.Name == "AsyncMethodsPlugin" ? new PluginMetadata
                {
                    Description = "This plugin tests async method filtering."
                } : null;

            public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
            {
                if (plugin.Name == "AsyncMethodsPlugin" && metadata.Name == "GetData")
                {
                    return new FunctionMetadata(metadata.Name)
                    {
                        Description = "Function GetData for testing async filtering."
                    };
                }
                
                return null;
            }
        }
    }
}