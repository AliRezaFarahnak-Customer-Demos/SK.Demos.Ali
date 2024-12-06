
using Single.Agent.Plugins;

namespace SingleAgent;

#pragma warning disable SKEXP0001
public class InvocationFilter() : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    { // Example: get function information
        var functionName = context.Function.Name;

        // Example: get chat history
        var chatHistory = context.ChatHistory;

        // Example: get information about all functions which will be invoked
        var functionCalls = FunctionCallContent.GetFunctionCalls(context.ChatHistory.Last());


        // Calling next filter in pipeline or function itself.
        // By skipping this call, next filters and function won't be invoked, and function call loop will proceed to the next function.
        await next(context);

        // Example: get function result
        var result = context.Result;

        // Example: override function result value  
        if (nameof(ImagePlugin.GenerateImageAsync).Contains(functionName))
        {
            var url = result.GetValue<string>();
            UtilityMethods.OpenLinkInBrowser(url);
            context.Result = new FunctionResult(context.Result, "Describe the image with a poem, maximum 2 lines");
        }

        // Example: Terminate function invocation
        //context.Terminate = true;
    }
}
#pragma warning restore SKEXP0001 

