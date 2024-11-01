using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MultiAgents;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101
class Program
{
    private const string System = @"You are an AI.";


    private static readonly IConfiguration _cfg = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    private static Kernel _kernel = null!;
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
            _session = new ChatHistory();

            @$"Welcome to SingleAgents".Write(ConsoleColor.Yellow, true);

            while (true)
            {
                "You: ".Write(ConsoleColor.White);

                var chat = _kernel.GetRequiredService<IChatCompletionService>();

                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
                {
                    break;
                }

                _session.AddUserMessage(userInput);
                
                await ProcessChatAsync(chat);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static ChatCompletionAgent CreateAgent(string name, string instructions)
        => new()
        {
            Instructions = instructions,
            Name = name,
            Kernel = _kernel
        };


    private static async Task ProcessChatAsync(IChatCompletionService chat)
    {
        await foreach (var content in chat.GetStreamingChatMessageContentsAsync(_session))
        {
            $"{content.Content}".Write(ConsoleColor.Green);
        }
        Console.WriteLine("\n");
        
    }
}

