using System.ComponentModel;
namespace Single.Agent.Plugins;

public class WorldTimePlugin
{
    [KernelFunction, Description("Returns the current time and year for the location and append a emoji depend on if it is day or night.")]
    public string GetCurrentTime([Description("The location to get the current time for.")] string location)
    {
        var dateTime = $@"This is time UTC: {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}, Now calculate time for {location}";
        return dateTime;
    }
}
