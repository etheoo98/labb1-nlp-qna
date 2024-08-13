using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class HttpClientService(
    IConfiguration configuration, 
    HttpClient httpClient) : IHttpClientService
{
    public async Task<Result<string>> FetchTranslationResponseAsync(string text, string toLanguageCode,
        CancellationToken cancellationToken)
    {
        try
        {
            ConfigureHeaders("MicrosoftTranslator:DefaultRequestHeaders");
            
            var requestData = new List<Dictionary<string, string>> { new() { { "Text", text } } };
            var mediaTypeHeader = configuration["MicrosoftTranslator:MediaTypeHeader"]!;
            var content = CreateRequestBody(requestData, mediaTypeHeader);
            
            var url = $"{configuration["MicrosoftTranslator:URL"]}&to={toLanguageCode}";
            var response = await SendPostRequestAsync(url, content, cancellationToken);

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<string>.Success(responseString);
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
    
    public async Task<Result<string>> FetchAnswerResponseAsync(string question, CancellationToken cancellationToken)
    {
        try
        {
            ConfigureHeaders("CognitiveService:DefaultRequestHeaders");
            
            var requestData = new { question };
            var mediaTypeHeader = configuration["CognitiveService:MediaTypeHeader"]!;
            var content = CreateRequestBody(requestData, mediaTypeHeader);
            
            var url = configuration["CognitiveService:URL"]!;
            var response = await SendPostRequestAsync(url, content, cancellationToken);
            
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<string>.Success(responseString);
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
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

    private static StringContent CreateRequestBody<T>(T requestData, string mediaTypeHeader)
    {
        var json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8)
        {
            Headers =
            {
                ContentType = new MediaTypeHeaderValue(mediaTypeHeader)
            }
        };

        return content;
    }

    private async Task<HttpResponseMessage> SendPostRequestAsync(string url, HttpContent content, CancellationToken cancellationToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        var response = await httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        
        if (response == null)
        {
            throw new InvalidOperationException("The HTTP response is null.");
        }

        return response;
    }
}