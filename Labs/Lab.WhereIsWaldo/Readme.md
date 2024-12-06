# Lab: Where Is Waldo - How many dogs are there in the picture?

## Objective
In this lab, you will initialize the Semantic Kernel without any plugins and use it to answer the question: "How many dogs are there in the Where's Waldo picture?"

## Purpose
The purpose of this lab is to understand how to initialize a Semantic Kernel and a ChatHistory, and use them together with text and image content to get a response.

## Steps

1. **Initialize the Semantic Kernel:**
   - Create a new instance of the `Kernel` using the `Kernel.CreateBuilder()` method.
   - Add the Azure OpenAI Chat Completion service to the kernel builder using the `AddAzureOpenAIChatCompletion` method.
   - Build the kernel instance.

2. **Create a Chat Session:**
   - Initialize a new `ChatHistory` instance to keep track of the conversation.

3. **Implement the Main Logic:**
   - Use a static message to ask the AI: "How many dogs are there in the Where's Waldo picture?"
   - Add the user's message to the chat session.
   - Use the kernel to get the response from the AI.
   - Display the AI's response to the user.

## Expected Output
The console application should display the AI response, either saying that there are 2 or 3 dogs.

## Additional Resources
You can look into the other sample projects in this solution to understand how the kernel is initialized.
