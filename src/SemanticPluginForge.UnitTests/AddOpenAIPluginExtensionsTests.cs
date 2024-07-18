#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using Moq;

namespace SemanticPluginForge.UnitTests
{
    public class AddOpenAIPluginExtensionsTests
    {
        [Fact]
        public async Task AddFromOpenApiWithMetadataAsync_ShouldAddPluginFromFilePath()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var mockMetadataProvider = new Mock<IPluginMetadataProvider>();
            mockMetadataProvider.Setup(metadataProvider => metadataProvider.GetPluginDescription(It.IsAny<KernelPlugin>())).Returns("Plugin description from metadata provider.");
            serviceCollection.AddScoped<IPluginMetadataProvider>(sp => mockMetadataProvider.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var pluginName = "SamplePluginFromOpenApi";
            var filePath = "sample_open_api_spec.json";

            // Act
            var result = await serviceProvider.AddFromOpenApiWithMetadataAsync(pluginName, filePath);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(pluginName);
            result.Description.Should().Be("Plugin description from metadata provider.");
        }

        [Fact]
        public async Task AddFromOpenApiWithMetadataAsync_ShouldAddPluginFromUri()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var mockMetadataProvider = new Mock<IPluginMetadataProvider>();
            mockMetadataProvider.Setup(metadataProvider => metadataProvider.GetPluginDescription(It.IsAny<KernelPlugin>())).Returns("Plugin description from metadata provider.");
            serviceCollection.AddScoped<IPluginMetadataProvider>(sp => mockMetadataProvider.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var uri = new Uri("https://raw.githubusercontent.com/lsiddiquee/SemanticPluginForge/main/src/SemanticPluginForge.UnitTests/sample_open_api_spec.json");
            var pluginName = "SamplePluginFromOpenApi";

            // Act
            var result = await serviceProvider.AddFromOpenApiWithMetadataAsync(pluginName, uri);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(pluginName);
            result.Description.Should().Be("Plugin description from metadata provider.");
        }

        [Fact]
        public async Task AddFromOpenApiWithMetadataAsync_ShouldAddPluginFromStream()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var mockMetadataProvider = new Mock<IPluginMetadataProvider>();
            mockMetadataProvider.Setup(metadataProvider => metadataProvider.GetPluginDescription(It.IsAny<KernelPlugin>())).Returns("Plugin description from metadata provider.");
            serviceCollection.AddScoped<IPluginMetadataProvider>(sp => mockMetadataProvider.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var pluginName = "SamplePluginFromOpenApi";
            var stream = File.OpenRead("sample_open_api_spec.json");

            // Act
            var result = await serviceProvider.AddFromOpenApiWithMetadataAsync(pluginName, stream);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(pluginName);
            result.Description.Should().Be("Plugin description from metadata provider.");
        }
    }
}