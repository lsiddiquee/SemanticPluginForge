using Microsoft.SemanticKernel;
using System.ComponentModel;

[Description("Sample plugin for testing.")]
public class WeatherPlugin
{
    [KernelFunction, Description("Returns the current temperature of the city.")]
    public string GetTemperatureByCity(
        [Description("The name of the city")] string name,
        [Description("The temperature unit, default is celsius")] string unit = "celsius"
    )
    {
        Console.WriteLine($"Getting temperature for {name} in {unit}.");
        return $"{name} temperature is 20 degrees {unit}";
    }
}
