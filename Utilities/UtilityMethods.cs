using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities;

public static class UtilityMethods
{
    public static void Write(this string message, ConsoleColor color, bool writeLine = false)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        if (writeLine)
            Console.WriteLine(message);
        else
            Console.Write(message);

        Console.ForegroundColor = previousColor;
    }

    public static bool TryExtractHtmlContent(this string content, out string? croppedHtml)
    {
        var match = Regex.Match(content, @"<html[\s\S]*?<\/html>", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            croppedHtml = match.Value;
            return true;
        }
        croppedHtml = null;
        return false;
    }

    public static void OpenHtmlInBrowser(this string htmlContent)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "temp.html");
        File.WriteAllText(tempFilePath, htmlContent);
        Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
    }

    public static void ScopeSession(this ChatHistory session, int maxMessages = 15)
    {
        Console.WriteLine("");
        foreach (var msg in session)
        {
            (@$"Role: {msg.Role}").Write(ConsoleColor.Cyan, true);
        }
      

        if (session.Count > maxMessages)
        {
            int userMessage = -1;

            // Find the index of the first User message  
            for (int i = 1; i < session.Count; i++)
            {
                if (session[i].Role == AuthorRole.User)
                {
                    userMessage = i;
                    break;
                }
            }

            session.RemoveRange(0, userMessage); // This will remove all messages up to the second user message  
        }
    }


}
