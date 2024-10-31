using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MultiAgents;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101
class Program
{
    private const string ProgramManager = @"  
            You are a Program Manager responsible for gathering detailed requirements from the user  
            and creating a comprehensive plan for app development. You will document the user  
            requirements and provide cost estimates. Ensure you extract all necessary requirements  
            from the user.  
        ";

    private const string SoftwareEngineer = @"  
            You are a Software Engineer tasked with developing a web app using HTML and JavaScript (JS).  
            You must adhere to all the requirements specified by the Program Manager. Your output  
            should consist of a single file containing the complete HTML and JS code that satisfies the requirements from the program manager.  
        ";

    private const string ProductOwner = @"  
            You are a Product Owner responsible for ensuring that the Software Engineer's code meets all client requirements.  
            Your role involves reviewing the code thoroughly to verify that it aligns with the specified requirements.  
            Once you are satisfied that all requirements are met, you can approve the request by simply responding with 'IAPPROVE'.  
        ";

    private static readonly IConfiguration _cfg = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    private static Kernel _kernel = null!;

    static async Task Main(string[] args)
    {
        try
        {
            var builder = Kernel.CreateBuilder()
                                .AddAzureOpenAIChatCompletion(_cfg["DeploymentName"],
                                                              _cfg["EndPoint"], 
                                                              _cfg["ApiKey"]);

            _kernel = builder.Build();
           
            var softwareEngineerAgent = CreateAgent(nameof(SoftwareEngineer), SoftwareEngineer);
            var programManagerAgent = CreateAgent(nameof(ProgramManager), ProgramManager);
            var productOwnerAgent = CreateAgent(nameof(ProductOwner), ProductOwner);

            @$"Welcome to MultiAgents".Write(ConsoleColor.Yellow);

            while (true)
            {
                "Explain the program (html, js) app you want the dev team to make:".Write(ConsoleColor.White, false);

                AgentGroupChat chat = new (programManagerAgent, softwareEngineerAgent, productOwnerAgent)
                {
                    ExecutionSettings = new()
                    {
                        TerminationStrategy = new ApprovalTerminationStrategy
                        {
                            Agents = new List<ChatCompletionAgent> { productOwnerAgent },
                            MaximumIterations = 15,
                        }
                    }
                };

                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "exit")
                {
                    break;
                }

                chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, userInput));
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


    private static async Task ProcessChatAsync(AgentGroupChat chat)
    {
        bool appCreated = false;
        await foreach (var content in chat.InvokeAsync())
        {
            if (content.Content != null && content.Content.TryExtractHtmlContent(out var croppedHtml))
            {
                $"#{content.AuthorName ?? "*"}: '{croppedHtml}'".Write(ConsoleColor.Green);
                croppedHtml.OpenHtmlInBrowser();
                appCreated = true;
                break;
            }
            else
            {
                $"#{content.AuthorName ?? "*"}: '{content.Content}'".Write(ConsoleColor.Green);
            }
        }

        if (appCreated)
            return;
    }

}

sealed class ApprovalTerminationStrategy : TerminationStrategy
{
    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
        => Task.FromResult(history.Last().Content?.Contains("PRODUCT-DONE", StringComparison.OrdinalIgnoreCase) ?? false);
}
