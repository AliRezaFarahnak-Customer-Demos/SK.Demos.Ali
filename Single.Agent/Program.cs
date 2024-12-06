global using Microsoft.SemanticKernel;
global using Microsoft.SemanticKernel.ChatCompletion;
global using Microsoft.SemanticKernel.Connectors.OpenAI;
global using Utilities;
using Microsoft.Extensions.DependencyInjection;
using Single.Agent.Plugins;

namespace SingleAgent;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101

class Program
{

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
            IKernelBuilder builder = Kernel.CreateBuilder()
                                .AddAzureOpenAIChatCompletion(Constants.Gpt4omini,
                                                              Constants.Endpoint,
                                                              Constants.ApiKey);

#pragma warning disable SKEXP0001
            builder.Services.AddSingleton<IAutoFunctionInvocationFilter, InvocationFilter>();
#pragma warning restore SKEXP0001


            _kernel = builder.Build();
            _kernel.ImportPluginFromType<WorldTimePlugin>();
            _kernel.ImportPluginFromType<WorldWeatherPlugin>();
            _kernel.ImportPluginFromType<OpenSkyPlugin>();
            _kernel.ImportPluginFromType<ImagePlugin>();
            _kernel.ImportPluginFromType<CurrencyConverterPlugin>();

            _session = new ChatHistory();

            @$"Welcome to SingleAgent".Write(ConsoleColor.Yellow, true);
            var chat = _kernel.GetRequiredService<IChatCompletionService>();
            var completeResponse = string.Empty;

            while (true)
            {
                "You: ".Write(ConsoleColor.White);
                var userInput = Console.ReadLine();

                if (userInput == "clear")
                {
                    _session.Clear();
                    "Session cleared".Write(ConsoleColor.Red, true);
                    Console.WriteLine();
                    Task.Delay(500).Wait();
                    Console.Clear();
                    @$"Welcome to SingleAgent".Write(ConsoleColor.Yellow, true);
                    continue;
                }

                _session.AddUserMessage(userInput);
                await foreach (var content in chat.GetStreamingChatMessageContentsAsync(_session, _executionSettings, _kernel))
                {
                    $"{content.Content}".Write(ConsoleColor.Green);
                    completeResponse += content.Content;
                }
                _session.AddAssistantMessage(completeResponse);
                _session.ScopeSession(true);
                Console.WriteLine();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

