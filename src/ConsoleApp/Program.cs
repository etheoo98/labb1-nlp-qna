using Application;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentation;

var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
{
    services.AddApplication();
    services.AddTransient<PresentationRunner>();
    services.AddHttpClient<IHttpClientService, HttpClientService>();
}).ConfigureLogging(logging =>
{
    logging.ClearProviders();
}).Build();

var presentationRunner = host.Services.GetRequiredService<PresentationRunner>();
await presentationRunner.Run();