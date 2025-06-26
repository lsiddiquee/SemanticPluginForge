# Default Value Plugin Sample

This sample demonstrates how to use **default values and parameter suppression** in Semantic Kernel plugin metadata. It shows how custom metadata providers can provide intelligent defaults and context-aware parameter handling for improved user experience.

The sample features a simple **WeatherPlugin** that retrieves temperature information, with a custom metadata provider that supplies context-aware default values and enhanced parameter descriptions.

> **Note**: This sample uses a simple weather plugin with mocked responses to demonstrate metadata concepts. The focus is on showing how default values and parameter descriptions can enhance the user experience in AI-driven applications.

## Key Features

- **Default Value Management**: Demonstrates how to provide intelligent default values for plugin parameters
- **Context-Aware Metadata**: Shows how metadata can guide the LLM to extract values from user context
- **Parameter Suppression Control**: Illustrates fine-grained control over parameter visibility
- **Enhanced Descriptions**: Provides detailed parameter descriptions that help the LLM understand intent
- **Multiple User Contexts**: Tests different scenarios with varying levels of context information

## Architecture

### WeatherPlugin Class

- **Single Responsibility**: Simple temperature lookup using city name and unit parameters
- **Standard Attributes**: Uses standard .NET attributes for basic function metadata
- **Default Parameters**: Includes C# default parameters that are enhanced by custom metadata

### Custom Metadata Provider

The `CustomMetadataProvider` enhances the basic plugin functionality by:

- **Enhanced Descriptions**: Provides detailed, context-aware parameter descriptions
- **Smart Defaults**: Sets intelligent default values (London for city, fahrenheit for unit)
- **Context Guidance**: Instructs the LLM to extract values from user context when available
- **Required Parameter Control**: Manages which parameters are required vs optional

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Azure OpenAI service with a chat deployment

### Configuration

Navigate to the project folder and set up user secrets for Azure OpenAI:

```console
cd samples\DefaultValue
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

### Default Value Scenarios

The sample demonstrates three different user context scenarios:

1. **Complete Context**: User with location and preferred unit
   - Context: `{"name": "John Doe", "location": "Utrecht", "preferredUnit": "celsius"}`
   - Expected: LLM extracts both city and unit from context

2. **Complete Context**: User with location and preferred unit
   - Context: `{"name": "Jane Doe", "location": "Rotterdam", "preferredUnit": "fahrenheit"}`
   - Expected: LLM extracts both city and unit from context

3. **Minimal Context**: User with no location specified
   - Context: `{"name": "Jan Jansen", "preferredUnit": "celsius"}`
   - Expected: LLM falls back to default location (London), uses unit from context

### Metadata-Driven Behavior

The custom metadata provider guides the LLM through enhanced descriptions:

- **City Parameter**: "The name of the city should be retrieved from the user context, if not in context, please ask the user."
- **Unit Parameter**: "The unit should be retrieved from the user context."
- **Default Values**: Provides fallback values when context is incomplete

### Context Extraction

The sample shows how the LLM can:

- Parse user context JSON to extract relevant values
- Apply intelligent defaults when context is missing
- Handle different context completeness levels gracefully

## Key Concepts Demonstrated

### 1. Default Value Strategy

```csharp
new ParameterMetadata("name")
{
    Description = "The name of the city should be retrieved from the user context, if not in context, please ask the user.",
    IsRequired = true,
    Suppress = false,
    DefaultValue = "London",
}
```

### 2. Context-Aware Descriptions

The metadata provides clear instructions to the LLM about:

- Where to find parameter values (user context)
- What to do when values are missing
- How to handle different scenarios

### 3. Parameter Control

- **IsRequired**: Controls whether the parameter must be provided
- **Suppress**: Controls whether the parameter is hidden from the LLM
- **DefaultValue**: Provides fallback values for missing context

## Extending the Sample

You can extend this sample by:

1. **Complex Default Logic**: Implement conditional defaults based on multiple context factors
2. **Validation Rules**: Add parameter validation and constraints
3. **Dynamic Defaults**: Generate defaults based on external services or user history
4. **Nested Context**: Handle more complex user context structures
5. **Conditional Suppression**: Dynamically show/hide parameters based on context

## Real-World Applications

This pattern is useful for:

- **Personalization**: Using user preferences and profile data as defaults
- **Location Services**: Defaulting to user's current or preferred location
- **Configuration Management**: Applying user-specific or organization-wide defaults
- **Progressive Disclosure**: Showing only relevant parameters based on context
- **Smart Assistants**: Reducing friction by intelligently filling common parameters

## Learning Objectives

This sample demonstrates:

- How to implement intelligent default values in plugin metadata
- Techniques for context-aware parameter handling
- Best practices for parameter description and LLM guidance
- Strategies for handling incomplete user context gracefully
- The relationship between C# default parameters and metadata defaults
- How to balance automation with user control in AI applications
