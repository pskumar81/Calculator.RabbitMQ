using Calculator.Server.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add all calculator server services including RabbitMQ
        services.AddCalculatorServerServices(context.Configuration);
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Calculator Server starting...");
    logger.LogInformation("Press Ctrl+C to shutdown the server");
    
    await host.RunAsync();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Calculator Server terminated unexpectedly");
    throw;
}
finally
{
    logger.LogInformation("Calculator Server shutdown complete");
}
