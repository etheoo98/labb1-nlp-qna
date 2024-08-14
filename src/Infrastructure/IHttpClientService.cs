using Ardalis.Result;

namespace Infrastructure;

public interface IHttpClientService
{ 
    Task<Result<string>> TranslateTextAsync(string text, string toLanguageCode, CancellationToken cancellationToken);
    Task<Result<string>> AnswerQuestionAsync(string question, CancellationToken cancellationToken);
}