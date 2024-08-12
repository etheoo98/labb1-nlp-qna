using Infrastructure;
using Ardalis.Result;
using Core;
using MediatR;

namespace Application.TranslateText;

public class TranslateTextQueryHandler(IHttpClientService httpClientService) : IRequestHandler<TranslateTextQuery, Result<TranslationResponse>>
{
    public async Task<Result<TranslationResponse>> Handle(TranslateTextQuery request, CancellationToken cancellationToken)
    {
        return await httpClientService.TranslateTextAsync(request.Text, request.ToLanguageCode, cancellationToken);
    }
}