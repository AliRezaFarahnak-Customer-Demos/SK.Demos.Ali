# Lab: Chef Process Workflow - Create a Dish from Available Ingredients

## Objective

In this lab, you will develop a workflow where the user provides the ingredients they have in their kitchen, and the AI (acting as a chef) will create a dish using those ingredients.

## Purpose

The purpose of this lab is to understand how to develop a workflow using the Semantic Kernel, initialize the kernel with execution settings, and use the workflow to provide responses. This lab will also demonstrate how to handle user input and generate a structured output.

## Steps

1. **Develop the Chef Process Workflow:**
   - Create a new class `ChefWorkflow`.
   - Implement steps for the workflow: `IntroStep`, `IngredientInputStep`, `RecipeSearchStep`, `DishPreparationStep`, and `CreateImageStep`.

2. **Initialize the Semantic Kernel:**
   - Create a new instance of the `Kernel` using the `Kernel.CreateBuilder()` method.
   - Add the Azure OpenAI Chat Completion service to the kernel builder using the `AddAzureOpenAIChatCompletion` method.
   - Define the `OpenAIPromptExecutionSettings` with appropriate values for `ToolCallBehavior`, `ChatSystemPrompt`, `Temperature`, `TopP`, and `MaxTokens`.
   - Build the kernel instance.

3. **Define Workflow Steps and Transitions:**
   - Define the steps and transitions for the workflow, similar to the `TravelPlanner.ProcessWorkflow`.
   - Ensure the workflow handles user input, searches for recipes, prepares a dish, and creates an image for the recipe.

4. **Implement the Main Logic:**
   - Prompt the user to input the ingredients they have in their kitchen.
   - Use the kernel to process the input and generate a recipe.
   - Display the recipe, preparation steps, and an image of the dish to the user.

## Expected Workflow Events

- `StartCooking`: Initiates the cooking process.
- `IngredientsProvided`: Triggered when the user provides the list of ingredients.
- `RecipeFound`: Triggered when a suitable recipe is found.
- `RecipeNotFound`: Triggered when no suitable recipe is found.
- `DishPrepared`: Triggered when the dish is successfully prepared.
- `ImageCreated`: Triggered when an image of the dish is successfully created.
- `Exit`: Ends the workflow.

## Expected Output

The console application should display the AI's response, a name for the dish including a recipe, preparation steps, and an image of the dish, based on the ingredients provided by the user. The output should be a structured HTML page that includes the recipe, preparation instructions, and the image.

## Completion Criteria

To complete the app, the AI should:
- Accept a list of ingredients from the user.
- Generate a recipe using the provided ingredients.
- Provide detailed preparation steps.
- Create an image of the dish.
- Display the recipe, preparation steps, and the image in a structured HTML page.

## Additional Resources

You can look into the `TravelPlanner.ProcessWorkflow` project to understand how the kernel is initialized and how workflows are implemented.
