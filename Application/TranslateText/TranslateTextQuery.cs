using Ardalis.Result;
using MediatR;

namespace Application.TranslateText;

public sealed record TranslateTextQuery(
    string Text, 
    string ToLanguageCode) : IRequest<Result<TranslateTextResponse>>;