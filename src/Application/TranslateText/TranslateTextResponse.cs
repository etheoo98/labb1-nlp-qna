namespace Application.TranslateText;

public sealed record TranslateTextResponse(
    string DetectedLanguageCode, 
    string Translation);