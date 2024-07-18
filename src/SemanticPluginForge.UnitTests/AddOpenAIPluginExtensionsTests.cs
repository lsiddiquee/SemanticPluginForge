#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Moq;

namespace SemanticPluginForge.UnitTests
{
    public class AddOpenAIPluginExtensionsTests
    {
        [Fact]
        public async Task AddFromOpenApiWithMetadataAsync_ShouldAddPluginFromFilePath()
        {
            // Arrange
            var kernel = BuildKernel();
            var pluginName = "SamplePluginFromOpenApi";
            var filePath = "sample_open_api_spec.json";

            // Act
            var result = await kernel.AddFromOpenApiWithMetadataAsync(pluginName, filePath);

            // Assert
            AssertPluginPatchedAndAdded(result, pluginName, kernel);
        }

        [Fact]
        public async Task AddFromOpenApiWithMetadataAsync_ShouldAddPluginFromUri()
        {
            // Arrange
            var kernel = BuildKernel();
            var uri = new Uri("https://raw.githubusercontent.com/lsiddiquee/SemanticPluginForge/main/src/SemanticPluginForge.UnitTests/sample_open_api_spec.json");
            var pluginName = "SamplePluginFromOpenApi";

            // Act
            var result = await kernel.AddFromOpenApiWithMetadataAsync(pluginName, uri);

            // Assert
            AssertPluginPatchedAndAdded(result, pluginName, kernel);
        }

        [Fact]
        public async Task AddFromOpenApiWithMetadataAsync_ShouldAddPluginFromStream()
        {
            // Arrange
            var kernel = BuildKernel();
            var pluginName = "SamplePluginFromOpenApi";
            var stream = File.OpenRead("sample_open_api_spec.json");

            // Act
            var result = await kernel.AddFromOpenApiWithMetadataAsync(pluginName, stream);

            // Assert
            AssertPluginPatchedAndAdded(result, pluginName, kernel);
        }

        [Fact]
        public async Task AddFromOpenAIWithMetadataAsync_ShouldAddPluginFromFilePath()
        {
            // Arrange
            var kernel = BuildKernel();
            var pluginName = "SamplePluginFromOpenApi";
            var filePath = "sample_open_ai_manifest.json";

            // Act
            var result = await kernel.AddFromOpenAIWithMetadataAsync(pluginName, filePath);

            // Assert
            AssertPluginPatchedAndAdded(result, pluginName, kernel);
        }

        [Fact]
        public async Task AddFromOpenAIWithMetadataAsync_ShouldAddPluginFromUri()
        {
            // Arrange
            var kernel = BuildKernel();
            var uri = new Uri("https://raw.githubusercontent.com/lsiddiquee/SemanticPluginForge/main/src/SemanticPluginForge.UnitTests/sample_open_ai_manifest.json");
            var pluginName = "SamplePluginFromOpenApi";

            // Act
            var result = await kernel.AddFromOpenAIWithMetadataAsync(pluginName, uri);

            // Assert
            AssertPluginPatchedAndAdded(result, pluginName, kernel);
        }

        [Fact]
        public async Task AddFromOpenAIWithMetadataAsync_ShouldAddPluginFromStream()
        {
            // Arrange
            var kernel = BuildKernel();
            var pluginName = "SamplePluginFromOpenApi";
            var stream = File.OpenRead("sample_open_ai_manifest.json");

            // Act
            var result = await kernel.AddFromOpenAIWithMetadataAsync(pluginName, stream);

            // Assert
            AssertPluginPatchedAndAdded(result, pluginName, kernel);
        }

        private void AssertPluginPatchedAndAdded(KernelPlugin? kernelPlugin, string pluginName, Kernel kernel)
        {
            kernelPlugin.Should().NotBeNull();
            kernelPlugin!.Name.Should().Be(pluginName);
            kernelPlugin.Description.Should().Be("Plugin description from metadata provider.");

            kernel.Plugins.Should().HaveCount(1);
            kernel.Plugins.First().Should().BeSameAs(kernelPlugin);
        }

        private static Kernel BuildKernel()
        {
            var serviceCollection = new ServiceCollection();
            var mockMetadataProvider = new Mock<IPluginMetadataProvider>();
            mockMetadataProvider.Setup(metadataProvider => metadataProvider.GetPluginDescription(It.IsAny<KernelPlugin>())).Returns("Plugin description from metadata provider.");
            serviceCollection.AddScoped<IPluginMetadataProvider>(sp => mockMetadataProvider.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return new Kernel(serviceProvider);
        }
    }
}