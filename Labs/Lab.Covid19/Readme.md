# Lab: Advanced AI for COVID-19 Data Analysis and Visualization  
  
## Objective  
  
In this lab, you will create an advanced AI capable of analyzing and visualizing COVID-19 data from tabular datasets. The AI will be designed to respond to complex queries and provide visual representations of the data.  
  
## Purpose  
  
The lab aims to teach you how to develop a sophisticated plugin for the Semantic Kernel that relies on data queries rather than search or unstructured data. This involves handling potentially large data sets to deliver efficient and accurate responses and visualizations. Additionally, you will learn how to implement and utilize a Semantic Kernel execution filter for data manipulation and visualization.  
  
## Steps  
  
1. **Prepare the Data:**  
   - The COVID-19 data has already been downloaded and is available in the `Data` folder.  
   - Ensure the data is in CSV format and accessible for processing.  
   - Select data spanning 10 days to visualize confirmed infections per state over this period.  
  
2. **Develop the Database Class:**  
   - Create a `Database` class to manage loading, deserializing, and serializing the COVID-19 data.  
   - Implement methods for loading records from the CSV file and storing them in a list.  
  
3. **Develop the Covid19Plugin:**  
   - Create a `Covid19Plugin` class.  
   - Implement methods to respond to queries and generate visualizations based on the COVID-19 data.  
   - Use the `UsagePlugin` as a reference for executing queries and manipulating data.  
  
4. **Initialize the Semantic Kernel:**  
   - Create a `Kernel` instance using `Kernel.CreateBuilder()`.  
   - Add the Azure OpenAI Chat Completion service to the kernel builder with `AddAzureOpenAIChatCompletion`.  
   - Define `OpenAIPromptExecutionSettings` with appropriate parameters like `ToolCallBehavior`, `ChatSystemPrompt`, `Temperature`, `TopP`, and `MaxTokens`.  
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
  
## Sample Questions

Here are some sample questions the app chat should be able to answer:

- "What is the total number of confirmed COVID-19 cases in Alabama over the last 10 days?"
- "How many deaths were reported in Alaska on January 25, 2023?"
- "Can you show a chart of the active COVID-19 cases in the US for the past week?"
- "What is the case fatality ratio for Alabama?"
- "How many people were hospitalized in Alaska due to COVID-19?"
- "What is the incident rate of COVID-19 in Alabama?"

## Expected Output  
  
The console application should display the AI's response, including visualizations, that explain the COVID-19 data based on the user's queries. The AI should visualize the confirmed infected people per state over a 10-day period.  
  
## Completion Criteria  
  
To complete the app, the AI should:  
- Answer complex queries regarding COVID-19 data.  
- Provide visual data representations, such as charts and graphs.  
- Use the Semantic Kernel and execution filter for data manipulation and filtering.  
  
## Additional Resources  
  
Review other sample projects in this solution to understand how the kernel is initialized and how plugins are implemented. Data from January 2023 for the US is already included.