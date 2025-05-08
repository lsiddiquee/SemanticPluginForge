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

// var targetObject = new ShortDate();
// kernelBuilder.Plugins.AddFromClrObjectWithMetadata(targetObject, "ShortDatePlugin");
kernelBuilder.Plugins.AddFromClrTypeWithMetadata<ShortDate>("ShortDatePlugin");

var host = builder.Build();
var kernel = host.Services.GetRequiredService<Kernel>();

var result = await kernel.InvokePromptAsync("What is the date in short format?", arguments: new KernelArguments(new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
}));

Console.WriteLine(result);
