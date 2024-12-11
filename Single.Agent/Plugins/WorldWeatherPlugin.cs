using System.ComponentModel;

namespace Single.Agent.Plugins;

public class WorldWeatherPlugin 
{
    [KernelFunction, Description(@$"Describe the time for the location and weather for each day line by line, including the temperature in Celsius and any other available details.  
                                     Each line should conclude with an emoji that best represents the weather for that day.")]
    public async Task<string> WeatherAsync(
        [Description("Latitude of location you calculate this based on location")] string latitude,
        [Description("Longitude of location you calculate this based on location")] string longitude)
    {
        var httpClient = new HttpClient();
        $@"Getting weather for location {latitude}, {longitude}".Write(ConsoleColor.Red, true);
        string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,weathercode&timezone=auto";
        string weatherJson = await httpClient.GetStringAsync(url);
        return weatherJson;
    }

}
