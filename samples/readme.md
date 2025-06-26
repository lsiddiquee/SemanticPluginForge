# SemanticPluginForge Samples

This directory contains comprehensive samples demonstrating different aspects of the SemanticPluginForge framework for building enhanced Semantic Kernel plugins. Each sample focuses on specific concepts and includes detailed documentation, setup instructions, and learning objectives.

## Available Samples

### 1. DefaultValue Sample

**Focus**: Default values, parameter suppression, and context-aware metadata

Demonstrates how to use intelligent default values and parameter handling in plugin metadata. Shows how custom metadata providers can provide context-aware defaults and guide LLM behavior for improved user experience.

**Key Concepts**:

- Default value management and fallback strategies
- Context extraction from user input
- Parameter suppression and visibility control
- Enhanced parameter descriptions for LLM guidance

[📖 View DefaultValue README](DefaultValue/README.md)

### 2. UseClrType Sample

**Focus**: CLR type registration and metadata enhancement

Shows how to convert existing .NET classes into Semantic Kernel plugins using both type-based and object-based registration approaches. Demonstrates integration of existing codebases with AI applications.

**Key Concepts**:

- CLR type plugin registration methods
- Object vs. type-based registration patterns
- Metadata enhancement for existing classes
- Integration strategies for legacy code

[📖 View UseClrType README](UseClrType/README.md)

### 3. AzureAiSearchPlugin Sample

**Focus**: Multiple plugin instances with different metadata configurations

Comprehensive example showing how to create one plugin class with multiple instances, each having distinct metadata for different data sources. Uses mocked data to demonstrate concepts without requiring external services.

**Key Concepts**:

- Single plugin class, multiple instances pattern
- Context-aware function selection through metadata
- Metadata-driven plugin behavior differentiation
- Production-ready architecture with mocked data for learning

[📖 View AzureAiSearchPlugin README](AzureAiSearchPlugin/README.md)

## Quick Setup

All samples require Azure OpenAI configuration. Navigate to any sample folder and run:

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

## Learning Path

**Recommended order for exploring the samples**:

1. **Start with UseClrType**: Learn basic CLR type registration and metadata enhancement
2. **Continue with DefaultValue**: Understand advanced parameter handling and context awareness  
3. **Finish with AzureAiSearchPlugin**: See comprehensive patterns for production scenarios

Each sample builds upon concepts from the previous ones while focusing on specific aspects of the framework.

## Sample Structure

Each sample includes:

- **📋 Comprehensive README**: Detailed documentation with setup instructions and learning objectives
- **🏗️ Architecture Overview**: Clear explanation of the sample's design and patterns
- **💡 Key Concepts**: Focused demonstration of specific framework features
- **🔧 Setup Instructions**: Step-by-step configuration and execution guide
- **🚀 Extension Ideas**: Suggestions for expanding the sample functionality
- **📚 Learning Objectives**: Clear goals and takeaways from each sample

## Prerequisites

- .NET 8.0 SDK
- Azure OpenAI service with a chat deployment
- Basic understanding of Semantic Kernel concepts
