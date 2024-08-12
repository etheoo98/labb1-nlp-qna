using Application.AnswerQuestion;
using Application.TranslateText;
using Ardalis.Result;
using Core;
using MediatR;

namespace Presentation;

public class PresentationRunner(
    ISender sender)
{
    public async Task Run()
    {

        do
        {
            Console.Clear();
            
            var question = GetUserQuestion();
            var translationResponse = await Translate(question);
            var answerResponse = await AnswerQuestion(translationResponse.Translation);

            if (translationResponse.DetectedLanguageCode != "en")
            {
                var translatedAnswer = await Translate(answerResponse.Value.Answer,
                    translationResponse.DetectedLanguageCode);
                Console.WriteLine(translatedAnswer.Translation);
            }
            else
            {
                Console.WriteLine(answerResponse.Value.Answer);
            }

            Console.Write("Do you want to ask another question? [Y/n] ");
        } while (Console.ReadKey().KeyChar != 'n');
    }

    private static string GetUserQuestion()
    {
        while (true)
        {
            Console.Write("Your question: ");
            var question = Console.ReadLine();
            if (!string.IsNullOrEmpty(question))
            {
                return question;
            }
        }
    }

    private async Task<TranslationResponse> Translate(string text,
        string toLanguageCode = "en")
    {
        var request = new TranslateTextQuery(text,
            toLanguageCode);
        var result = await sender.Send(request);
        Guard(result);
        return result.Value;
    }

    private async Task<Result<AnswerResponse>> AnswerQuestion(string question)
    {
        var command = new AnswerQuestionQuery(question);
        var result = await sender.Send(command);
        Guard(result);
        return result.Value;
    }

    private static void Guard<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        foreach (var error in result.Errors)
        {
            Console.WriteLine(error);
        }

        Environment.Exit(0);
    }
}