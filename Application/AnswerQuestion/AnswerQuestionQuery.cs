using Ardalis.Result;
using Core;
using MediatR;

namespace Application.AnswerQuestion;

public sealed record AnswerQuestionQuery(
    string Question) : IRequest<Result<AnswerResponse>>;