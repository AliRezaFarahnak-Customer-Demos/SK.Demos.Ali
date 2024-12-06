global using Microsoft.SemanticKernel;
global using Microsoft.SemanticKernel.ChatCompletion;
global using Microsoft.SemanticKernel.Connectors.OpenAI;
global using Utilities;
using GitHub.Manager.CoPilot;
using GitHubCoPilotExtreme;



namespace GitHub.Manager.CoPilot;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101
class Program
{
    private static Kernel _kernel = null!;

    private static OpenAIPromptExecutionSettings _executionSettings => new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        ChatSystemPrompt = @$"You are the GitHub admin with knowledge on: GitHub Rest API (https://docs.github.com/en/rest/meta/meta?apiVersion=2022-11-28)",
        Temperature = 0.2f, 
        TopP = 0.5f,        
        MaxTokens = 4096    
    };

    private static ChatHistory _session;

    static async Task Main(string[] args)
    {
        try
        {
            IKernelBuilder builder = Kernel.CreateBuilder()
                                .AddAzureOpenAIChatCompletion(Constants.Gpt4o,
                                                              Constants.Endpoint,
                                                              Constants.ApiKey);

#pragma warning disable SKEXP0001
          //  builder.Services.AddSingleton<IAutoFunctionInvocationFilter, InvocationFilter>();
#pragma warning restore SKEXP0001

            _kernel = builder.Build();
            _kernel.ImportPluginFromType<GitHubPlugin>();

            _session = new ChatHistory();

            @$"GitHub Manager CoPilot".Write(ConsoleColor.Yellow, true);
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
                    @$"GitHub Manager CoPilot".Write(ConsoleColor.Yellow, true);
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

