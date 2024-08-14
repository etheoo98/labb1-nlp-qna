using System.Text.Json;
using Ardalis.Result;
using Infrastructure;
using MediatR;

namespace Application.AnswerQuestion;

public class AnswerQuestionQueryHandler(
    IHttpClientService httpClientService) : IRequestHandler<AnswerQuestionQuery, Result<AnswerQuestionResponse>>
{
    public async Task<Result<AnswerQuestionResponse>> Handle(AnswerQuestionQuery request,
        CancellationToken cancellationToken)
    {
        var result = await httpClientService.AnswerQuestionAsync(request.Question,
            cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Result.CriticalError((string[])result.Errors);
        }
        
        var responseBody = result.Value;

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        var answers = root.GetProperty("answers");
        var firstAnswer = answers.EnumerateArray().First();
        var answer = firstAnswer.GetProperty("answer").GetString();

        if (answer == null)
        {
            return Result.CriticalError("Response body does not contain property 'answer' in expected path.");
        }

        var answerResponse = new AnswerQuestionResponse(answer);
        return Result<AnswerQuestionResponse>.Success(answerResponse);
    }
}