using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Utilities;

namespace MultiAgents;

#pragma warning disable SKEXP0110, SKEXP0001, SKEXP0101
class Program
{
    private const string ProgramManagerSystem = @"  
            You are a Program Manager responsible for gathering detailed requirements from the user  
            and creating a comprehensive plan for app development. You will document the user  
            requirements and provide cost estimates. Ensure you extract all necessary requirements  
            from the user.  
        ";

    private const string SoftwareEngineerSystem = @"  
            You are a Software Engineer tasked with developing a web app using HTML and JavaScript (JS).  
            You must adhere to all the requirements specified by the Program Manager. Your output  
            should consist of a single file containing the complete HTML and JS code that satisfies the requirements from the program manager.  
        ";

    private const string ProductOwnerSystem = @"  
            You are a Product Owner responsible for ensuring that the Software Engineer's code meets all client requirements.  
            Your role involves reviewing the code thoroughly to verify that it aligns with the specified requirements.  
            Once you are satisfied that all requirements are met, you can approve the request by simply responding with 'IAPPROVE'.  
        ";


    static async Task Main(string[] args)
    {
        try
        {
            IKernelBuilder builder = Kernel.CreateBuilder()
                                           .AddAzureOpenAIChatCompletion(Constants.Gpt4o,
                                                                         Constants.Endpoint,
                                                                         Constants.ApiKey);

            Kernel kernel = builder.Build();

            ChatCompletionAgent softwareEngineerAgent = CreateAgent(nameof(SoftwareEngineerSystem), SoftwareEngineerSystem, kernel);
            ChatCompletionAgent programManagerAgent = CreateAgent(nameof(ProgramManagerSystem), ProgramManagerSystem, kernel);
            ChatCompletionAgent productOwnerAgent = CreateAgent(nameof(ProductOwnerSystem), ProductOwnerSystem, kernel);

            "Welcome to MultiAgents".Write(ConsoleColor.Yellow, true);

            while (true)
            {
                Environment.NewLine.Write(ConsoleColor.White);    

                "You: ".Write(ConsoleColor.White);

                AgentGroupChat groupSession = new(programManagerAgent, softwareEngineerAgent, productOwnerAgent)
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

                groupSession.AddChatMessage(new ChatMessageContent(AuthorRole.User, userInput));
                await ProcessChatAsync(groupSession);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static ChatCompletionAgent CreateAgent(string name, string instructions, Kernel kernel)
        => new()
        {
            Instructions = instructions,
            Name = name,
            Kernel = kernel
        };

    private static async Task ProcessChatAsync(AgentGroupChat chat)
    {
        bool appCreated = false;
        string completeString = string.Empty;
        await foreach (var content in chat.InvokeStreamingAsync())
        {
            completeString += content.Content ?? string.Empty;
            if (content.Content != null && completeString.TryExtractHtmlContent(out var croppedHtml))
            {
                $"{content.AuthorName ?? "*"}: '{croppedHtml}'".Write(ConsoleColor.Green, true);
                croppedHtml?.OpenHtmlInBrowser();
                appCreated = true;
                break;
            }
            else
            {
                $"{content.Content}".Write(ConsoleColor.Blue);
            }
        }

        if (appCreated)
            return;
    }
}

sealed class ApprovalTerminationStrategy : TerminationStrategy
{
    protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> session, CancellationToken cancellationToken)
        => Task.FromResult(session.Last().Content?.Contains("approve", StringComparison.OrdinalIgnoreCase) ?? false);
}
