using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities;

public static class UtilityMethods
{
    public static void Write(this string message, ConsoleColor color = ConsoleColor.White, bool writeLine = false)
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

    public static void OpenLinkInBrowser(string url)
    {
        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else
        {
            throw new ArgumentException("The provided URL is not valid.", nameof(url));
        }
    }


    public static void OpenHtmlInBrowser(this string htmlContent)
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), "temp.html");
        File.WriteAllText(tempFilePath, htmlContent);
        Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
    }

    public static void ScopeSession(this ChatHistory session, bool writeSession = false, int maxMessages = 40)
    {
        Console.WriteLine("");
        if (writeSession)
        {
            foreach (var msg in session)
            {
               $"{msg.Role}".Write(ConsoleColor.Blue,true);
            }
        }

        if (session.Count > maxMessages && writeSession)
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
