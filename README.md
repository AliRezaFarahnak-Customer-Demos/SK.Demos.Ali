# Semantic Kernel Demos and Labs 

## Purpose

This repository is designed to help developers learn the latest Semantic Kernel and advanced AI flows. It contains a collection of labs and demos that provide step-by-step instructions to develop AI applications leveraging Azure OpenAI services. Each lab focuses on a specific use case, while the demos showcase practical implementations, offering a comprehensive guide to building sophisticated AI solutions.

## Demos

### Researcher Database CoPilot

An AI application that answers questions from a large COVID-19 database. It demonstrates how to handle large datasets and provide accurate responses using the Semantic Kernel.

### Travel Planner Process Workflow

An AI application that helps users plan their travel by providing flight, hotel, and attraction recommendations. It demonstrates the use of a process workflow and the Semantic Kernel for creating a comprehensive travel planning solution.

### GitHub Manager CoPilot

A very advanced GitHub manager that can give a quick overview of the enterprise by being able to talk to the whole REST API. It can handle tasks such as Copilot search, license management, failed builds, and git committers, providing a complete overview.

### Multi Agents

An AI application that involves multiple agents working together to gather requirements, develop a web app, and ensure the final product meets the specified requirements. It showcases the use of multiple agents and the Semantic Kernel for collaborative AI-driven workflows.

### Single Agent

A single agent AI application that uses various plugins to provide responses to user queries. It showcases the integration of multiple plugins and the use of the Semantic Kernel for AI-driven interactions.

## Labs

### Lab: Chef Process Workflow - Create a Dish from Available Ingredients

#### Objective

Develop a workflow where the user provides the ingredients they have in their kitchen, and the AI (acting as a chef) will create a dish using those ingredients.

#### Steps

1. **Develop the Chef Process Workflow:**
   - Create a new class `ChefWorkflow`.
   - Implement steps for the workflow: `IntroStep`, `IngredientInputStep`, `RecipeSearchStep`, and `DishPreparationStep`.

2. **Initialize the Semantic Kernel:**
   - Create a new instance of the `Kernel` using the `Kernel.CreateBuilder()` method.
   - Add the Azure OpenAI Chat Completion service to the kernel builder using the `AddAzureOpenAIChatCompletion` method.
   - Define the `OpenAIPromptExecutionSettings` with appropriate values for `ToolCallBehavior`, `ChatSystemPrompt`, `Temperature`, `TopP`, and `MaxTokens`.
   - Build the kernel instance.

3. **Define Workflow Steps and Transitions:**
   - Define the steps and transitions for the workflow, similar to the `TravelPlanner.ProcessWorkflow`.
   - Ensure the workflow handles user input, searches for recipes, and prepares a dish.

4. **Implement the Main Logic:**
   - Prompt the user to input the ingredients they have in their kitchen.
   - Use the kernel to process the input and generate a recipe.
   - Display the recipe and preparation steps to the user.

### Lab: Advanced AI for COVID-19 Data Analysis and Visualization

#### Objective

Create an advanced AI capable of analyzing and visualizing COVID-19 data from tabular datasets. The AI will respond to complex queries and provide visual representations of the data.

#### Steps

1. **Prepare the Data:**
   - Ensure the data is in CSV format and accessible for processing.
   - Select data spanning 10 days to visualize confirmed infections per state over this period.

2. **Develop the Database Class:**
   - Create a `Database` class to manage loading, deserializing, and serializing the COVID-19 data.
   - Implement methods for loading records from the CSV file and storing them in a list.

3. **Develop the Covid19Plugin:**
   - Create a `Covid19Plugin` class.
   - Implement methods to respond to queries and generate visualizations based on the COVID-19 data.

4. **Initialize the Semantic Kernel:**
   - Create a `Kernel` instance using `Kernel.CreateBuilder()`.
   - Add the Azure OpenAI Chat Completion service to the kernel builder with `AddAzureOpenAIChatCompletion`.
   - Define `OpenAIPromptExecutionSettings` with appropriate parameters.
   - Build the kernel instance and import the `Covid19Plugin`.

5. **Implement the Kernel Execution Filter:**
   - Create a `KernelExecutionFilter` class to manage the execution and visualization of data queries.
   - Implement methods to process function invocation contexts, check for JSON results, and visualize data using HTML and ECharts.

6. **Create a Chat Session:**
   - Initialize a `ChatHistory` instance to track the conversation.

7. **Implement the Main Logic:**
   - Prompt the user to input their COVID-19 data-related question.
   - Add the user's message to the chat session.
   - Use the kernel to get a response from the AI via the `Covid19Plugin`.
   - Display the AI's response and visualizations to the user.

### Lab: Where Is Waldo - How many dogs are there in the picture?

#### Objective

Initialize the Semantic Kernel without any plugins and use it to answer the question: "How many dogs are there in the Where's Waldo picture?"

#### Steps

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

### Lab: World Time - Explain the World Time

#### Objective

Develop a single agent AI that can explain the world time to the user.

#### Steps

1. **Develop the WorldTimePlugin:**
   - Create a new class `WorldTimePlugin`.
   - Implement a method `GetCurrentTime` that returns the current time and year for a given location.

2. **Initialize the Semantic Kernel:**
   - Create a new instance of the `Kernel` using the `Kernel.CreateBuilder()` method.
   - Add the Azure OpenAI Chat Completion service to the kernel builder using the `AddAzureOpenAIChatCompletion` method.
   - Define the `OpenAIPromptExecutionSettings` with appropriate values.
   - Build the kernel instance.
   - Import the `WorldTimePlugin` into the kernel.

3. **Create a Chat Session:**
   - Initialize a new `ChatHistory` instance to keep track of the conversation.

4. **Implement the Main Logic:**
   - Prompt the user to input their question about the world time.
   - Add the user's message to the chat session.
   - Use the kernel to get the response from the AI using the `WorldTimePlugin`.
   - Display the AI's response to the user.

## Additional Resources

Refer to the individual lab readme files and demo source code for detailed instructions and lab guidelines.