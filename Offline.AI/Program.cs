using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SixLabors.ImageSharp; // Make sure to add ImageSharp as a dependency  
using SixLabors.ImageSharp.Formats;

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

            // Load the image into a byte array  
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dog.jpg");
            byte[] imageData;
            string mimeType;

            using (var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                mimeType = GetMimeTypeFromStream(imageStream);
                imageData = new byte[imageStream.Length];
                imageStream.Read(imageData, 0, imageData.Length);
            }

            while (true)
            {
                Console.Write("\nYour question: ");
                var question = Console.ReadLine();
                if (string.IsNullOrEmpty(question)) break;

                // Add the image data and question to the chat history  
                chatHistory.AddUserMessage([
                
                    new ImageContent(imageData, mimeType),
                    new TextContent(question)
                ]);

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