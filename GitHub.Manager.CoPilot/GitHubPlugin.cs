using System.ComponentModel;
using System.Net.Http.Headers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace GitHubCoPilotExtreme;

public class GitHubPlugin
{
    public static HttpClient Client => new HttpClient()
    {
        DefaultRequestHeaders = {
            UserAgent = { new ProductInfoHeaderValue("DotNetApp", "1.0") },
            Authorization = new AuthenticationHeaderValue("Bearer", Constants.GitHubPAT)
        }
    };


    [KernelFunction, Description(
        @"
This is the enterprise you have full access to: https://github.com/enterprises/alirezafarahnak/
You are the github admin and you have full access to the GitHub Rest API with the authorized client: https://docs.github.com/en/rest/meta/meta?apiVersion=2022-11-28
Client object is accesible. 

For example, to list the organizations, you can use the following CodeQuery:
var content = await Client.GetStringAsync(""https://api.github.com/user/orgs""); 
var orgs = JsonSerializer.Deserialize<List<JsonElement>>(content);
var logins = orgs.Select(org => org.GetProperty(""login"").GetString()).ToList();
var loginsJson = JsonSerializer.Serialize(logins);
return loginsJson;

You can also delete, create, and update teams, manage team members, and manage team repositories and workflows and issues.
")]
    public async Task<string> GetGitHubRestAPIDataQueryAnswerAsync([Description(@"CSharpScript.EvaluateAsync(CodeQuery)")] string CodeQuery)
    {
        try
        {
            $@"Dynamic Code:{Environment.NewLine}{CodeQuery}".Write(ConsoleColor.Red, true);   
            string result = await DynamicQuery(CodeQuery);
            $@"Function => Json Response:{Environment.NewLine}{result}".Write(ConsoleColor.Cyan, true);
            return result;
        }
        catch (Exception ex)
        {
            ex.Message.Write(ConsoleColor.Red, true);
            return ex.Message;
        }
    }

    protected async Task<string> DynamicQuery(string codeQuery)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        var result = await CSharpScript.EvaluateAsync<dynamic>(
            codeQuery,
            ScriptOptions.Default
                .WithReferences(assemblies)
                .WithImports("System",
                             "System.Linq",
                             "System.Collections.Generic",
                             "System.Net.Http",
                             "System.Text.Json",
                             "System.Text",
                             "System.Threading.Tasks",
                             $"{nameof(GitHubCoPilotExtreme)}.{nameof(GitHubPlugin)}",
                             $"{nameof(GitHubCoPilotExtreme)}")
        );

        return result;
    }
}