using Application;
using Infrastructure.Interfaces;
using Infrastructure.Services.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentation;

var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
{
    services.AddApplication();
    services.AddTransient<PresentationRunner>();
    services.AddTransient<ITextTranslationService, AzureTranslationService>();
    services.AddTransient<IQuestionAnsweringService, AzureQuestionAnsweringService>();
}).ConfigureLogging(logging =>
{
    logging.ClearProviders();
}).Build();

var presentationRunner = host.Services.GetRequiredService<PresentationRunner>();
await presentationRunner.Run();