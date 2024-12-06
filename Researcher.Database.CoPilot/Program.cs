using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Utilities;

namespace Researcher.Insights;

class Program
{
    private static OpenAIPromptExecutionSettings _executionSettings => new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        ChatSystemPrompt = "You answer questions from databses.",
        Temperature = 0.3f,
        TopP = 0.3f,
        MaxTokens = 4096
    };

    static async Task Main(string[] args)
    {
        @$"Database CoPilot".Write(ConsoleColor.Yellow, true);

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(Constants.Gpt4o,Constants.Endpoint, Constants.ApiKey);
        Kernel kernel = kernelBuilder.Build();
        kernel.ImportPluginFromType<UsagePlugin>();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        kernel.AutoFunctionInvocationFilters.Add(new KernelExecutionFilter());
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


        Database.LoadRecords();

        while (true)
        {
            string user = Console.ReadLine();
            if (user.ToLower() == "exit")
                break;
            await CompletionAsync(user, kernel);
        }
    }

    public static async Task CompletionAsync(string user, Kernel kernel)
    {
        ChatHistory session = [];
        session.AddUserMessage(user);
        IAsyncEnumerable<StreamingChatMessageContent> responseStream = kernel.GetRequiredService<IChatCompletionService>()
            .GetStreamingChatMessageContentsAsync(session, _executionSettings, kernel);

        string completeResponse = "";
        await foreach (var part in responseStream)
        {
            string content = part.Content;
            completeResponse += content;
            content.Write();
        }
        Console.WriteLine(Environment.NewLine);
    }
}