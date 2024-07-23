using FluentAssertions;
using Microsoft.SemanticKernel;
using Moq;

using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests
{
    public class PluginBuilderTests
    {
        [Fact]
        public void PatchKernelPluginWithMetadata_ShouldReturnUnalteredPlugin_WhenNoFunctionMetadataExists()
        {
            // Arrange
            var metadataProviderMock = new Mock<IPluginMetadataProvider>();
            metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns(null as PluginMetadata);
            metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.IsAny<KernelFunctionMetadata>()))
                .Returns(null as FunctionMetadata);

            var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
            var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

            // Act
            var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);

            // Assert
            result.Should().BeSameAs(kernelPlugin);
            result.Description.Should().Be("Sample plugin for testing");
            result.GetFunctionsMetadata().First().Description.Should().Be("Returns the current username");
        }

        [Fact]
        public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenPluginDescriptionExists()
        {
            // Arrange
            var metadataProviderMock = new Mock<IPluginMetadataProvider>();
            metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });

            var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
            var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

            // Act
            var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);

            // Assert
            result.Should().NotBeSameAs(kernelPlugin);
            result.Description.Should().Be("Plugin description altered using patch mechanism.");
            result.GetFunctionsMetadata().First().Description.Should().Be("Returns the current username");
        }

        [Fact]
        public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenFunctionMetadataExists()
        {
            // Arrange
            var metadataProviderMock = new Mock<IPluginMetadataProvider>();
            metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.IsAny<KernelFunctionMetadata>()))
                .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Description = "Function description altered using patch mechanism." });

            var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
            var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

            // Act
            var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);

            // Assert
            result.Should().NotBeSameAs(kernelPlugin);
            result.Description.Should().Be("Sample plugin for testing");
            result.GetFunctionsMetadata().First().Description.Should().Be("Function description altered using patch mechanism.");
        }

        [Fact]
        public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenPluginDescriptionAndFunctionMetadataExists()
        {
            // Arrange
            var metadataProviderMock = new Mock<IPluginMetadataProvider>();
            metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
            metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.IsAny<KernelFunctionMetadata>()))
                .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Description = "Function description altered using patch mechanism." });

            var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
            var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

            // Act
            var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);

            // Assert
            result.Should().NotBeSameAs(kernelPlugin);
            result.Description.Should().Be("Plugin description altered using patch mechanism.");
            result.GetFunctionsMetadata().First().Description.Should().Be("Function description altered using patch mechanism.");
        }
    }
}