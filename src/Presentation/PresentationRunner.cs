using Application.AnswerQuestion;
using Application.TranslateText;
using Ardalis.Result;
using MediatR;
using Spectre.Console;

namespace Presentation;

public class PresentationRunner(ISender sender)
{
    public async Task Run()
    {
        while (true)
        {
            var question = AnsiConsole.Prompt(new TextPrompt<string>("[deepskyblue1]You[/]:"));

            if (question.ToLower() == "quit" || question.ToLower() == "exit")
            {
                break;
            }

            var answer = await HandleUserQueryAsync(question);
            AnsiConsole.MarkupLine($"[orange1]Bot[/]: {answer}\n");
        }
    }

    private async Task<string> HandleUserQueryAsync(string question)
    {
        var answer = string.Empty;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("springgreen3"))
            .StartAsync("Loading",
                async ctx =>
                {
                    ctx.Status("Detecting question language");
                    var (detectedLanguageCode, translatedQuestion) = await Translate(question);

                    ctx.Status("Retrieving answer");
                    var answerResponse = await AnswerQuestion(translatedQuestion);
                    answer = answerResponse.Answer;

                    if (detectedLanguageCode != "en")
                    {
                        ctx.Status("Translating answer into question language");
                        var translatedAnswer = await Translate(answer,
                            detectedLanguageCode);

                        answer = translatedAnswer.Translation;
                    }
                });

        return answer;
    }

    private async Task<TranslateTextResponse> Translate(string text, string toLanguageCode = "en")
    {
        var request = new TranslateTextQuery(text, toLanguageCode);
        var result = await sender.Send(request);
        Guard(result);
        return result;
    }

    private async Task<AnswerQuestionResponse> AnswerQuestion(string question)
    {
        var command = new AnswerQuestionQuery(question);
        var result = await sender.Send(command);
        Guard(result);
        return result;
    }

    private static void Guard<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            AnsiConsole.WriteLine(error);
        }

        Environment.Exit(0);
    }
}