using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticPluginForge;
using Microsoft.Extensions.Hosting;
using SemanticPluginForge.Core;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Azure.Storage.Queues;
using Azure.Identity;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.AddAzureOpenAIChatCompletion(
    builder.Configuration["AzureOpenAI:ChatDeploymentName"]!,
    builder.Configuration["AzureOpenAI:Endpoint"]!,
    builder.Configuration["AzureOpenAI:ApiKey"]!
);

builder.Services.AddSingleton<IPluginMetadataProvider, CustomMetadataProvider>();

// Add DateTimeWrapper for date/time operations
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<DateTimeWrapper>("DateTimeWrapper");

// Add Random for random number generation
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<Random>("RandomPlugin");

// Add queue client for Azure Storage Queue operations
var qc = new QueueClient(new Uri(builder.Configuration["AzureQueueUri"]!), new DefaultAzureCredential());
kernelBuilder.Plugins.AddFromClrObjectWithMetadata(qc, "Queue");

var host = builder.Build();
var kernel = host.Services.GetRequiredService<Kernel>();
kernel.AutoFunctionInvocationFilters.Add(new FunctionLogger());

while (true)
{
    Console.Write("User: ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase) || input.Equals("quit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Exiting...");
        break;
    }

    try
    {
        var response = await kernel.InvokePromptAsync(input, arguments: new KernelArguments(new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        }));
        Console.WriteLine($"Bot: {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}