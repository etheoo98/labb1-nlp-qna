using Ardalis.Result;

namespace Infrastructure;

public interface IHttpClientService
{ 
    Task<Result<string>> FetchTranslationResponseAsync(string text, string toLanguageCode, CancellationToken cancellationToken);
    Task<Result<string>> FetchAnswerResponseAsync(string question, CancellationToken cancellationToken);
}