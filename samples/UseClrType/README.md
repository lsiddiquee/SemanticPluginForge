# Use CLR Type Plugin Sample

This sample demonstrates how to create Semantic Kernel plugins from **custom CLR (Common Language Runtime) types** using the SemanticPluginForge framework. It shows how to register both type-based and object-based plugins with enhanced metadata providers, and how to use `FunctionMetadata.OverrideFunctionName` to handle overloaded methods.

The sample features:

- A **DateTimeWrapper** class that provides date and time functionality
- The standard .NET **Random** class for random number generation with overloaded methods
- Direct integration with the **Azure Storage Queue Client**

Together, these demonstrate how any .NET class can be transformed into a Semantic Kernel plugin with custom metadata.

The sample also showcases:

- How to use `FunctionMetadata.OverrideFunctionName` to handle overloaded methods like `Random.Next()`
- How to enhance CLR types with detailed metadata for improved LLM understanding
- Techniques for integrating Azure SDK clients directly as AI plugins

> **Note**: This sample demonstrates CLR type plugin concepts with both custom and standard .NET classes, as well as Azure service clients. The focus is on showing how to register custom .NET types as plugins and enhance them with metadata providers.

## Key Features

- **CLR Type Registration**: Demonstrates registering .NET types directly as Semantic Kernel plugins
- **Object Instance Registration**: Shows alternative registration using object instances
- **Custom Metadata Enhancement**: Illustrates how metadata providers can enhance basic CLR type functionality
- **Multiple Plugin Types**: Demonstrates custom classes, standard .NET classes, and Azure SDK clients as plugins
- **Direct SDK Integration**: Shows how to expose Azure SDK functionality directly to the LLM
- **Function Name Overrides**: Uses custom function names for methods with same name but different parameters
- **Type-Safe Integration**: Leverages .NET's type system for plugin registration

## Architecture

### DateTimeWrapper Class

- **Multiple Date/Time Methods**: Provides date and time functionality through various methods
- **Standard .NET Class**: Regular C# class without special plugin attributes
- **Simple Interface**: Methods return formatted date and time strings
- **No Dependencies**: Standalone class that demonstrates basic CLR type plugin concepts

### Random Class (Standard .NET)

- **Built-in .NET Class**: Demonstrates using framework classes as plugins
- **Method Overloads**: Shows handling of multiple methods with the same name
- **Function Name Overrides**: Uses custom names to disambiguate overloaded methods

### Azure Storage Queue Client

- **Direct SDK Integration**: Uses the Azure SDK QueueClient without a wrapper
- **Real-World Application**: Demonstrates practical cloud service integration
- **Authentication Support**: Uses DefaultAzureCredential for secure authentication

### Custom Metadata Provider

The `CustomMetadataProvider` enhances the CLR types by:

- **Plugin Descriptions**: Provides meaningful descriptions for each plugin type
- **Function Enhancement**: Adds descriptive metadata for each function
- **Parameter Documentation**: Documents parameters for complex methods
- **Function Name Overrides**: Provides friendly names for overloaded methods
- **Default Values**: Sets default parameter values where appropriate
- **Semantic Context**: Helps the LLM understand each plugin's purpose and capabilities

### Registration Options

The sample demonstrates multiple registration approaches:

1. **Type-Based for Custom Class**: `AddFromClrTypeWithMetadata<DateTimeWrapper>("DateTimeWrapper")`
2. **Type-Based for Standard .NET Class**: `AddFromClrTypeWithMetadata<Random>("RandomPlugin")`
3. **Object-Based for Azure SDK Client**: `AddFromClrObjectWithMetadata(qc, "Queue")`

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Azure OpenAI service with a chat deployment

### Configuration

Navigate to the project folder and set up user secrets for Azure OpenAI and Azure Storage Queue:

```console
cd samples\UseClrType
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "YOUR_DEPLOYMENT_NAME"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
dotnet user-secrets set "AzureQueueUri" "https://<storage_account>.queue.core.windows.net/<queue_name>"
```

The sample uses Azure Identity's DefaultAzureCredential for authentication to Azure Storage. Make sure you are logged in with the Azure CLI or have appropriate credentials configured.

### Run the Sample

```console
dotnet run
```

## How It Works

### CLR Type Discovery

When registering a CLR type as a plugin:

1. **Method Scanning**: The framework automatically discovers public methods
2. **Parameter Analysis**: Method parameters become plugin function parameters
3. **Return Type Mapping**: Method return types are mapped to plugin return values
4. **Metadata Enhancement**: Custom metadata providers can enhance the discovered information

### Plugin Registration Process

```csharp
// Type-based registration for custom classes
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<DateTimeWrapper>("DateTimeWrapper");

// Type-based registration for standard .NET classes
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<Random>("RandomPlugin");

// Object-based registration for Azure SDK clients
var qc = new QueueClient(new Uri(builder.Configuration["AzureQueueUri"]!), new DefaultAzureCredential());
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(qc, "Queue");
```

### Metadata Enhancement

The custom metadata provider adds semantic meaning to each plugin:

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata)
{
    return plugin.Name switch
    {
        "DateTimeWrapper" => metadata.Name switch
        {
            "ToShortDateString" => new FunctionMetadata(metadata.Name)
            {
                Description = "Returns the current date in short format (MM/dd/yyyy)."
            },
            // Other DateTimeWrapper methods...
        },
        "RandomPlugin" => metadata.Name switch
        {
            "Next" when metadata.Parameters.Count == 1 => new FunctionMetadata(metadata.Name)
            {
                OverrideFunctionName = "NextWithUpperBound",
                Description = "Returns a random integer within a specified range.",
                Parameters = [
                    new ParameterMetadata("maxValue") 
                    { 
                        Description = "The exclusive upper bound of the random number returned." 
                    }
                ]
            },
            // Other Random methods...
        },
        "Queue" => metadata.Name switch
        {
            "SendMessage" => new FunctionMetadata(metadata.Name)
            {
                Description = "Sends a message to the Azure Storage Queue.",
                // Parameter definitions...
            },
            // Other Queue methods...
        },
        _ => null
    };
}
```

### LLM Integration

The sample demonstrates how the LLM can:

- Understand multiple plugins' purposes through enhanced descriptions
- Automatically invoke the appropriate function based on user requests
- Handle method overloads through function name overrides
- Work with Azure services directly through SDK integration
- Integrate plugin responses into natural language conversations

## Key Concepts Demonstrated

### 1. CLR Type Plugin Registration

Any .NET class can become a plugin:

```csharp
public class DateTimeWrapper
{
    public string ToShortDateString()
    {
        return DateTime.Now.ToShortDateString();
    }

    public string ToLongDateString()
    {
        return DateTime.Now.ToLongDateString();
    }

    public string CurrentTime()
    {
        return DateTime.Now.ToString("T");
    }
    
    // Additional methods...
}
```

### 2. Metadata Provider Integration

Enhance CLR types with semantic information:

```csharp
public class CustomMetadataProvider : IPluginMetadataProvider
{
    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin)
    {
        return plugin.Name switch
        {
            "DateTimeWrapper" => new PluginMetadata
            {
                Description = "This plugin returns date and time information."
            },
            "RandomPlugin" => new PluginMetadata
            {
                Description = "This plugin generates random numbers and values."
            },
            "Queue" => new PluginMetadata
            {
                Description = "This plugin interacts with Azure Storage Queues."
            },
            _ => null,
        };
    }

    // Function metadata implementation...
}
```

### 3. Flexible Registration Options

- **Type-based for custom classes**: Register your own classes, letting the framework create instances
- **Type-based for standard .NET classes**: Register built-in .NET classes like Random, DateTime, etc.
- **Object-based for service clients**: Register Azure SDK clients and other service objects
- **Method overload handling**: Use function name overrides for method disambiguation

## Extending the Sample

You can extend this sample by:

1. **Additional Azure Services**: Register other Azure SDK clients like Blob Storage or CosmosDB
2. **Complex CLR Types**: Register classes with more complex methods and properties
3. **Authentication Handlers**: Explore different authentication mechanisms for Azure services
4. **State Management**: Use object-based registration for stateful plugins
5. **Dependency Injection**: Integrate with .NET DI container for complex objects
6. **Generic Types**: Explore registration of generic classes and methods
7. **Property Exposure**: Extend to expose properties as plugin functions

## Real-World Applications

This pattern is useful for:

- **Azure SDK Integration**: Exposing Azure services directly to AI models
- **Existing Code Integration**: Converting existing .NET libraries into AI plugins
- **Utility Functions**: Exposing common utility classes as AI-accessible tools
- **Business Logic**: Making domain-specific business logic available to AI models
- **Third-Party SDK Integration**: Bridging external libraries with AI applications
- **Legacy System Integration**: Connecting older .NET code with modern AI applications
- **Rapid Prototyping**: Quick conversion of existing classes for AI experimentation

## Comparison with Other Registration Methods

| Method | Use Case | Pros | Cons |
|--------|----------|------|------|
| `AddFromType` | Standard plugin classes | Simple, attribute-driven | Requires plugin-specific attributes |
| `AddFromClrType` | Existing .NET classes | No attribute requirements | Limited metadata |
| `AddFromClrTypeWithMetadata` | Enhanced existing classes | Rich metadata, flexible | Requires metadata provider |
| `AddFromObject` | Stateful instances, SDK clients | Instance control, pre-configured clients | Manual lifecycle management |
| `AddFromClrObjectWithMetadata` | Enhanced SDK clients | Rich metadata, pre-configured instances | Requires metadata provider and manual lifecycle |

## Best Practices

When using CLR type registration:

1. **Clear Naming**: Use descriptive plugin and function names that indicate purpose
2. **Meaningful Descriptions**: Provide detailed descriptions for plugins, functions, and parameters
3. **Method Disambiguation**: Use function name overrides for overloaded methods
4. **Parameter Documentation**: Document all parameters, especially for complex methods
5. **Default Values**: Provide default values for optional parameters
6. **Authentication Handling**: Use appropriate authentication for Azure and other services
7. **Error Handling**: Include proper error handling in your methods
8. **Thread Safety**: Consider thread safety for shared instances

## Learning Objectives

This sample demonstrates:

- How to register existing .NET types as Semantic Kernel plugins
- How to integrate Azure SDK clients directly as plugins
- The difference between type-based and object-based plugin registration
- How to handle method overloads using function name overrides
- How metadata providers enhance CLR type functionality
- Integration patterns for cloud services with AI applications
- Best practices for converting utility classes into AI-accessible tools
