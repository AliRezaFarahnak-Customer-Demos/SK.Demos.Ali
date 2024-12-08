using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            // Get all image files in the app directory  
            var imageFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                               file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            while (true)
            {
                // List available images  
                Console.WriteLine("Available images:");
                for (int i = 0; i < imageFiles.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {Path.GetFileName(imageFiles[i])}");
                }

                Console.Write("Select an image by number (or press Enter to exit): ");
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) break;

                if (!int.TryParse(input, out int imageIndex) || imageIndex < 1 || imageIndex > imageFiles.Length)
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    continue;
                }

                string imagePath = imageFiles[imageIndex - 1];
                byte[] imageData;
                string mimeType;

                var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

                mimeType = GetMimeTypeFromStream(imageStream);
                imageData = new byte[imageStream.Length];
                imageStream.Read(imageData, 0, imageData.Length);


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
                    Console.ResetColor();
                }

                Console.WriteLine();
                chatHistory.AddAssistantMessage(completeResponse);
                chatHistory.Clear();
            }
        }

        private static string GetMimeTypeFromStream(Stream stream)
        {
            IImageFormat format = Image.DetectFormat(stream);
            return format.DefaultMimeType; // Fallback if not detected  
        }
    }
}