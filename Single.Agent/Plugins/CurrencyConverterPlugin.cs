using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Single.Agent.Plugins;

public class CurrencyConverterPlugin 
{

    [KernelFunction, Description("Convert currency from one type to another. When you answer this then there should be a emoji flag for each currency in the resulting answer.")]
    public async Task<string> ConvertCurrencyAsync(
        [Description("Amount to convert")] decimal amount,
        [Description("Currency code to convert to")] string toCurrency)
    {
        var httpClient = new HttpClient();  
        string url = $"https://open.er-api.com/v6/latest/{toCurrency}";
        string responseJson = await httpClient.GetStringAsync(url);
        return responseJson;
    }

}
