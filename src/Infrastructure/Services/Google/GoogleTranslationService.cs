using Ardalis.Result;
using Google.Cloud.Translation.V2;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using TranslationResult = Infrastructure.Models.TranslationResult;

namespace Infrastructure.Services.Google;

public class GoogleTranslationService(IConfiguration configuration) : ITextTranslationService
{
    public async Task<Result<TranslationResult>> TranslateTextAsync(string text, string toLanguageCode, CancellationToken cancellationToken)
    {
        var client = TranslationClient.CreateFromApiKey(configuration["GoogleCloud:ApiKey"]);
        var response = await client.TranslateTextAsync(text, toLanguageCode, cancellationToken: cancellationToken);

        if (response?.DetectedSourceLanguage == null || response.TranslatedText == null)
        {
            return Result.CriticalError("Unexpected Null values in Google Cloud Translation response.");
        }

        var translationResult = new TranslationResult(response.DetectedSourceLanguage, response.TranslatedText);
        return Result<TranslationResult>.Success(translationResult);
    }
}