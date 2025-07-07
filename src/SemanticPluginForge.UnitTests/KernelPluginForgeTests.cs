using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests
{
    public partial class KernelPluginForgeTests
    {
        [Fact]
        public void CreateFromClrObjectWithMetadata_ShouldReturnValidPlugin()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPluginMetadataProvider, StubPluginMetadataProvider>()
                .BuildServiceProvider();

            var targetObject = new DateTime();
            string pluginName = "DateTimePlugin";

            // Act
            var plugin = KernelPluginForge.CreateFromClrObjectWithMetadata(targetObject, serviceProvider, pluginName);

            // Assert
            plugin.Should().NotBeNull();
            plugin.Name.Should().Be(pluginName);
            plugin.Description.Should().Be("This plugin returns date and time information.");
            plugin.FunctionCount.Should().Be(1);
            plugin["ToShortDateString"].Should().NotBeNull();
            plugin["ToShortDateString"].Metadata.Description.Should().Be("Returns the date in short format.");
        }

        [Fact]
        public void CreateFromClrTypeWithMetadata_ShouldReturnValidPlugin()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPluginMetadataProvider, StubPluginMetadataProvider>()
                .BuildServiceProvider();

            string pluginName = "DateTimePlugin";

            // Act
            var plugin = KernelPluginForge.CreateFromClrTypeWithMetadata<StubPluginWithoutAttribute>(serviceProvider, pluginName);

            // Assert
            plugin.Should().NotBeNull();
            plugin.Name.Should().Be(pluginName);
            plugin.Description.Should().Be("This plugin returns date and time information.");
            plugin.FunctionCount.Should().Be(1);
            plugin["ToShortDateString"].Should().NotBeNull();
            plugin["ToShortDateString"].Metadata.Description.Should().Be("Returns the date in short format.");
        }

        [Fact]
        public void CreateFromClrObjectWithMetadata_ThrowsException_WhenPluginMetadataIsNull()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPluginMetadataProvider, StubPluginMetadataProvider>()
                .BuildServiceProvider();

            var targetObject = new DateTime();
            var pluginName = "InvalidPlugin";

            // Act
            Action act = () => KernelPluginForge.CreateFromClrObjectWithMetadata(targetObject, serviceProvider, pluginName);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateFromClrObjectWithMetadata_ThrowsException_WhenNoFunctionMetadataDefined()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPluginMetadataProvider, StubPluginMetadataProvider>()
                .BuildServiceProvider();

            var target = new DateTimeOffset();
            var pluginName = "DateTimePlugin";

            // Act
            Action act = () => KernelPluginForge.CreateFromClrObjectWithMetadata(target, serviceProvider, pluginName);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateFromClrObjectWithMetadata_FiltersDuplicateMethods()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IPluginMetadataProvider, AsyncMethodsMetadataProvider>()
                .BuildServiceProvider();

            var target = new AsyncMethodsPlugin();
            var pluginName = "AsyncMethodsPlugin";

            // Act
            var plugin = KernelPluginForge.CreateFromClrObjectWithMetadata(target, serviceProvider, pluginName);

            // Assert
            plugin.Should().NotBeNull();
            plugin.Name.Should().Be(pluginName);
            plugin.FunctionsMetaShouldBe([
                new FunctionMetadata("GetData")
                {
                    Description = "Function GetData for testing async filtering.",
                    Parameters = [],
                    ReturnParameter = new ReturnParameterMetadata { Description = string.Empty }
                }
            ]);
        }
    }
}