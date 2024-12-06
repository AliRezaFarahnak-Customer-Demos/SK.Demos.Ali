# Lab: World Time - Explain the World Time

## Objective
In this lab, you will develop a single agent AI that can explain the world time to the user.

## Purpose
The purpose of this lab is to understand how to develop a plugin for the Semantic Kernel, initialize the kernel with execution settings, and use the plugin to provide responses.

## Steps

1. **Develop the WorldTimePlugin:**
   - Create a new class `WorldTimePlugin`.
   - Implement a method `GetCurrentTime` that returns the current time and year for a given location.

2. **Initialize the Semantic Kernel:**
   - Create a new instance of the `Kernel` using the `Kernel.CreateBuilder()` method.
   - Add the Azure OpenAI Chat Completion service to the kernel builder using the `AddAzureOpenAIChatCompletion` method.
   - Define the `OpenAIPromptExecutionSettings` with appropriate values for `ToolCallBehavior`, `ChatSystemPrompt`, `Temperature`, `TopP`, and `MaxTokens`.
   - Build the kernel instance.
   - Import the `WorldTimePlugin` into the kernel.

3. **Create a Chat Session:**
   - Initialize a new `ChatHistory` instance to keep track of the conversation.

4. **Implement the Main Logic:**
   - Prompt the user to input their question about the world time.
   - Add the user's message to the chat session.
   - Use the kernel to get the response from the AI using the `WorldTimePlugin`.
   - Display the AI's response to the user.

## Expected Output
The console application should display the AI response explaining the current time and year for the given location.

## Completion Criteria
To complete the app, the AI should be able to tell the time in any city in the world, such as Tokyo, New York, Copenhagen, Riyadh, etc.

## Additional Resources
You can look into the other sample projects in this solution to understand how the kernel is initialized.
