# Use CLR Type Plugin Sample

This sample demonstrates how to create Semantic Kernel plugins from **custom CLR (Common Language Runtime) types** using the SemanticPluginForge framework. It shows how to register both type-based and object-based plugins with enhanced metadata providers.

The sample features a simple **ShortDate** class that provides date formatting functionality, demonstrating how any .NET class can be transformed into a Semantic Kernel plugin with custom metadata.

> **Note**: This sample uses a simple date utility class to demonstrate CLR type plugin concepts. The focus is on showing how to register custom .NET types as plugins and enhance them with metadata providers.

## Key Features

- **CLR Type Registration**: Demonstrates registering .NET types directly as Semantic Kernel plugins
- **Object Instance Registration**: Shows alternative registration using object instances
- **Custom Metadata Enhancement**: Illustrates how metadata providers can enhance basic CLR type functionality
- **Simple Plugin Design**: Uses a focused, single-purpose class to demonstrate core concepts
- **Type-Safe Integration**: Leverages .NET's type system for plugin registration

## Architecture

### ShortDate Class

- **Single Responsibility**: Provides date formatting functionality through `ToShortDateString()` method
- **Standard .NET Class**: Regular C# class without special plugin attributes
- **Simple Interface**: Single public method that returns formatted date string
- **No Dependencies**: Standalone class that demonstrates basic CLR type plugin concepts

### Custom Metadata Provider

The `CustomMetadataProvider` enhances the basic CLR type by:

- **Plugin Description**: Provides meaningful description for the date/time functionality
- **Function Enhancement**: Adds descriptive metadata for the date formatting function
- **Semantic Context**: Helps the LLM understand the plugin's purpose and capabilities

### Registration Options

The sample demonstrates two registration approaches:

1. **Type-Based**: `AddFromClrTypeWithMetadata<ShortDate>("ShortDatePlugin")`
2. **Object-Based**: `AddFromClrObjectWithMetadata(targetObject, "ShortDatePlugin")` (commented example)

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Azure OpenAI service with a chat deployment

### Configuration

Navigate to the project folder and set up user secrets for Azure OpenAI:

```console
cd samples\UseClrType
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "YOUR_DEPLOYMENT_NAME"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
```

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
// Type-based registration (recommended)
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<ShortDate>("ShortDatePlugin");

// Alternative object-based registration
// var targetObject = new ShortDate();
// kernelBuilder.Plugins.AddFromClrObjectWithMetadata(targetObject, "ShortDatePlugin");
```

### Metadata Enhancement

The custom metadata provider adds semantic meaning:

```csharp
plugin.Name == "ShortDatePlugin" && metadata.Name == "ToShortDateString" ? 
    new FunctionMetadata(metadata.Name)
    {
        Description = "Returns the date in short format."
    } : null
```

### LLM Integration

The sample demonstrates how the LLM can:

- Understand the plugin's purpose through enhanced descriptions
- Automatically invoke the date formatting function when requested
- Integrate the plugin response into natural language conversations

## Key Concepts Demonstrated

### 1. CLR Type Plugin Registration

Any .NET class can become a plugin:

```csharp
public class ShortDate
{
    public string ToShortDateString()
    {
        return DateTime.Now.ToShortDateString();
    }
}
```

### 2. Metadata Provider Integration

Enhance basic CLR types with semantic information:

```csharp
public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
    plugin.Name == "ShortDatePlugin" && metadata.Name == "ToShortDateString" ? 
        new FunctionMetadata(metadata.Name)
        {
            Description = "Returns the date in short format."
        } : null;
```

### 3. Flexible Registration Options

- **Type-based**: Register the type, let the framework create instances
- **Object-based**: Register a specific object instance

## Extending the Sample

You can extend this sample by:

1. **Complex CLR Types**: Register classes with multiple methods and properties
2. **Parameter Handling**: Add classes with method parameters and custom types
3. **State Management**: Use object-based registration for stateful plugins
4. **Dependency Injection**: Integrate with .NET DI container for complex objects
5. **Generic Types**: Explore registration of generic classes and methods
6. **Property Exposure**: Extend to expose properties as plugin functions

## Real-World Applications

This pattern is useful for:

- **Existing Code Integration**: Converting existing .NET libraries into AI plugins
- **Utility Functions**: Exposing common utility classes as AI-accessible tools
- **Business Logic**: Making domain-specific business logic available to AI models
- **Legacy System Integration**: Bridging older .NET code with modern AI applications
- **Rapid Prototyping**: Quick conversion of existing classes for AI experimentation

## Comparison with Other Registration Methods

| Method | Use Case | Pros | Cons |
|--------|----------|------|------|
| `AddFromType` | Standard plugin classes | Simple, attribute-driven | Requires plugin-specific attributes |
| `AddFromClrType` | Existing .NET classes | No attribute requirements | Limited metadata |
| `AddFromClrTypeWithMetadata` | Enhanced existing classes | Rich metadata, flexible | Requires metadata provider |
| `AddFromObject` | Stateful instances | Instance control | Manual lifecycle management |

## Best Practices

When using CLR type registration:

1. **Keep It Simple**: Start with simple, focused classes
2. **Clear Naming**: Use descriptive method names that indicate purpose
3. **Return Value Types**: Use appropriate return types that LLMs can work with
4. **Metadata Enhancement**: Always provide metadata providers for better LLM understanding
5. **Error Handling**: Include proper error handling in your methods
6. **Thread Safety**: Consider thread safety for shared instances

## Learning Objectives

This sample demonstrates:

- How to register existing .NET types as Semantic Kernel plugins
- The difference between type-based and object-based plugin registration
- How metadata providers enhance basic CLR type functionality
- Integration patterns for existing codebases with AI applications
- Best practices for converting utility classes into AI-accessible tools
- The flexibility of the SemanticPluginForge framework for various plugin types
