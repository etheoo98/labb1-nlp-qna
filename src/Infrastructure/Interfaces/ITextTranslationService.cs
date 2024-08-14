using Ardalis.Result;
using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface ITextTranslationService
{
    Task<Result<TranslationResult>> TranslateTextAsync(string text, string toLanguageCode, CancellationToken cancellationToken);
}