using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Moq;

namespace SemanticPluginForge.UnitTests
{
    public class AddPluginExtensionsTest
    {
        [Fact]
        public void AddFromObjectWithMetadata_ShouldAddSingletonService_AndReturnSamePluginsInstance()
        {
            // Arrange
            var plugins = new Mock<IKernelBuilderPlugins>();
            var serviceCollection = new ServiceCollection();
            plugins.SetupGet(p => p.Services).Returns(serviceCollection);
            var target = new SamplePlugin();

            // Act
            var result = plugins.Object.AddFromObjectWithMetadata(target);

            // Assert
            result.Should().BeSameAs(plugins.Object);
            serviceCollection.Should().HaveCount(1);
            serviceCollection[0].ServiceType.Should().Be(typeof(KernelPlugin));
        }

        [Fact]
        public void AddFromObjectWithMetadata_ShouldAddKernelPluginToCollection_AndReturnAddedPlugin()
        {
            // Arrange
            var plugins = new List<KernelPlugin>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IPluginMetadataProvider)))
                .Returns(new Mock<IPluginMetadataProvider>().Object);
            var target = new SamplePlugin();

            // Act
            var result = plugins.AddFromObjectWithMetadata(target, serviceProvider.Object);

            // Assert
            plugins.Should().HaveCount(1);
            result.Should().BeSameAs(plugins[0]);
        }

        [Fact]
        public void AddFromTypeWithMetadata_ShouldAddSingletonService_AndReturnSamePluginsInstance()
        {
            // Arrange
            var plugins = new Mock<IKernelBuilderPlugins>();
            var serviceCollection = new ServiceCollection();
            plugins.SetupGet(p => p.Services).Returns(serviceCollection);

            // Act
            var result = plugins.Object.AddFromTypeWithMetadata<SamplePlugin>();

            // Assert
            result.Should().BeSameAs(plugins.Object);
            serviceCollection.Should().HaveCount(1);
            serviceCollection[0].ServiceType.Should().Be(typeof(KernelPlugin));
        }

        [Fact]
        public void AddFromTypeWithMetadata_ShouldAddKernelPluginToCollection_AndReturnAddedPlugin()
        {
            // Arrange
            var plugins = new List<KernelPlugin>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IPluginMetadataProvider)))
                .Returns(new Mock<IPluginMetadataProvider>().Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(SamplePlugin)))
                .Returns(new SamplePlugin());

            // Act
            var result = plugins.AddFromTypeWithMetadata<SamplePlugin>(serviceProvider.Object);

            // Assert
            plugins.Should().HaveCount(1);
            result.Should().BeSameAs(plugins[0]);
        }

        [Fact]
        public void AddFromFunctionsWithMetadata_ShouldAddSingletonService_AndReturnSamePluginsInstance()
        {
            // Arrange
            var plugins = new Mock<IKernelBuilderPlugins>();
            var serviceCollection = new ServiceCollection();
            plugins.SetupGet(p => p.Services).Returns(serviceCollection);

            // Act
            var result = plugins.Object.AddFromFunctionsWithMetadata("FromFunctions", null);

            // Assert
            result.Should().BeSameAs(plugins.Object);
            serviceCollection.Should().HaveCount(1);
            serviceCollection[0].ServiceType.Should().Be(typeof(KernelPlugin));
        }

        [Fact]
        public void AddFromFunctionsWithMetadata_ShouldAddKernelPluginToCollection_AndReturnAddedPlugin()
        {
            // Arrange
            var plugins = new List<KernelPlugin>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IPluginMetadataProvider)))
                .Returns(new Mock<IPluginMetadataProvider>().Object);
            var target = new SamplePlugin();

            // Act
            var result = plugins.AddFromFunctionsWithMetadata(serviceProvider.Object, "FromFunctions", null);

            // Assert
            plugins.Should().HaveCount(1);
            result.Should().BeSameAs(plugins[0]);
        }
    }
}