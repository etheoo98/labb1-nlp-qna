namespace Infrastructure.Models;

public sealed record TranslationResult(
    string DetectedLanguageCode, 
    string TranslatedText);