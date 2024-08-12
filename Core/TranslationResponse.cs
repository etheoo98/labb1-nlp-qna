namespace Core;

public sealed record TranslationResponse(
    string DetectedLanguageCode, 
    string Translation);