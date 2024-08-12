using Ardalis.Result;
using Core;
using Infrastructure;
using MediatR;

namespace Application.AnswerQuestion;

public class AnswerQuestionQueryHandler(IHttpClientService httpClientService) : IRequestHandler<AnswerQuestionQuery, Result<AnswerResponse>>
{
    public async Task<Result<AnswerResponse>> Handle(AnswerQuestionQuery request, CancellationToken cancellationToken)
    {
        return await httpClientService.AnswerQuestionAsync(request.Question, cancellationToken);
    }
}