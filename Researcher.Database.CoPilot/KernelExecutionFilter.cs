using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Researcher.Insights;


#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public class KernelExecutionFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        // Example: get function information
        var functionName = context.Function.Name;

        // Example: get chat history
        var chatHistory = context.ChatHistory;

        // Example: get information about all functions which will be invoked
        var functionCalls = FunctionCallContent.GetFunctionCalls(context.ChatHistory.Last());

        // Calling next filter in pipeline or function itself.
        // By skipping this call, next filters and function won't be invoked, and function call loop will proceed to the next function.
        await next(context);

        // Example: get function result
        var result = context.Result;

        // check if result is json 
        // Check if result is JSON using regex
        if (Regex.IsMatch(context.Result.GetValue<string>(), @"\{.*\}"))
            await VisualizeJSONAsync(context.Result.GetValue<string>(), context.Kernel);
    }

    public async static Task VisualizeJSONAsync(string json, Kernel kernel)
    {
        try
        {
            ChatHistory messages = new ChatHistory();
            messages.AddUserMessage(@$"Convert this usage data to be presented as a echart in a html file, give me the complete HTML so it can be displayed, also add a small script that reloads the page every 10 seconds.
                                       If it doesn't make sense to display it as a echart, just give me the HTML and message that says that the data is not feasible for a visual chart, make piechart as first priority if possible.
                                       Json: {json}");

            var response = await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(messages);
            var content = response.Content;

            // Extract HTML content using regex  
            string pattern = @"<html[\s\S]*<\/html>";
            Match match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
            string htmlContent = match.Value;

            // Save the content to an HTML file named "echart.html" three directories up from the current executable  
            SaveHtmlToFile(htmlContent, "echart.html");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while converting the data to an EChart: {ex.Message}");
        }
    }

    private static void SaveHtmlToFile(string htmlContent, string fileName)
    {
        try
        {
            // Get the directory of the current executable  
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

            // Navigate three directories up from the current executable directory  
            string projectDirectory = Path.GetFullPath(Path.Combine(directoryPath, @"..\..\.."));
            string filePath = Path.Combine(projectDirectory, fileName);

            // Write the HTML content to the file  
            File.WriteAllText(filePath, htmlContent);

            // Open the HTML file with the default web browser
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving the file: {ex.Message}");
        }
    }
}

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.