using Ardalis.Result;
using Azure;
using Azure.AI.Language.QuestionAnswering;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Azure;

public class AzureQuestionAnsweringService(IConfiguration configuration) : IQuestionAnsweringService
{
    public async Task<Result<AnswerQuestionResult>> AnswerQuestionAsync(string question,
        CancellationToken cancellationToken)
    {
        try
        {
            var credential = new AzureKeyCredential(configuration["Azure:CognitiveService:Ocp-Apim-Subscription-Key"]!);
            var uri = new Uri(configuration["Azure:CognitiveService:URL"]!);
            var project = new QuestionAnsweringProject(configuration["Azure:CognitiveService:ProjectName"],
                configuration["Azure:CognitiveService:DeploymentName"]);

            var client = new QuestionAnsweringClient(uri, credential);
            var response = await client.GetAnswersAsync(question, project, cancellationToken: cancellationToken);

            if (response?.Value.Answers == null || string.IsNullOrEmpty(response.Value.Answers[0].Answer))
            {
                return Result.CriticalError("Unexpected format in Azure Question Answering response");
            }

            var answer = response.Value.Answers[0].Answer;
            var result = new AnswerQuestionResult(answer);
            
            return Result<AnswerQuestionResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result.CriticalError(ex.Message);
        }
    }
}