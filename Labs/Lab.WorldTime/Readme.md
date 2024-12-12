# Lab: World Time and Aviation Information  
  
## Objective  
In this lab, you will develop a single-agent AI capable of providing users with current world time information and aviation data, such as flight arrivals and departures.  
  
## Purpose  
The purpose of this lab is to gain hands-on experience in developing plugins for the Semantic Kernel, initializing the kernel with execution settings, and utilizing these plugins to generate informative responses. You will also learn how to integrate external data sources into your AI application.  
  
## Steps  
  
### 1. Initialize the Semantic Kernel  
- **Create a new instance of the `Kernel` using the `Kernel.CreateBuilder()` method.**  
- **Add the Azure OpenAI Chat Completion service to the kernel builder using the `AddAzureOpenAIChatCompletion` method.**  
- **Define the `OpenAIPromptExecutionSettings` with appropriate values for `ToolCallBehavior`, `ChatSystemPrompt`, `Temperature`, `TopP`, and `MaxTokens`.**  
- **Build the kernel instance.**  
  
### 2. Create a Chat Session  
- **Initialize a new `ChatHistory` instance to keep track of the conversation.**  
  
### 3. Implement the Main Logic  
- **Prompt the user to input their question about the world time or flight information.**  
- **Add the user's message to the chat session.**  
- **Use the kernel to get the response from the AI.**  
- **Display the AI's response to the user.**  
  
### 4. Develop the WorldTimePlugin  
- **Create a new class `WorldTimePlugin`.**  
  - Implement a method `GetCurrentTime` that returns the current time and year for a specified location.  
  
### 5. Develop the AviationPlugin  
- **Create a new class `AviationPlugin`.**  
  - Implement a method `QueryFlightArrivalsAsync` that fetches flight arrival data from an external API.  
  - Use the `HttpClient` to make asynchronous requests to the API.  
  - Deserialize the JSON response and format it into a CSV string.  
  - Ensure the response includes emojis and relevant flight details like airline names and flight numbers.  
  
### 6. Integrate the Plugins  
- **Import the `WorldTimePlugin` and `AviationPlugin` into the kernel.**  
- **Ensure the plugins can handle queries related to world time and flight information.**  
  
## API Key  
- You will need an API key to access the aviation data. Use the following URL format for the AviationPlugin: `https://aviation-edge.com/v2/public/timetable?key=YOUR_API_KEY&iataCode=cph&type=arrival`. Replace `YOUR_API_KEY` with your actual API key.  
- This is an example how you can call the api with the key https://aviation-edge.com/v2/public/timetable?key=79a407-f81a02&iataCode=cph&type=arrival
 
## Expected Output  
The console application should display AI responses that explain the current time and year for a given location and provide detailed flight information for specified queries.  
  
## Completion Criteria  
To complete the lab, the AI should be able to answer thse:  
- What is the current time in CPH?
- What is the current time in Dubai?

For aviation it should be able to tell the correct time for arrival and departure from this website for Dubau arrivals: https://dubai-dxb-airport.com/flights/arrivals/
  
By following these steps, you will create a robust AI application capable of providing both world time and aviation information to users.  

arrivals for CPH:
https://aviation-edge.com/v2/public/timetable?key=79a407-f81a02&iataCode=cph&type=arrival
departures for CPH:
https://aviation-edge.com/v2/public/timetable?key=79a407-f81a02&iataCode=CPH&type=departure