using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests
{
    public partial class KernelPluginForgeTests
    {
        private class GenericMethodsPlugin<T>
        {
            // Closed generic method
            public T? ClosedGenericMethod()
            {
                return default;
            }

            // Open generic method with one type parameter
            public string OpenGenericMethodWithParameter<TParam>(TParam param)
            {
                return $"Parameter type: {typeof(TParam).Name}, Value: {param}";
            }

            // Open generic method with multiple type parameters
            public string OpenGenericMethodWithMultipleParameters<TParam1, TParam2>(TParam1 param1, TParam2 param2)
            {
                return $"Parameter types: {typeof(TParam1).Name}, {typeof(TParam2).Name}, Values: {param1}, {param2}";
            }

            // Open generic method with generic return type
            public TReturn? OpenGenericMethodWithReturnType<TReturn>()
            {
                return default;
            }
            
            // Open generic method with generic return type and parameters
            public TReturn? OpenGenericMethodWithReturnTypeAndParameters<TParam, TReturn>(TParam param)
            {
                return default;
            }
        }

        private class GenericMethodsPluginMetadataProvider : IPluginMetadataProvider
        {
            public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
                plugin.Name == "GenericMethodsPlugin" ? new PluginMetadata
                {
                    Description = "This plugin tests generic method filtering."
                } : null;

            public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
            {
                if (plugin.Name != "GenericMethodsPlugin")
                {
                    return null;
                }

                return metadata.Name switch
                {
                    "ClosedGenericMethod" => new FunctionMetadata(metadata.Name)
                    {
                        Description = "Closed generic method that returns a default value of type T."
                    },
                    "OpenGenericMethodWithParameter" => new FunctionMetadata(metadata.Name)
                    {
                        Description = "Open generic method with one type parameter."
                    },
                    "OpenGenericMethodWithMultipleParameters" => new FunctionMetadata(metadata.Name)
                    {
                        Description = "Open generic method with multiple type parameters."
                    },
                    "OpenGenericMethodWithReturnType" => new FunctionMetadata(metadata.Name)
                    {
                        Description = "Open generic method with a generic return type."
                    },
                    "OpenGenericMethodWithReturnTypeAndParameters" => new FunctionMetadata(metadata.Name)
                    {
                        Description = "Open generic method with a generic return type and parameters."
                    },
                    _ => null,
                };
            }
        }
    }
}