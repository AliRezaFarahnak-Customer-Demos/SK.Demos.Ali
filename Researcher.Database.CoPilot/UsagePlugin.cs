using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using Utilities;

namespace Researcher.Insights;

public class UsagePlugin
{
    [KernelFunction, Description(@"
        Answers queries on the usage data, with minimal amount of data returned by the CodeQuery. 
        CodeQuery should make query and group by to minimize the data returned as much as possible
        Usage records is accessible like this: 
        Database.Records; 
        Database class looks like this:
        public static class Database { public static List<ChatProcessed> Records;}
        public record struct ChatProcessed(DateTime Timestamp, string Country);")]
    public async Task<string> GetUsageDataQueryAnswerAsync([Description(@$"CSharpScript.EvaluateAsync({nameof(CodeQuery)})")] string CodeQuery)
    {
        CodeQuery.Write(ConsoleColor.DarkYellow,true);
        string result = await DynamicQuery(CodeQuery);
        result.Write(ConsoleColor.Yellow, true);
        Console.WriteLine();
        return result;
    }

    protected async Task<string> DynamicQuery(string codeQuery)
    {
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            var result = await CSharpScript.EvaluateAsync<dynamic>(
                codeQuery,
                ScriptOptions.Default
                    .WithReferences(assemblies)
                    .WithImports("System",
                                 "System.Linq",
                                 "System.Collections.Generic",
                                 $@"{nameof(Researcher)}.{nameof(Insights)}"
            ));

            var resultString = JsonSerializer.Serialize(result).ToString();

            return resultString;
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
                ex.Message.Write(ConsoleColor.Red, true);

            return ex.Message;
        }
    }
}

/**
    Which dates do you have data from?
    Show me top 10 countries by usage?
    Show me top 5 countries by usage in Asia?
    Show me top 3 european countries by usage?
    Which 2 weekddays have the highest number of usage?
    Give me 3 countries that use it the lowest in the morning hours from 8 to 12?
    Give me 3 countries that use it the lowest in the morning hours from 8 to 12 in Asia?
    What is the peak day and hour on the day for highest usage?
 * */
