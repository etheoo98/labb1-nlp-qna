using Ardalis.Result;
using Infrastructure.Models;

namespace Infrastructure.Interfaces;

public interface IQuestionAnsweringService
{
    Task<Result<AnswerQuestionResult>> AnswerQuestionAsync(string question, CancellationToken cancellationToken);
}