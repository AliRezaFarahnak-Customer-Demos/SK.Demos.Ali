global using Microsoft.SemanticKernel;
global using System.ComponentModel;
global using Utilities;
global using Microsoft.Extensions.Configuration;
global using Microsoft.SemanticKernel.ChatCompletion;
global using  Microsoft.SemanticKernel.Connectors.OpenAI;

using SingleAgent.Plugin;


namespace SingleAgent;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101
class Program
{
    private static readonly IConfiguration _cfg = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    private static Kernel _kernel = null!;

    private static OpenAIPromptExecutionSettings _executionSettings => new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        ChatSystemPrompt = $"You are an AI",
        Temperature = 0.1f,
        TopP = 0.1f,
        MaxTokens = 4096
    };

    private static ChatHistory _session;

    static async Task Main(string[] args)
    {
        try
        {
            var builder = Kernel.CreateBuilder()
                                .AddAzureOpenAIChatCompletion(_cfg["DeploymentName"],
                                                              _cfg["EndPoint"],
                                                              _cfg["ApiKey"]);

            _kernel = builder.Build();
            _kernel.ImportPluginFromType<WorldTimePlugin>();
            _kernel.ImportPluginFromType<WorldWeatherPlugin>();

            _session = new ChatHistory();

            @$"Welcome to SingleAgent".Write(ConsoleColor.Yellow, true);
            var chat = _kernel.GetRequiredService<IChatCompletionService>();

            while (true)
            {
                "You: ".Write(ConsoleColor.White);
                var userInput = Console.ReadLine();

                _session.AddUserMessage(userInput);
                await foreach (var content in chat.GetStreamingChatMessageContentsAsync(_session, _executionSettings, _kernel))
                {
                    $"{content.Content}".Write(ConsoleColor.Green);
                }
                Console.WriteLine("\n");

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

