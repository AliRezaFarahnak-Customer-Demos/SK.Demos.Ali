using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SixLabors.ImageSharp; // Make sure to add ImageSharp as a dependency  
using SixLabors.ImageSharp.Formats;
using static System.Net.Mime.MediaTypeNames;

namespace Offline.AI
{
    class Program
    {
        private static Kernel kernel;

        public static OpenAIPromptExecutionSettings ExecutionSettings => new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.1f,
            TopP = 0.3f,
            MaxTokens = 4096
        };

        static async Task Main(string[] args)
        {
            kernel = Kernel.CreateBuilder()
                .AddHuggingFaceChatCompletion("microsoft/Phi-3.5-vision-instruct", new Uri("http://localhost:8000"))
                .Build();

            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            var chatHistory = new ChatHistory();

            // Create an ImageContent object  
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Waldo.jpg");
            var imageUri = new Uri(imagePath, UriKind.Absolute);

            while (true)
            {
                Console.Write("\nYour question: ");
                var question = Console.ReadLine();
                if (string.IsNullOrEmpty(question)) break;

                    // Add the image and question to the chat history  
                    chatHistory.AddUserMessage([
                                                    new ImageContent(imageUri),
                                                    new TextContent(question)
                                                ]);

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

        private static string GetMimeTypeFromStream(Stream stream)
        {
            IImageFormat format = SixLabors.ImageSharp.Image.DetectFormat(stream);
            return format?.DefaultMimeType ?? "application/octet-stream"; // Fallback if not detected  
        }
    }
}