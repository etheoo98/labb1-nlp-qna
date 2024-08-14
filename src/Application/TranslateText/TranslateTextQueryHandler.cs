using Ardalis.Result;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.TranslateText;

public class TranslateTextQueryHandler(
    ITextTranslationService textTranslationService) : IRequestHandler<TranslateTextQuery, Result<TranslateTextResponse>>
{
    public async Task<Result<TranslateTextResponse>> Handle(TranslateTextQuery request,
        CancellationToken cancellationToken)
    {
        var result = await textTranslationService.TranslateTextAsync(request.Text, 
            request.ToLanguageCode, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result.CriticalError((string[])result.Errors);
        }

        var (detectedLanguageCode, translationText) = result.Value;
        var translationResponse = new TranslateTextResponse(detectedLanguageCode, translationText);
        
        return Result<TranslateTextResponse>.Success(translationResponse);
    }
}