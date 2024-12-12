using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Single.Agent.Plugins;

public class AviationPlugin
{
    [KernelFunction, Description($@"
Get the data the user ask for always add emojis like airplane and flags and also include the airplane and airline in response etc.
You explain if flight is delyaed even 2 minutes, you can do this by compared scheduledtime to estimated time both for arrival and departure.
")]
    public async Task<string> QueryFlightArrivalsAsync([Description($@"
Url to call, Ex. 
arrivals for CPH:
https://aviation-edge.com/v2/public/timetable?key=79a407-f81a02&iataCode=cph&type=arrival
departures for CPH:
https://aviation-edge.com/v2/public/timetable?key=79a407-f81a02&iataCode=CPH&type=departure
or use any https://aviation-edge.com/ url you find feasible with the apikey.
")] string url,
[Description("The search keyword can be a flight identifier or another attribute from a row returned by the API call to https://aviation-edge.com/v2/public/timetable, such as the city or airport of origin. Since the airport is already used in the URL, do not use it as a keyword.")]
string keyword)
    {

        string data = await new HttpClient().GetStringAsync(url);
        var flights = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(data);

        var scopedData = string.Concat(
            flights.Where(flight => ((string)flight.ToString()).Contains(keyword, StringComparison.OrdinalIgnoreCase))
                   .Select(flight => flight.ToString())
        );

        if (scopedData.Length == 0)
            return "Try another search or keyword as we didnt find any results";
        

        return scopedData;
    }
}


/**
What is the current time?
What are the 5 latest flight arrivals at Copenhagen Airport (CPH) today?
Can you show me the 2 flights that have been delayed but have arrived at CPH?
Which 4 airlines have flights arriving at CPH this morning?
What is the status of the next 3 flights arriving at CPH?
Are there any 6 flights arriving at CPH from New York today?
Can you list the 3 flights arriving at CPH with their scheduled and actual arrival times?
What are the first 2 flights arriving at CPH from European cities today?
How many flights are scheduled to arrive at CPH in the next 4 hours?
Can you provide the arrival details for 5 flights coming from London to CPH?
Are there any 3 flights arriving at CPH that have been canceled today? 
*/