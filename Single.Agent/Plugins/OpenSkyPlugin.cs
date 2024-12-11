using System.ComponentModel;
namespace Single.Agent.Plugins;

public class OpenSkyPlugin : BasePlugin
{

    [KernelFunction, Description("Retrieves flights for a certain time interval (anonymous user).")]
    public async Task<string> GetFlightsInTimeIntervalAsync(
        [Description("Ask user about this by getting Current Time First,Start of time interval to retrieve flights for as Unix time (seconds since epoch).")] int begin,
        [Description("Ask user about this by getting Current Time First, End of time interval to retrieve flights for as Unix time (seconds since epoch).")] int end
    )
    {
        var url = $"https://opensky-network.org/api/flights/all?begin={begin}&end={end}";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }


    [KernelFunction, Description("Ask user about this by getting Current Time First, Retrieves flights for a certain airport which arrived within a given time interval (anonymous user).")]
    public async Task<string> GetArrivalsByAirportAsync(
        [Description("ICAO identifier for the airport.")] string airport,
        [Description("Start of time interval to retrieve flights for as Unix time (seconds since epoch).")] int begin,
        [Description("End of time interval to retrieve flights for as Unix time (seconds since epoch).")] int end
    )
    {
        try
        {
            var url = $"https://opensky-network.org/api/flights/arrival?airport={airport}&begin={begin}&end={end}";

            string responseJson = await httpClient.GetStringAsync(url);
            return responseJson;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }


    [KernelFunction, Description("Ask user about this by getting Current Time First,Retrieves flights for a certain airport which departed within a given time interval (anonymous user).")]
    public async Task<string> GetDeparturesByAirportAsync(
        [Description("ICAO identifier for the airport.")] string airport,
        [Description("Start of time interval to retrieve flights for as Unix time (seconds since epoch).")] int begin,
        [Description("End of time interval to retrieve flights for as Unix time (seconds since epoch).")] int end
    )
    {
        var url = $"https://opensky-network.org/api/flights/departure?airport={airport}&begin={begin}&end={end}";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }

}