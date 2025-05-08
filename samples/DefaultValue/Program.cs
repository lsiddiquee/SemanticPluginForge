using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticPluginForge;
using Microsoft.Extensions.Hosting;
using SemanticPluginForge.Core;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.AddAzureOpenAIChatCompletion(
    builder.Configuration["AzureOpenAI:ChatDeploymentName"],
    builder.Configuration["AzureOpenAI:Endpoint"],
    builder.Configuration["AzureOpenAI:ApiKey"]
);

builder.Services.AddSingleton<IPluginMetadataProvider, CustomMetadataProvider>();

kernelBuilder.Plugins.AddFromTypeWithMetadata<WeatherPlugin>("WeatherPlugin");

var host = builder.Build();
var kernel = host.Services.GetRequiredService<Kernel>();

var contexts = new List<string> { "UserContext: { \"name\": \"John Doe\", \"location\": \"Utrecht\", \"preferredUnit\": \"celsius\" }",
"UserContext: { \"name\": \"Jane Doe\", \"location\": \"Rotterdam\", \"preferredUnit\": \"celsius\" }",
"UserContext: { \"name\": \"Jan Jansen\", \"preferredUnit\": \"celsius\" }",
};

foreach (var context in contexts)
{
    Console.WriteLine($"Context: {context}");

    var prompt = $"{context}\nWhat is the temperature?";
    var result = await kernel.InvokePromptAsync(prompt, arguments: new KernelArguments(new PromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    }));
    Console.WriteLine(result);
}
// var result = await kernel.InvokePromptAsync(prompt, arguments: new KernelArguments(new PromptExecutionSettings
// {
//     FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
// }));

// Console.WriteLine(result);
