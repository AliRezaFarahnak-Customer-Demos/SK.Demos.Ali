using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Single.Agent.Plugins;
using Utilities;

namespace TravelPlanner.Workflow;

internal class Program
{
    private static Kernel _kernel;

    // Defines the events used in the travel planning process  
    public static class TravelPlannerProcessEvents
    {
        public const string StartPlanning = nameof(StartPlanning);
        public const string TravelRequestReceived = nameof(TravelRequestReceived);
        public const string FlightsFound = nameof(FlightsFound);
        public const string FlightsNotFound = nameof(FlightsNotFound);
        public const string HotelsFound = nameof(HotelsFound);
        public const string HotelsNotFound = nameof(HotelsNotFound);
        public const string WeatherChecked = nameof(WeatherChecked); // New event  
        public const string AttractionsFound = nameof(AttractionsFound);
        public const string Exit = nameof(Exit);
    }

    // Complex object to carry travel plan details  
    public record struct TravelPlan(string TravelRequest, string FlightInfo, string HotelInfo, string Attractions, string WeatherInfo, string CurrentDate);

    private static async Task Main(string[] args)
    {
        // Initialize the Kernel with Azure OpenAI Chat Completion  
        _kernel = Kernel.CreateBuilder()
                       .AddAzureOpenAIChatCompletion(Constants.Gpt4o, Constants.Endpoint, Constants.ApiKey)
                       .Build();
        _kernel.ImportPluginFromType<WorldWeatherPlugin>();

        // Create and configure the travel planning process  
        ProcessBuilder travelProcess = new(nameof(travelProcess));
        var introStep = travelProcess.AddStepFromType<IntroStep>();
        var travelInputStep = travelProcess.AddStepFromType<TravelInputStep>();
        var flightSearchStep = travelProcess.AddStepFromType<FlightSearchStep>();
        var hotelSearchStep = travelProcess.AddStepFromType<HotelSearchStep>();
        var attractionsStep = travelProcess.AddStepFromType<AttractionsStep>();
        var weatherCheckStep = travelProcess.AddStepFromType<WeatherCheckStep>(); // New step  
        var offerStep = travelProcess.AddStepFromType<OfferStep>();

        // Define the workflow steps and transitions  
        travelProcess
            .OnInputEvent(TravelPlannerProcessEvents.StartPlanning)
            .SendEventTo(new ProcessFunctionTargetBuilder(introStep));

        introStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(travelInputStep));

        travelInputStep
            .OnEvent(TravelPlannerProcessEvents.Exit)
            .StopProcess();
        travelInputStep
            .OnEvent(TravelPlannerProcessEvents.TravelRequestReceived)
                .SendEventTo(new ProcessFunctionTargetBuilder(flightSearchStep, parameterName: "travelPlan"));

        flightSearchStep
            .OnEvent(TravelPlannerProcessEvents.FlightsFound)
                .SendEventTo(new ProcessFunctionTargetBuilder(hotelSearchStep, parameterName: "travelPlan"));
        flightSearchStep
            .OnEvent(TravelPlannerProcessEvents.FlightsNotFound)
            .SendEventTo(new ProcessFunctionTargetBuilder(introStep));

        hotelSearchStep
            .OnEvent(TravelPlannerProcessEvents.HotelsFound)
                .SendEventTo(new ProcessFunctionTargetBuilder(attractionsStep, parameterName: "travelPlan"));
        hotelSearchStep
            .OnEvent(TravelPlannerProcessEvents.HotelsNotFound)
            .SendEventTo(new ProcessFunctionTargetBuilder(introStep));

        attractionsStep
            .OnEvent(TravelPlannerProcessEvents.AttractionsFound)
                .SendEventTo(new ProcessFunctionTargetBuilder(weatherCheckStep, parameterName: "travelPlan")); // Updated transition  

        weatherCheckStep // New transition  
            .OnEvent(TravelPlannerProcessEvents.WeatherChecked)
                .SendEventTo(new ProcessFunctionTargetBuilder(offerStep, parameterName: "travelPlan"));

        offerStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(introStep)); // Reset process after offer is sent  

        // Build and start the travel planning kernel process  
        KernelProcess travelKernelProcess = travelProcess.Build();
        await StartProcessAsync(travelKernelProcess);
    }

    private static async Task StartProcessAsync(KernelProcess travelKernelProcess)
    {
        while (true)
        {
            var runningProcess = await travelKernelProcess.StartAsync(_kernel, new KernelProcessEvent() { Id = TravelPlannerProcessEvents.StartPlanning, Data = null });
        }
    }


    // Step 1: Introduction Step  
    private sealed class IntroStep : KernelProcessStep
    {
        [KernelFunction]
        public void PrintIntroMessage()
        {
            "Welcome to the Travel Planner!".Write(ConsoleColor.Green, true);
        }
    }

    // Step 2: Travel Input Step  
    private sealed class TravelInputStep : KernelProcessStep
    {
        [KernelFunction]
        public async ValueTask GetTravelDetailsAsync(KernelProcessStepContext context)
        {
            var travelRequest = Console.ReadLine();
            var travelPlan = new TravelPlan { TravelRequest = travelRequest };

            if (travelRequest.ToLowerInvariant().Equals(TravelPlannerProcessEvents.Exit.ToLowerInvariant()))
                await context.EmitEventAsync(new() { Id = TravelPlannerProcessEvents.Exit });
            else
            {
                travelPlan.CurrentDate = DateTime.Now.ToString("dd/MM/yyyy");
                await context.EmitEventAsync(new() { Id = TravelPlannerProcessEvents.TravelRequestReceived, Data = travelPlan });
            }
        }
    }

    // Step 3: Flight Search Step  
    private sealed class FlightSearchStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task SearchFlightsAsync(KernelProcessStepContext context, TravelPlan travelPlan)
        {
            "Searching for flights...".Write(ConsoleColor.Yellow);
            var aiCompletion = _kernel.GetRequiredService<IChatCompletionService>();
            var flightSession = new ChatHistory("You generate mock data in json. Only pick airlines belonging to the country location.");
            flightSession.AddUserMessage(travelPlan.TravelRequest);
            var flightResponse = await aiCompletion.GetChatMessageContentAsync(flightSession);

            // Simulate flight search and emit appropriate event  
            bool flightsFound = true; // Simulate the result  
            if (flightsFound)
            {
                travelPlan.FlightInfo = flightResponse.Content;
                await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.FlightsFound, Data = travelPlan });
            }
            else
            {
                await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.FlightsNotFound, Data = travelPlan });
            }
        }
    }

    // Step 4: Hotel Search Step  
    private sealed class HotelSearchStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task SearchHotelsAsync(KernelProcessStepContext context, TravelPlan travelPlan)
        {
            "Searching for hotels...".Write(ConsoleColor.Yellow);
            var aiCompletion = _kernel.GetRequiredService<IChatCompletionService>();
            var hotelSession = new ChatHistory("Generate mock data in json, max 100 characters. Pick a top hotel from the location.");
            hotelSession.AddUserMessage(travelPlan.FlightInfo);
            var hotelResponse = await aiCompletion.GetChatMessageContentAsync(hotelSession);

            // Simulate hotel search and emit appropriate event  
            bool hotelsFound = true; // Simulate the result  
            if (hotelsFound)
            {
                travelPlan.HotelInfo = hotelResponse.Content;
                await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.HotelsFound, Data = travelPlan });
            }
            else
            {
                await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.HotelsNotFound, Data = travelPlan });
            }
        }
    }

    // Step 5: Must-See Attractions Step  
    private sealed class AttractionsStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task ListAttractionsAsync(KernelProcessStepContext context, TravelPlan travelPlan)
        {
            "Gathering must-see attractions...".Write(ConsoleColor.Yellow, true);
            var aiCompletion = _kernel.GetRequiredService<IChatCompletionService>();
            var attractionsSession = new ChatHistory("Generate a list of must-see attractions in the specified location and within the given travel dates, make it short.");
            attractionsSession.AddUserMessage(travelPlan.HotelInfo);
            var attractionsResponse = await aiCompletion.GetChatMessageContentAsync(attractionsSession);

            // Assume attractions are always found for simplicity  
            travelPlan.Attractions = attractionsResponse.Content;
            await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.AttractionsFound, Data = travelPlan });
        }
    }

    // Step 6: Weather Check Step  
    private sealed class WeatherCheckStep : KernelProcessStep
    {
        private static OpenAIPromptExecutionSettings _executionSettings => new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            ChatSystemPrompt = $"You are an AI",
            Temperature = 0.2f,
            TopP = 0.4f,
            MaxTokens = 4096
        };

        [KernelFunction]
        public async Task CheckWeatherAsync(KernelProcessStepContext context, TravelPlan travelPlan)
        {
           
            var aiCompletion = _kernel.GetRequiredService<IChatCompletionService>();
            var weatherSession = new ChatHistory(@$"Give me the weather of the location in this: {travelPlan.HotelInfo}");
            weatherSession.AddUserMessage(travelPlan.HotelInfo);
            var weatherResponse = await aiCompletion.GetChatMessageContentAsync(weatherSession, _executionSettings, _kernel);

            travelPlan.WeatherInfo = weatherResponse.Content;

            // Assume weather check is always successful for simplicity  
            await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.WeatherChecked, Data = travelPlan });
        }
    }

    // Step 7: Offer Step  
    private sealed class OfferStep : KernelProcessStep
    {
        [KernelFunction]
        public async Task SendOfferAsync(KernelProcessStepContext context, TravelPlan travelPlan, Kernel kernel)
        {
            "Sending travel offer to the user...".Write(ConsoleColor.White, true);
            var aiCompletion = kernel.GetRequiredService<IChatCompletionService>();
            var offerSession = new ChatHistory("Make a great offer including daily plus total price with a lot of emojis and offer should be inside card view and very beautiful and the recommendations in regards to the weather, for the chosen travel and it should be beautiful HTML");
            offerSession.AddUserMessage($@"
                Travel Request: {travelPlan.TravelRequest}
                Flight Info: {travelPlan.FlightInfo}
                Hotel Info: {travelPlan.HotelInfo}
                Attractions: {travelPlan.Attractions}
                Weather Info: {travelPlan.WeatherInfo}
                Current Date: {travelPlan.CurrentDate}
            ");
            var flightOffer = await aiCompletion.GetChatMessageContentAsync(offerSession);
            if (flightOffer.Content.TryExtractHtmlContent(out var croppedHtml))
            {
                croppedHtml?.OpenHtmlInBrowser();
            }
            "Travel offer made with ProcessWorkflow".Write(ConsoleColor.Blue, true);
            Console.WriteLine();

            // Reset the process for a new travel request
            await context.EmitEventAsync(new KernelProcessEvent { Id = TravelPlannerProcessEvents.StartPlanning, Data = null });
        }
    }
}