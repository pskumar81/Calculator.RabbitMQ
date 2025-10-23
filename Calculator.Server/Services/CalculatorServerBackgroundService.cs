using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Calculator.Server.Services;

/// <summary>
/// Background service that hosts the RabbitMQ consumer
/// </summary>
public class CalculatorServerBackgroundService : BackgroundService
{
    private readonly IRabbitMQConsumerService _consumerService;
    private readonly ILogger<CalculatorServerBackgroundService> _logger;

    public CalculatorServerBackgroundService(
        IRabbitMQConsumerService consumerService,
        ILogger<CalculatorServerBackgroundService> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Calculator Server Background Service starting...");
            
            // Setup queues and exchanges
            _consumerService.SetupQueuesAndExchanges();
            
            _logger.LogInformation("Calculator Server is ready to process calculation requests");
            
            // Start consuming messages
            await _consumerService.StartConsumingAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Calculator Server Background Service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Calculator Server Background Service");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculator Server Background Service stopping...");
        
        try
        {
            await _consumerService.StopConsumingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping consumer service");
        }
        
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Calculator Server Background Service stopped");
    }

    public override void Dispose()
    {
        try
        {
            _consumerService?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing consumer service");
        }
        
        base.Dispose();
    }
}