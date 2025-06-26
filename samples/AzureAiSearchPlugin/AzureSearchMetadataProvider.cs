using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

/// <summary>
/// Custom metadata provider for Azure AI Search plugins with different configurations for each data source
/// </summary>
public class AzureSearchMetadataProvider : IPluginMetadataProvider
{
    public FunctionMetadata? GetFunctionMetadata(KernelPlugin plugin, KernelFunctionMetadata metadata) =>
        plugin.Name switch
        {
            // Product Search Plugin Instance
            "ProductSearch" => metadata.Name == "Search" ? new FunctionMetadata(metadata.Name)
            {
                Description = "Search for products in an e-commerce catalog. Use this when users want to find products to buy, compare items, or get shopping recommendations.",
                Parameters = [
                    new ParameterMetadata("query")
                    {
                        Description = "The search query text describing what product the user wants to find (e.g., 'laptop', 'running shoes', 'wireless headphones'). Extract this from the user's natural language request about products they want to purchase.",
                        IsRequired = true,
                    },
                    new ParameterMetadata("maxResults")
                    {
                        Description = "Maximum number of products to return. Default is 10 for product searches.",
                        IsRequired = false,
                        DefaultValue = 10,
                    }
                ],
                ReturnParameter = new ReturnParameterMetadata
                {
                    Description = "A formatted list of products matching the search criteria, including product names, prices, categories, ratings, and descriptions."
                },
            }
            : null,

            // Knowledge Base Search Plugin Instance  
            "KnowledgeBaseSearch" => metadata.Name == "Search" ? new FunctionMetadata(metadata.Name)
            {
                Description = "Search for knowledge base articles, documentation, and help content. Use this when users have questions that need answers from documentation, need help, or are looking for tutorials and guides.",
                Parameters = [
                    new ParameterMetadata("query")
                    {
                        Description = "The user's question or search query to find relevant knowledge base articles. Convert natural language questions into effective search terms for finding help documentation.",
                        IsRequired = true,
                    },
                    new ParameterMetadata("maxResults")
                    {
                        Description = "Maximum number of articles to return. Default is 5 for focused knowledge base results.",
                        IsRequired = false,
                        DefaultValue = 5,
                    }
                ],
                ReturnParameter = new ReturnParameterMetadata
                {
                    Description = "A formatted list of knowledge base articles with titles, content, document types, topics, URLs, relevance scores, and other helpful information."
                },
            }
            : null,
            _ => null,
        };

    public PluginMetadata? GetPluginMetadata(KernelPlugin plugin) =>
        plugin.Name switch
        {
            "ProductSearch" => new PluginMetadata 
            { 
                Description = "E-commerce product search system. Use this plugin when users want to find products to purchase, compare items, or get product recommendations. This searches through a product catalog with commercial items for sale." 
            },
            "KnowledgeBaseSearch" => new PluginMetadata 
            { 
                Description = "Knowledge base and documentation search system. Use this plugin when users have questions, need help documentation, want to find tutorials, or are looking for answers to problems. This searches through help articles, FAQs, guides, and reference materials." 
            },
            _ => null,
        };
}
