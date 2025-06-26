using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticPluginForge;
using Microsoft.Extensions.Hosting;
using SemanticPluginForge.Core;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

// Register the custom metadata provider
builder.Services.AddSingleton<IPluginMetadataProvider, AzureSearchMetadataProvider>();

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

// Configure Semantic Kernel
var kernelBuilder = builder.Services.AddKernel();

// Create the plugin instances
var productSearchPlugin = new AzureSearchPlugin(SearchType.Product);
var knowledgeBaseSearchPlugin = new AzureSearchPlugin(SearchType.KnowledgeBase);

kernelBuilder.AddAzureOpenAIChatCompletion(
    builder.Configuration["AzureOpenAI:ChatDeploymentName"]!,
    builder.Configuration["AzureOpenAI:Endpoint"]!,
    builder.Configuration["AzureOpenAI:ApiKey"]!
);
kernelBuilder.Plugins.AddFromObjectWithMetadata(productSearchPlugin, "ProductSearch");
kernelBuilder.Plugins.AddFromObjectWithMetadata(knowledgeBaseSearchPlugin, "KnowledgeBaseSearch");

var host = builder.Build();

var kernel = host.Services.GetRequiredService<Kernel>();
// kernel.FunctionInvocationFilters.Add(new FunctionLogger());
kernel.AutoFunctionInvocationFilters.Add(new FunctionLogger());

// Add plugins with different names to get different metadata

Console.WriteLine("Azure AI Search Plugin Demo");
Console.WriteLine("===========================");
Console.WriteLine("This demo shows how one plugin class can have different metadata for different data sources.");
Console.WriteLine();

// Demo scenarios
var scenarios = new List<(string Description, string Prompt)>
{
    (
        "Product Search - Finding laptops",
        "I'm looking for a laptop for college. Can you help me find some options?"
    ),
    (
        "Knowledge Base Search - API integration question", 
        "How do I integrate with your REST API? I need documentation on authentication."
    ),
    (
        "Product Search - Gaming accessories",
        "I want to find gaming keyboards and mice for my setup."
    ),
    (
        "Knowledge Base Search - FAQ about billing",
        "I have questions about billing and payment methods. Can you help?"
    ),
    (
        "Product Search - Wireless headphones",
        "Show me some wireless headphones for working out."
    ),
    (
        "Knowledge Base Search - Recent tutorials",
        "I need tutorials about Azure integration. Can you find some guides?"
    )
};

foreach (var (description, prompt) in scenarios)
{
    Console.WriteLine($"Scenario: {description}");
    Console.WriteLine($"User Query: {prompt}");
    Console.WriteLine("Response:");
    Console.WriteLine(new string('-', 50));

    try
    {
        var result = await kernel.InvokePromptAsync(prompt, arguments: new KernelArguments(new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        }));

        Console.WriteLine(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        
        // If it's a search service error, provide helpful information
        if (ex.Message.Contains("search") || ex.Message.Contains("index"))
        {
            Console.WriteLine("\nNote: This demo requires Azure AI Search to be configured.");
            Console.WriteLine("Please ensure your search service and indexes are set up with sample data.");
        }
    }

    Console.WriteLine();
    Console.WriteLine(new string('=', 80));
    Console.WriteLine();
    
    // Add a small delay between scenarios for better readability
    await Task.Delay(1000);
}

Console.WriteLine("Demo completed! Press any key to exit...");
// Console.ReadKey();
