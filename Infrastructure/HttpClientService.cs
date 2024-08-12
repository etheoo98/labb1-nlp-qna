using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using Core;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class HttpClientService(
    IConfiguration configuration, 
    HttpClient httpClient) : IHttpClientService
{
    public async Task<Result<TranslationResponse>> TranslateTextAsync(string text, string toLanguageCode,
        CancellationToken cancellationToken)
    {
        try
        {
            ConfigureHeaders("MicrosoftTranslator:DefaultRequestHeaders");
            // TODO: Refactor into private methods
            var json = JsonSerializer.Serialize(new List<Dictionary<string, string>> { new() { { "Text", text } } });
            var content = new StringContent(json, Encoding.UTF8)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(configuration["MicrosoftTranslator:MediaTypeHeader"]!)
                }
            };

            var url = $"{configuration["MicrosoftTranslator:URL"]}&to={toLanguageCode}";
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (root.GetArrayLength() == 0)
            {
                return Result<TranslationResponse>.CriticalError("No translations found.");
            }

            var firstResult = root[0];
            var detectedLanguageCode = firstResult.GetProperty("detectedLanguage").GetProperty("language").GetString();
            var translationText = firstResult.GetProperty("translations")[0].GetProperty("text").GetString();

            var translationResponse = new TranslationResponse(detectedLanguageCode, translationText);

            return Result<TranslationResponse>.Success(translationResponse);
        }
        catch (Exception ex)
        {
            return Result<TranslationResponse>.CriticalError(ex.Message);
        }
    }
    
    public async Task<Result<AnswerResponse>> AnswerQuestionAsync(string question, CancellationToken cancellationToken)
    {
        try
        {
            ConfigureHeaders("CognitiveService:DefaultRequestHeaders");
            
            var questionData = new { question };
            var json = JsonSerializer.Serialize(questionData);
            var content = new StringContent(json, Encoding.UTF8)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(configuration["CognitiveService:MediaTypeHeader"]!)
                }
            };

            var url = configuration["CognitiveService:URL"];
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var jsonDoc = JsonDocument.Parse(responseBody);
            var root = jsonDoc.RootElement;
            var answers = root.GetProperty("answers");
            var firstAnswer = answers.EnumerateArray().First();
            var answerResult = new AnswerResponse(firstAnswer.GetProperty("answer").GetString());

            return Result<AnswerResponse>.Success(answerResult);
        }
        catch (Exception ex)
        {
            return Result<AnswerResponse>.CriticalError(ex.Message);
        }
    }
    
    private void ConfigureHeaders(string sectionName)
    {
        httpClient.DefaultRequestHeaders.Clear();

        var configSection = configuration.GetSection(sectionName);
        foreach (var item in configSection.GetChildren())
        {
            httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
        }
    }
}