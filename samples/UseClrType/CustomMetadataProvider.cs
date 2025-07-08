using Microsoft.SemanticKernel;
using SemanticPluginForge.Core;

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
                "ToLongDateString" => new FunctionMetadata(metadata.Name)
                {
                    Description = "Returns the current date in long format (day of week, month day, year)."
                },
                "CurrentTime" => new FunctionMetadata(metadata.Name)
                {
                    Description = "Returns the current time in HH:MM:SS format."
                },
                "CurrentDateTime" => new FunctionMetadata(metadata.Name)
                {
                    Description = "Returns the current date and time in long format."
                },
                "GetDayOfWeek" => new FunctionMetadata(metadata.Name)
                {
                    Description = "Returns the name of the day of the week for the current date."
                },
                _ => null
            },
            "RandomPlugin" => metadata.Name switch
            {
                "Next" when metadata.Parameters.Count == 0 => new FunctionMetadata(metadata.Name)
                {
                    Description = "Returns a non-negative random integer."
                },
                "Next" when metadata.Parameters.Count == 1 => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "NextWithUpperBound",
                    Description = "Returns a random integer within a specified range.",
                    Parameters = [
                        new ParameterMetadata("maxValue") { Description = "The exclusive upper bound of the random number returned." }
                    ]
                },
                "Next" when metadata.Parameters.Count == 2 => new FunctionMetadata(metadata.Name)
                {
                    OverrideFunctionName = "NextWithRange",
                    Description = "Returns a random integer within a specified range.",
                    Parameters = [
                        new ParameterMetadata("minValue") { Description = "The inclusive lower bound of the random number returned." },
                        new ParameterMetadata("maxValue") { Description = "The exclusive upper bound of the random number returned." }
                    ]
                },
                _ => null
            },
            "Queue" => metadata.Name switch
            {
                "ReceiveMessages" when metadata.Parameters.Count == 2 => new FunctionMetadata(metadata.Name)
                {
                    Description = "Retrieves messages from the Azure Storage Queue.",
                    Parameters = new List<ParameterMetadata>
                    {
                        new ParameterMetadata("maxMessages") { Description = "The maximum number of messages to retrieve.", DefaultValue = "5", IsRequired = false },
                        new ParameterMetadata("visibilityTimeout") { Description = "Specifies the new visibility timeout value, in seconds, relative to server time", DefaultValue = "60", IsRequired = false },
                    },
                },
                "PeekMessages" when metadata.Parameters.Count == 1 => new FunctionMetadata(metadata.Name)
                {
                    Description = "Peeks at messages in the Azure Storage Queue without removing them.",
                    Parameters = new List<ParameterMetadata>
                    {
                        new ParameterMetadata("maxMessages") { Description = "The maximum number of messages to peek.", DefaultValue = "5", IsRequired = false },
                    },
                },
                "SendMessage" when metadata.Parameters.Count == 1 => new FunctionMetadata(metadata.Name)
                {
                    Description = "Sends a message to the Azure Storage Queue.",
                    Parameters = new List<ParameterMetadata>
                    {
                        new ParameterMetadata("messageText") { Description = "The message to send." },
                    },
                },
                _ => null
            },
            _ => null,
        };
    }
}
