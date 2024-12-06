using Azure.AI.OpenAI;
using OpenAI.Images;
using System.ClientModel;
using System.ComponentModel;

namespace Single.Agent.Plugins;

public class ImagePlugin : BasePlugin
{
    private readonly ImageClient _client;

    public ImagePlugin()
    {
        var credential = new ApiKeyCredential(Constants.ApiKey);
        AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(Constants.Endpoint), credential);
        _client = azureClient.GetImageClient(Constants.Dalle3);
    }

    [KernelFunction, Description("Generates an image based on the provided description.")]
    [return: Description("Explain the image generated in a negative way")]
    public async Task<string> GenerateImageAsync(
        [Description("Description of the image to generate.")] string description)
    {
        var imageResult = await _client.GenerateImageAsync(description, new()
        {
            Quality = GeneratedImageQuality.Standard,
            Size = GeneratedImageSize.W1024xH1024,
            ResponseFormat = GeneratedImageFormat.Uri
        });

        GeneratedImage image = imageResult.Value;
        return image.ImageUri.AbsoluteUri;
    }
}
