using System.ComponentModel;
namespace Single.Agent.Plugins;

public class OpenSkyPlugin : BasePlugin
{
    [KernelFunction, Description("Retrieves most recent state vectors for all aircraft (anonymous user).")]
    public async Task<string> GetAllStateVectorsAsync(
        [Description("One or more ICAO24 transponder addresses represented by a hex string (e.g., abc9f3). To filter multiple ICAO24 append the property once for each address.")] List<string> icao24 = null,
        [Description("Lower bound for the latitude in decimal degrees.")] float? lamin = null,
        [Description("Lower bound for the longitude in decimal degrees.")] float? lomin = null,
        [Description("Upper bound for the latitude in decimal degrees.")] float? lamax = null,
        [Description("Upper bound for the longitude in decimal degrees.")] float? lomax = null,
        [Description("Set to 1 if the extended category of aircraft is required.")] int? extended = null
    )
    {
        var url = "https://opensky-network.org/api/states/all?";
        if (icao24 != null) foreach (var id in icao24) url += $"icao24={id}&";
        if (lamin.HasValue) url += $"lamin={lamin.Value}&";
        if (lomin.HasValue) url += $"lomin={lomin.Value}&";
        if (lamax.HasValue) url += $"lamax={lamax.Value}&";
        if (lomax.HasValue) url += $"lomax={lomax.Value}&";
        if (extended.HasValue) url += $"extended={extended.Value}&";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }

    [KernelFunction, Description("Retrieves flights for a certain time interval (anonymous user).")]
    public async Task<string> GetFlightsInTimeIntervalAsync(
        [Description("Start of time interval to retrieve flights for as Unix time (seconds since epoch).")] int begin,
        [Description("End of time interval to retrieve flights for as Unix time (seconds since epoch).")] int end
    )
    {
        var url = $"https://opensky-network.org/api/flights/all?begin={begin}&end={end}";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }

    [KernelFunction, Description("Retrieves flights for a particular aircraft within a certain time interval (anonymous user).")]
    public async Task<string> GetFlightsByAircraftAsync(
        [Description("Unique ICAO 24-bit address of the transponder in hex string representation.")] string icao24,
        [Description("Start of time interval to retrieve flights for as Unix time (seconds since epoch).")] int begin,
        [Description("End of time interval to retrieve flights for as Unix time (seconds since epoch).")] int end
    )
    {
        var url = $"https://opensky-network.org/api/flights/aircraft?icao24={icao24}&begin={begin}&end={end}";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }

    [KernelFunction, Description("Retrieves flights for a certain airport which arrived within a given time interval (anonymous user).")]
    public async Task<string> GetArrivalsByAirportAsync(
        [Description("ICAO identifier for the airport.")] string airport,
        [Description("Start of time interval to retrieve flights for as Unix time (seconds since epoch).")] int begin,
        [Description("End of time interval to retrieve flights for as Unix time (seconds since epoch).")] int end
    )
    {
        var url = $"https://opensky-network.org/api/flights/arrival?airport={airport}&begin={begin}&end={end}";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }

    [KernelFunction, Description("Retrieves flights for a certain airport which departed within a given time interval (anonymous user).")]
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

    [KernelFunction, Description("Retrieves the trajectory for a certain aircraft at a given time (anonymous user).")]
    public async Task<string> GetTrackByAircraftAsync(
        [Description("Unique ICAO 24-bit address of the transponder in hex string representation.")] string icao24,
        [Description("Unix time in seconds since epoch. It can be any time between start and end of a known flight. If time = 0, get the live track if there is any flight ongoing for the given aircraft.")] int time
    )
    {
        var url = $"https://opensky-network.org/api/tracks/all?icao24={icao24}&time={time}";

        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }
}