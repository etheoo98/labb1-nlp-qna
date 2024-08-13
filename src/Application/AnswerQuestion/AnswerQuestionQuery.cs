using Ardalis.Result;
using MediatR;

namespace Application.AnswerQuestion;

public sealed record AnswerQuestionQuery(
    string Question) : IRequest<Result<AnswerQuestionResponse>>;