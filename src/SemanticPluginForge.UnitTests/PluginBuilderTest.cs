using FluentAssertions;
using Microsoft.SemanticKernel;
using Moq;

using SemanticPluginForge.Core;

namespace SemanticPluginForge.UnitTests;

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
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().BeSameAs(kernelPlugin);
        result.Description.Should().Be("Sample plugin for testing.");
        result.FunctionsMetaShouldBe(GetExpectedFunctionsMetadata());
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
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().NotBeSameAs(kernelPlugin);
        result.Description.Should().Be("Plugin description altered using patch mechanism.");
        result.FunctionsMetaShouldBe(GetExpectedFunctionsMetadata());
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenFunctionMetadataExists()
    {
        // Arrange
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetCurrentUsername))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) {
                Description = "Function description altered using patch mechanism.",
                ReturnParameter = new ReturnParameterMetadata { Description = "The username of the current user." }
        });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().NotBeSameAs(kernelPlugin);
        result.Description.Should().Be("Sample plugin for testing.");
        var expectedFunctionsMetadata = GetExpectedFunctionsMetadata(
            usernameFunctionDescription: "Function description altered using patch mechanism.",
            usernameReturnDescription: "The username of the current user."
        );
        result.FunctionsMetaShouldBe(expectedFunctionsMetadata);
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenPluginDescriptionAndFunctionMetadataExists()
    {
        // Arrange
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetCurrentUsername))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) {
                Description = "Function description altered using patch mechanism."
        });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().NotBeSameAs(kernelPlugin);
        result.Description.Should().Be("Plugin description altered using patch mechanism.");
        var expectedFunctionsMetadata = GetExpectedFunctionsMetadata(
            usernameFunctionDescription: "Function description altered using patch mechanism."
        );
        result.FunctionsMetaShouldBe(expectedFunctionsMetadata);
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenFunctionIsSuppressed()
    {
        // Arrange
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetCurrentUsername))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Suppress = true });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().NotBeSameAs(kernelPlugin);
        result.Description.Should().Be("Plugin description altered using patch mechanism.");
        var expectedFunctionsMetadata = GetExpectedFunctionsMetadata();
        expectedFunctionsMetadata.Remove(expectedFunctionsMetadata.Single(f => f.Name == nameof(SamplePlugin.GetCurrentUsername)));
        result.FunctionsMetaShouldBe(expectedFunctionsMetadata);
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenParameterIsSuppressed()
    {
        // Arrange
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetAllCities))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Parameters = new List<ParameterMetadata> { new ParameterMetadata("filter") { Suppress = true } } });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().NotBeSameAs(kernelPlugin);
        result.Description.Should().Be("Plugin description altered using patch mechanism.");
        var expectedFunctionsMetadata = GetExpectedFunctionsMetadata();
        var expectedFunctionMetadata = expectedFunctionsMetadata.Single(f => f.Name == nameof(SamplePlugin.GetAllCities));
        expectedFunctionsMetadata.Remove(expectedFunctionMetadata);
        expectedFunctionsMetadata.Add(new FunctionMetadata(expectedFunctionMetadata.Name)
        {
            Description = expectedFunctionMetadata.Description,
            Parameters = expectedFunctionMetadata.Parameters!.Where(p => p.Name != "filter").ToList(),
            ReturnParameter = expectedFunctionMetadata.ReturnParameter
        });
        result.FunctionsMetaShouldBe(expectedFunctionsMetadata);
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldThrowException_WhenAttemptingToSuppressRequiredParameterWithoutDefault()
    {
        // 
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetTemperatureByCity))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Parameters = new List<ParameterMetadata> { new ParameterMetadata("name") { Suppress = true } } });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act/Assert
        pluginBuilder.Invoking(x => x.PatchKernelPluginWithMetadata(kernelPlugin))
            .Should().Throw<ArgumentException>()
            .WithMessage("Parameter 'name' is required and cannot be suppressed without a default value.");
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldReturnAlteredPlugin_WhenAttemptingToSuppressRequiredParameterWithDefault()
    {
        // 
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetTemperatureByCity))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Parameters = new List<ParameterMetadata> { new ParameterMetadata("name") { Suppress = true, DefaultValue = "Amsterdam" } } });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var result = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var functionsMetadata = result.GetFunctionsMetadata();

        // Assert
        result.Should().NotBeSameAs(kernelPlugin);
        result.Description.Should().Be("Plugin description altered using patch mechanism.");
        var expectedFunctionsMetadata = GetExpectedFunctionsMetadata();
        var expectedFunctionMetadata = expectedFunctionsMetadata.Single(f => f.Name == nameof(SamplePlugin.GetTemperatureByCity));
        expectedFunctionsMetadata.Remove(expectedFunctionMetadata);
        expectedFunctionsMetadata.Add(new FunctionMetadata(expectedFunctionMetadata.Name)
        {
            Description = expectedFunctionMetadata.Description,
            Parameters = expectedFunctionMetadata.Parameters!.Where(p => p.Name != "name").ToList(),
            ReturnParameter = expectedFunctionMetadata.ReturnParameter
        });
        result.FunctionsMetaShouldBe(expectedFunctionsMetadata);
    }

    [Fact]
    public async Task PatchKernelPluginWithMetadata_UsesCorrectDefault_SuppressRequiredParameterWithDefaultAsync()
    {
        // 
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetPluginMetadata(It.IsAny<KernelPlugin>())).Returns<KernelPlugin>(p => new PluginMetadata { Description = "Plugin description altered using patch mechanism." });
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetTemperatureByCity))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Parameters = new List<ParameterMetadata> { new ParameterMetadata("name") { Suppress = true, DefaultValue = "Amsterdam" } } });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var patchedPlugin = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var f = patchedPlugin[nameof(SamplePlugin.GetTemperatureByCity)];
        var result = await f.InvokeAsync(new Kernel());

        // Assert
        result.GetValue<string>().Should().Be("Amsterdam temperature is 20 degrees celsius");
        var functionsMetadata = patchedPlugin.GetFunctionsMetadata();
        var expectedFunctionsMetadata = GetExpectedFunctionsMetadata();
        var expectedFunctionMetadata = expectedFunctionsMetadata.Single(f => f.Name == nameof(SamplePlugin.GetTemperatureByCity));
        expectedFunctionsMetadata.Remove(expectedFunctionMetadata);
        expectedFunctionsMetadata.Add(new FunctionMetadata(expectedFunctionMetadata.Name)
        {
            Description = expectedFunctionMetadata.Description,
            Parameters = expectedFunctionMetadata.Parameters!.Where(p => p.Name != "name").ToList(),
            ReturnParameter = expectedFunctionMetadata.ReturnParameter
        });
        patchedPlugin.FunctionsMetaShouldBe(expectedFunctionsMetadata);
    }

    [Fact]
    public async Task PatchKernelPluginWithMetadata_UsesCorrectDefault_SuppressOptionalParameterWithDefaultAsync()
    {
        // 
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetTemperatureByCity))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Parameters = new List<ParameterMetadata> { new ParameterMetadata("unit") { Suppress = true } } });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var patchedPlugin = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var f = patchedPlugin[nameof(SamplePlugin.GetTemperatureByCity)];
        var result = await f.InvokeAsync(new Kernel(), new KernelArguments { { "name", "Utrecht" } });

        // Assert
        result.GetValue<string>().Should().Be("Utrecht temperature is 20 degrees celsius");
    }

    [Fact]
    public async Task PatchKernelPluginWithMetadata_UsesCorrectDefault_SuppressOptionalParameterWithDefaultOverrideAsync()
    {
        // 
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        metadataProviderMock.Setup(p => p.GetFunctionMetadata(It.IsAny<KernelPlugin>(), It.Is<KernelFunctionMetadata>(func => func.Name == nameof(SamplePlugin.GetTemperatureByCity))))
            .Returns<KernelPlugin, KernelFunctionMetadata>((p, m) => new FunctionMetadata(m.Name) { Parameters = new List<ParameterMetadata> { new ParameterMetadata("unit") { Suppress = true, DefaultValue = "fahrenheit" } } });

        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var kernelPlugin = KernelPluginFactory.CreateFromObject(new SamplePlugin());

        // Act
        var patchedPlugin = pluginBuilder.PatchKernelPluginWithMetadata(kernelPlugin);
        var f = patchedPlugin[nameof(SamplePlugin.GetTemperatureByCity)];
        var result = await f.InvokeAsync(new Kernel(), new KernelArguments { { "name", "Rotterdam" } });

        // Assert
        result.GetValue<string>().Should().Be("Rotterdam temperature is 20 degrees fahrenheit");
    }

    [Fact]
    public void PatchKernelPluginWithMetadata_ShouldHandleEmptyPlugin()
    {
        // Arrange
        var metadataProviderMock = new Mock<IPluginMetadataProvider>();
        var pluginBuilder = new PluginBuilder(metadataProviderMock.Object);
        var emptyPlugin = KernelPluginFactory.CreateFromFunctions("EmptyPlugin", "", new List<KernelFunction>());

        // Act
        var result = pluginBuilder.PatchKernelPluginWithMetadata(emptyPlugin);

        // Assert
        result.Should().BeSameAs(emptyPlugin);
        result.FunctionCount.Should().Be(0);
    }

    private List<FunctionMetadata> GetExpectedFunctionsMetadata(
        string usernameFunctionDescription = "Returns the current username.",
        string usernameReturnDescription = "",
        string citiesFunctionDescription = "Returns all the cities based on the filter provided.",
        string citiesReturnDescription = ""
    )
    {
        return 
        [
            new FunctionMetadata(nameof(SamplePlugin.GetCurrentUsername))
            {
                Description = usernameFunctionDescription,
                Parameters = [],
                ReturnParameter = new ReturnParameterMetadata { Description = usernameReturnDescription }
            },
            new FunctionMetadata(nameof(SamplePlugin.GetAllCities))
            {
                Description = citiesFunctionDescription,
                Parameters = 
                [
                    new ParameterMetadata("filter")
                    {
                        Description = "Optional filter parameter which can be used to get a subset of cities",
                        IsRequired = false,
                        DefaultValue = null
                    }
                ],
                ReturnParameter = new ReturnParameterMetadata { Description = citiesReturnDescription }
            },
            new FunctionMetadata(nameof(SamplePlugin.GetTemperatureByCity))
            {
                Description = "Returns the current temperature of the city.",
                Parameters = 
                [
                    new ParameterMetadata("name")
                    {
                        Description = "The name of the city",
                        IsRequired = true,
                        DefaultValue = null
                    },
                    new ParameterMetadata("unit")
                    {
                        Description = "The temperature unit, default is celsius",
                        IsRequired = false,
                        DefaultValue = "celsius"
                    }
                ],
                ReturnParameter = new ReturnParameterMetadata { Description = "" }
            }
        ];
    }
}
