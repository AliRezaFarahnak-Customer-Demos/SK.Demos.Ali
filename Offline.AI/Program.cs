using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Utilities;
// sudo docker run --runtime nvidia --gpus all --name Phi-3.5-vision-instruct -v ~/.cache/huggingface:/root/.cache/huggingface -p 8000:8000 --ipc=host vllm/vllm-openai:latest --model microsoft/Phi-3.5-vision-instruct --gpu_memory_utilization=0.99 --max_model_len=4000 --trust-remote-code
// sudo docker run --runtime nvidia --gpus all --name Phi-3-vision-128k-instruct -v ~/.cache/huggingface:/root/.cache/huggingface -p 8000:8000 --ipc=host vllm/vllm-openai:latest --model microsoft/Phi-3-vision-128k-instruct --gpu_memory_utilization=0.99 --max_model_len=4000 --trust-remote-code

namespace Offline.AI;

class Program
{
    private static Kernel kernel;
    static async Task Main(string[] args)
    {
        kernel = Kernel.CreateBuilder()
                      .AddHuggingFaceChatCompletion("microsoft/Phi-3.5-vision-instruct", new Uri("http://localhost:8000"))
                      .Build();

        var chatService = kernel.GetRequiredService<IChatCompletionService>();



        var chatHistory = new ChatHistory($@"
Answer as short as possible and only what the user asks for.
Novo Developer Day (proposed agenda)
Facilitators: Martin Nielsen
Speakers: Ali (alfarahn), Bartek (kierunb) and Abishek (abymsft)

Time	Plan	NOTE	Duration
8 to 830 AM	Breakfast 🥗🍲		
8:45	Kick-off by Novo leadership		
9 AM	Slides and demo sessions (3 Tracks, Level Beginner->Advanced)
	Everyone attends every track. 45 minutes for 3 tracks 5-minute break between sessions.
	2 hour 35 minutes
	Track#1 Working with AI agents with SK
Track#2 GH Copilot
Track#3 GHAS		
11:30 AM	Lunch + Coffee	-		60 minutes
12:30 PM	Put your learnings to Test!	1.	Three separate tracks, participants decide which track they want to join. 
2.	Participants take the demo code repositories/samples from speakers and try it themselves on a Sandbox GitHub Org (in Novo Nordisk Enterprise).
3.	Speakers share challenges/tasks with participants
4.	If needed, participants clone the repositories in Novo Nordisk’s sandbox GH organization
	2.5 Hours
14:30
Participant feedback	Group wise feedback
-	3 groups per track
-	What have the participants learned? 2 take aways from the session.	30 minutes
15:00	Socializing and Q&A with Microsoft		30 minutes
15:30	End of Day		

Agenda here.

Pre-requisites for tools: GitHub Copilot, Codespaces, and GitHub Advanced Security:

Get an OpenAI endpoint and Api KEY from Novo Nordisk [To be used with Semantic Kernel challenges]
GitHub Copilot
1.	GitHub Account: Ensure all participants have a GitHub account.
2.	IDE Installation: Install a supported IDE such as Visual Studio Code, Visual Studio, or JetBrains IDEs.
3.	GitHub Copilot Extension: Install the GitHub Copilot extension for the chosen IDE.
4.	Subscription: Ensure participants have an active GitHub Copilot Enterprise subscription and are assigned a seat by GH org admins.
5.	Configuration: Configure GitHub Copilot in the IDE settings2.
GitHub Codespaces
1.	GitHub Account: Ensure all participants have a GitHub account.
2.	Repository Setup: Set up a repository with a dev container configuration to define the development environment3.
3.	Codespaces Access: Ensure participants have access to GitHub Codespaces.
1.	Link1 Managing Codespaces
4.	Browser: Use a modern web browser to access Codespaces.
5.	Permissions: Check for permissions to create and manage Codespaces4.
GitHub Advanced Security
1.	GitHub Account: Ensure all participants have a GitHub account.
2.	Repository Setup: Set up repositories with GitHub Advanced Security features enabled.
3.	License: Ensure participants have a GitHub Advanced Security license.
4.	Configuration: Enable and configure CodeQL, secret scanning, and dependency review in the repositories6.
5.	Permissions: Ensure repository admin or organization Security Manager permissions.
");


        "Welcome to AI on localHost!".Write(ConsoleColor.Yellow, true);

        while (true)
        {

            Console.Write("\nYou: ");
            var question = Console.ReadLine();
            if (string.IsNullOrEmpty(question)) break;

            chatHistory.AddUserMessage(question);

            string completeResponse = string.Empty;

            await foreach (var responsePart in chatService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                var botResponsePart = responsePart?.Content;
                if (botResponsePart != null)
                {
                    botResponsePart.Write(ConsoleColor.Green);
                    completeResponse += botResponsePart;
                }
            }

            Console.WriteLine();

            if (!string.IsNullOrEmpty(completeResponse))
            {
                chatHistory.AddAssistantMessage(completeResponse);
            }
        }
    }
}


