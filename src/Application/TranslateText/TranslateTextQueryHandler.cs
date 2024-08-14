using System.Text.Json;
using Ardalis.Result;
using Infrastructure;
using MediatR;

namespace Application.TranslateText;

public class TranslateTextQueryHandler(
    IHttpClientService httpClientService) : IRequestHandler<TranslateTextQuery, Result<TranslateTextResponse>>
{
    public async Task<Result<TranslateTextResponse>> Handle(TranslateTextQuery request,
        CancellationToken cancellationToken)
    {
        var result = await httpClientService.TranslateTextAsync(request.Text,
            request.ToLanguageCode,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Result.CriticalError((string[])result.Errors);
        }
        
        var responseBody = result.Value;

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        if (root.GetArrayLength() == 0)
        {
            return Result.CriticalError("Response body is not in expected format: Expected array at root.");
        }

        var firstResult = root[0];

        var detectedLanguageCode = firstResult.GetProperty("detectedLanguage")
            .GetProperty("language")
            .GetString();

        var translationText = firstResult.GetProperty("translations")[0]
            .GetProperty("text")
            .GetString();

        if (detectedLanguageCode == null || translationText == null)
        {
            return Result.CriticalError("Unable to locate language code and/or translation in response body.");
        }

        var translationResponse = new TranslateTextResponse(detectedLanguageCode, translationText);
        return Result<TranslateTextResponse>.Success(translationResponse);
    }
}