using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Offline.AI;
// sudo docker run --runtime nvidia --gpus all --name Phi-3.5-vision-instruct -v ~/.cache/huggingface:/root/.cache/huggingface -p 8000:8000 --ipc=host vllm/vllm-openai:latest --model microsoft/Phi-3.5-vision-instruct --gpu_memory_utilization=0.99 --max_model_len=4000 --trust-remote-code 
class Program
{
    private static Kernel kernel;
    public static OpenAIPromptExecutionSettings ExecutionSettings => new OpenAIPromptExecutionSettings
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        Temperature = 0.1f,
        TopP = 0.1f,
        MaxTokens = 4096
    };

    static async Task Main(string[] args)
    {
        kernel = Kernel.CreateBuilder()
                   .AddHuggingFaceChatCompletion("microsoft/Phi-3.5-vision-instruct", new Uri("http://localhost:8000"))
              .Build();

        kernel.ImportPluginFromType<WorldWeatherPlugin>(); 

        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        var chatHistory = new ChatHistory();

        while (true)
        {
            Console.Write("\nYour question: ");
            var question = Console.ReadLine();
            if (string.IsNullOrEmpty(question)) break;

            chatHistory.AddUserMessage(question);

            string completeResponse = string.Empty;

            try
            {

                await foreach (var responsePart in chatService.GetStreamingChatMessageContentsAsync(chatHistory, ExecutionSettings, kernel))
                {
                    var botResponsePart = responsePart?.Content;
                    if (botResponsePart != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(botResponsePart);
                        Console.ResetColor();
                        completeResponse += botResponsePart;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
            }

            Console.WriteLine();
            chatHistory.AddAssistantMessage(completeResponse);
        }
    }


}



#pragma warning restore SKEXP0070