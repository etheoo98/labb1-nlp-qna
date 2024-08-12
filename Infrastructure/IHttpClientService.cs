using Ardalis.Result;
using Core;

namespace Infrastructure;

public interface IHttpClientService
{ 
    Task<Result<TranslationResponse>> TranslateTextAsync(string text, string toLanguageCode, CancellationToken cancellationToken);
    Task<Result<AnswerResponse>> AnswerQuestionAsync(string question, CancellationToken cancellationToken);
}