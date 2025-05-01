using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticPluginForge.UnitTests;

[Description("Sample plugin for testing.")]
public class SamplePlugin
{
    [KernelFunction, Description("Returns the current username.")]
    public string GetCurrentUsername() => "my_user";

    [KernelFunction, Description("Returns all the cities based on the filter provided.")]
    public List<string> GetAllCities([Description("Optional filter parameter which can be used to get a subset of cities")]string? filter = null)
    {
        var users = new List<string> { "amsterdam", "rotterdam", "london", "paris", "berlin" };

        if (filter != null)
        {
            users = users.Where(u => u.Contains(filter)).ToList();
        }

        return users;
    }

    [KernelFunction, Description("Returns the current temperature of the city.")]
    public string GetTemperatureByCity(
        [Description("The name of the city")]string name,
        [Description("The temperature unit, default is celsius")]string unit = "celsius"
    )
    {
        return $"{name} temperature is 20 degrees {unit}";
    }
}
