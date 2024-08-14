using Ardalis.Result;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.AnswerQuestion;

public class AnswerQuestionQueryHandler(
    IQuestionAnsweringService questionAnsweringService) : IRequestHandler<AnswerQuestionQuery, Result<AnswerQuestionResponse>>
{
    public async Task<Result<AnswerQuestionResponse>> Handle(AnswerQuestionQuery request,
        CancellationToken cancellationToken)
    {
        var result = await questionAnsweringService.AnswerQuestionAsync(request.Question,
            cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Result.CriticalError((string[])result.Errors);
        }
        
        var answer = result.Value.Answer;
        var answerResponse = new AnswerQuestionResponse(answer);
        
        return Result<AnswerQuestionResponse>.Success(answerResponse);
    }
}