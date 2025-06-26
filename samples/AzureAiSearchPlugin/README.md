# Azure AI Search Plugin Sample

This sample demonstrates how to build Semantic Kernel plugins using **one plugin class with two different instances**, each having distinct metadata configurations for different data sources. The sample uses **mocked data** to showcase the concepts without requiring an actual Azure AI Search service:

1. **ProductSearch** - Plugin instance for searching e-commerce product catalogs  
2. **KnowledgeBaseSearch** - Plugin instance for searching knowledge base articles and documentation

> **Note**: This sample uses built-in mocked data to demonstrate the plugin concepts. The plugin architecture is designed to easily integrate with actual Azure AI Search services by replacing the mock data service with real Azure AI Search client calls.

## Key Features

- **Single Plugin Class**: One `AzureSearchPlugin` class that performs simple full-text search
- **Multiple Plugin Instances**: Two instances of the same class targeting different data sources
- **Different Metadata Per Instance**: Sophisticated metadata configuration that provides different parameter descriptions and behaviors for each plugin instance
- **Intelligent Function Selection**: The metadata provider helps the LLM choose the right plugin instance based on user intent (commercial vs. help-seeking)
- **Mocked Data**: Built-in sample data demonstrates concepts without requiring external services
- **Production-Ready Architecture**: Easily adaptable to connect with actual Azure AI Search instances

## Architecture

### AzureSearchPlugin Class

- **Single Responsibility**: Simple full-text search using only the `query` parameter
- **Reusable**: Same class used for different data sources
- **Generic**: Works with any data source (currently mocked, easily adaptable to Azure AI Search)

### Two Plugin Instances

#### ProductSearch Instance

- **Data Source**: Mocked e-commerce product catalog data
- **Plugin Name**: "ProductSearch"
- **Intent**: Commercial searches for products to purchase
- **Default Results**: 10 (more products for comparison)

#### KnowledgeBaseSearch Instance

- **Data Source**: Mocked knowledge base and documentation data
- **Plugin Name**: "KnowledgeBaseSearch"  
- **Intent**: Help-seeking searches for information and answers
- **Default Results**: 5 (focused help content)

### Custom Metadata Provider

The `AzureSearchMetadataProvider` provides different metadata based on plugin name:

- **ProductSearch**: Descriptions focus on commercial intent, product finding, shopping
- **KnowledgeBaseSearch**: Descriptions focus on help-seeking, documentation, problem-solving
- **Different Defaults**: Different default result counts for each use case

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Azure OpenAI service with a chat deployment

### Configuration

Navigate to the project folder and set up user secrets for Azure OpenAI:

```console
cd samples\AzureAiSearchPlugin
dotnet user-secrets init
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "YOUR_DEPLOYMENT_NAME"
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR_ENDPOINT.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY"
```

### Run the Sample

```console
dotnet run
```

## Built-in Sample Data

The sample includes built-in mocked data to demonstrate the concepts:

### Product Data

The sample includes products like:

- Gaming laptops and electronics
- Computer peripherals
- Software and accessories

### Knowledge Base Data

The sample includes articles like:

- API integration guides
- Troubleshooting documentation
- FAQ entries
- How-to tutorials

This mocked data simulates what you would typically find in separate Azure AI Search indexes.

## How It Works

### Metadata-Driven Plugin Selection

The sample demonstrates how different metadata configurations guide the LLM to select appropriate plugin instances:

1. **Product-Related Queries**:
   - "Looking for laptops" → Uses `ProductSearch.Search`
   - "Show me gaming accessories" → Uses `ProductSearch.Search`

2. **Knowledge-Seeking Queries**:
   - "How do I integrate with the API?" → Uses `KnowledgeBaseSearch.Search`
   - "Billing questions" → Uses `KnowledgeBaseSearch.Search`

### Same Function, Different Context

Both plugin instances use the same `Search` function but with different metadata:

- **ProductSearch**: Emphasizes commercial intent, product finding, shopping context
- **KnowledgeBaseSearch**: Emphasizes help-seeking, documentation, problem-solving context

### Error Handling

The sample includes comprehensive error handling for common scenarios:

- Search query processing issues
- Data retrieval failures
- Invalid search parameters

## Extending the Sample

You can extend this sample by:

1. **Connecting to Azure AI Search**: Replace the mocked data service with actual Azure AI Search client integration
2. **Adding More Data Sources**: Create additional instances for different search indexes or data sources
3. **Enhanced Metadata**: Add more sophisticated parameter descriptions and defaults
4. **Custom Result Formatting**: Modify the plugin to format results differently based on data source
5. **Semantic Search**: Implement vector search capabilities when using Azure AI Search
6. **Caching**: Add result caching for improved performance

## Connecting to Actual Azure AI Search

To connect this sample to a real Azure AI Search service:

1. **Install Azure AI Search SDK**: Add the `Azure.Search.Documents` NuGet package
2. **Update Configuration**: Add Azure AI Search connection settings
3. **Replace Mock Service**: Swap the mocked data service with `SearchClient` calls
4. **Configure Indexes**: Set up your search indexes with appropriate schemas

The plugin architecture is designed to make this transition seamless - only the data access layer needs to change.

## Learning Objectives

This sample demonstrates:

- How to create multiple instances of the same plugin class for different data sources
- Custom metadata provider patterns for giving the same function different behavior
- Context-aware function selection through metadata descriptions
- Simple, focused plugin design with single responsibility
- Mocked data patterns for demonstrating concepts without external dependencies
- Architecture that easily scales to production scenarios with real data sources
