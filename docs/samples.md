# Samples

Explore practical examples of using `SemanticPluginForge` in different scenarios. Each sample includes comprehensive documentation, setup instructions, and focuses on specific framework concepts.

## Available Samples

### [DefaultValue Sample](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/DefaultValue)

**Focus**: Advanced parameter handling including suppression, default values, and context-aware metadata

Demonstrates how to use intelligent default values and parameter handling in plugin metadata. Shows how custom metadata providers can provide context-aware defaults and guide LLM behavior for improved user experience.

**Key Concepts**:

- Default value management and fallback strategies
- Context extraction from user input
- Parameter suppression and visibility control
- Enhanced parameter descriptions for LLM guidance

**What You'll Learn**:

- How to suppress parameters that should be automatically resolved
- How to provide default values through metadata providers
- How to override parameter descriptions for better AI understanding
- How to ensure parameters are never resolved from context when suppressed

**Sample Code Preview**:

```csharp
public class WeatherMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "WeatherPlugin" && metadata.Name == "GetWeather")
        {
            return new FunctionMetadata(metadata.Name)
            {
                Description = "Gets weather information. Location is automatically detected from user context.",
                Parameters = new List<ParameterMetadata>
                {
                    new ParameterMetadata("location") 
                    { 
                        Suppress = true, 
                        DefaultValue = "user_current_location",
                        Description = "Location automatically resolved from user context"
                    }
                }
            };
        }
        return null;
    }
}
```

---

### [UseClrType Sample](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/UseClrType)

**Focus**: CLR type registration and metadata enhancement

Shows how to convert existing .NET classes into Semantic Kernel plugins using both type-based and object-based registration approaches. Demonstrates integration of existing codebases with AI applications.

**Key Concepts**:

- CLR type plugin registration methods
- Object vs. type-based registration patterns
- Metadata enhancement for existing classes
- Integration strategies for legacy code

**What You'll Learn**:

- How to use existing classes as plugins without modification
- How to register both types and object instances
- How to provide metadata for classes that don't have AI attributes
- How to integrate legacy business logic with AI applications

**Sample Code Preview**:

```csharp
public class ShortDate
{
    public string ToShortDateString()
    {
        return DateTime.Now.ToShortDateString();
    }
}

public class DateMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        if (plugin.Name == "ShortDatePlugin" && metadata.Name == "ToShortDateString")
        {
            return new FunctionMetadata(metadata.Name)
            {
                Description = "Returns the current date in short format (MM/dd/yyyy)."
            };
        }
        return null;
    }
}

// Registration
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<ShortDate>("ShortDatePlugin");
```

---

### [AzureAiSearchPlugin Sample](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/AzureAiSearchPlugin)

**Focus**: Multiple plugin instances with different metadata configurations

Comprehensive example showing how to create multiple instances of the same plugin class with different metadata configurations for various data sources. Uses mocked data for learning without external dependencies.

**Key Concepts**:

- Single plugin class, multiple instances pattern
- Context-aware function selection through metadata
- Metadata-driven plugin behavior differentiation
- Production-ready architecture with mocked data for learning

**What You'll Learn**:

- How to create multiple instances of the same plugin with different configurations
- How to use metadata to differentiate between similar plugins
- How to structure plugins for different data sources
- How to mock external dependencies for learning and testing

**Sample Code Preview**:

```csharp
public class SearchPlugin
{
    private readonly string _dataSource;
    
    public SearchPlugin(string dataSource)
    {
        _dataSource = dataSource;
    }
    
    public async Task<string> Search(string query)
    {
        // Implementation varies based on data source
        return await SearchInDataSource(query, _dataSource);
    }
}

public class SearchMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        // Different descriptions based on plugin instance
        return plugin.Name switch
        {
            "ProductSearchPlugin" => new FunctionMetadata(metadata.Name)
            {
                Description = "Searches product catalog for items matching the query"
            },
            "DocumentSearchPlugin" => new FunctionMetadata(metadata.Name)
            {
                Description = "Searches document repository for content matching the query"
            },
            _ => null
        };
    }
}
```

## Quick Setup

All samples require Azure OpenAI configuration. Navigate to any sample folder and configure your secrets:

```console
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "YOUR_DEPLOYMENT_NAME"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
```

Then run the sample:

```console
dotnet run
```

### Configuration Details

Each sample expects the following configuration:

- **AzureOpenAI:ChatDeploymentName**: Your Azure OpenAI chat model deployment name
- **AzureOpenAI:Endpoint**: Your Azure OpenAI service endpoint
- **AzureOpenAI:ApiKey**: Your Azure OpenAI API key

Some samples may require additional configuration. Check the individual sample README files for specific requirements.

## Learning Path

**Recommended exploration order**:

1. **[UseClrType](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/UseClrType)**: Start here to learn basic CLR type registration and metadata enhancement
2. **[DefaultValue](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/DefaultValue)**: Continue with advanced parameter handling and context awareness  
3. **[AzureAiSearchPlugin](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/AzureAiSearchPlugin)**: Finish with comprehensive patterns for production scenarios

Each sample builds upon concepts from the previous ones while focusing on specific aspects of the framework.

## Sample Structure

Each sample includes:

### 📋 Comprehensive README

Detailed documentation with:

- Clear explanation of the sample's purpose
- Step-by-step setup instructions
- Learning objectives and key takeaways
- Code explanations and best practices

### 🏗️ Architecture Overview

- Design patterns demonstrated
- Class relationships and dependencies
- Metadata provider implementations
- Plugin registration strategies

### 💡 Key Concepts

- Focused demonstration of specific framework features
- Real-world scenarios and use cases
- Problem-solving approaches
- Integration techniques

### 🔧 Setup Instructions

- Prerequisites and dependencies
- Configuration requirements
- Step-by-step execution guide
- Troubleshooting tips

### 🚀 Extension Ideas

Each sample includes suggestions for extending functionality:

- Additional features to implement
- Integration opportunities
- Performance optimizations
- Production considerations

### 📚 Learning Objectives

Clear goals for each sample:

- What you'll understand after completing it
- Skills you'll develop
- Concepts you'll master
- Next steps for further learning

## Common Patterns Demonstrated

### 1. Metadata Provider Design

```csharp
public class ExampleMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin)
    {
        // Plugin-level customizations
    }
    
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
    {
        // Function-level customizations
    }
}
```

### 2. Dependency Injection Setup

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Register metadata providers
builder.Services.AddSingleton<IPluginMetadataProvider, ExampleMetadataProvider>();

// Configure Semantic Kernel
var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.Plugins.AddFromTypeWithMetadata<ExamplePlugin>();
```

### 3. Plugin Registration Patterns

```csharp
// Standard plugin with attributes
kernelBuilder.Plugins.AddFromTypeWithMetadata<StandardPlugin>();

// CLR type without attributes
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<LegacyClass>("LegacyPlugin");

// Object instance
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(instance, "InstancePlugin");
```

## Prerequisites

Before running the samples, ensure you have:

- **.NET 8.0 SDK** or later
- **Azure OpenAI service** with a chat deployment
- **Basic understanding** of Semantic Kernel concepts
- **Visual Studio Code** or **Visual Studio** (recommended)

## Getting Help

If you encounter issues with the samples:

1. **Check the sample README**: Each sample has detailed setup instructions
2. **Verify configuration**: Ensure your Azure OpenAI settings are correct
3. **Review logs**: Check console output for error messages
4. **Compare with documentation**: Reference the main documentation for concepts
5. **Open an issue**: Report problems on the GitHub repository

## Contributing Sample Ideas

We welcome contributions of new samples! Consider creating samples that demonstrate:

- Industry-specific use cases
- Integration with other services
- Advanced metadata provider patterns
- Performance optimization techniques
- Testing strategies

Navigate to the [`samples`](https://github.com/lsiddiquee/SemanticPluginForge/tree/main/samples/) folder to get started with these examples.
