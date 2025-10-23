using Calculator.Server.Models;

namespace Calculator.Server.Services.Interfaces;

/// <summary>
/// Interface for RabbitMQ message consumer service
/// </summary>
public interface IRabbitMQConsumerService : IDisposable
{
    /// <summary>
    /// Starts consuming messages from the request queue
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for stopping consumption</param>
    Task StartConsumingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Stops consuming messages
    /// </summary>
    Task StopConsumingAsync();

    /// <summary>
    /// Sets up the required queues and exchanges
    /// </summary>
    void SetupQueuesAndExchanges();
}