using Ardalis.Result;
using Azure;
using Azure.AI.Translation.Text;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Azure;

public class AzureTranslationService(IConfiguration configuration) : ITextTranslationService
{
    public async Task<Result<TranslationResult>> TranslateTextAsync(string text,
        string toLanguageCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var credential = new AzureKeyCredential(configuration["Azure:Translator:Ocp-Apim-Subscription-Key"]!);
            var uri = new Uri(configuration["Azure:Translator:URL"]!);
            var region = configuration["Azure:Translator:Ocp-Apim-Subscription-Region"];
            
            var client = new TextTranslationClient(credential, uri, region);
            var response = await client.TranslateAsync(toLanguageCode, text, cancellationToken: cancellationToken);

            if (response?.Value == null || string.IsNullOrEmpty(response.Value[0].DetectedLanguage.Language) 
                                        || string.IsNullOrEmpty(response.Value[0].Translations[0].Text))
            {
                return Result.CriticalError(
                    "Unexpected format in Azure Translation response.");
            }

            var detectedLanguageCode = response.Value[0].DetectedLanguage.Language;
            var translatedText = response.Value[0].Translations[0].Text;
            var translationResult = new TranslationResult(detectedLanguageCode, translatedText);
            
            return Result<TranslationResult>.Success(translationResult);
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}