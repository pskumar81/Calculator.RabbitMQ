using RabbitMQ.Client;

namespace Calculator.Server.Services.Interfaces;

/// <summary>
/// Interface for managing RabbitMQ connections
/// </summary>
public interface IRabbitMQConnectionService : IDisposable
{
    /// <summary>
    /// Gets the RabbitMQ connection
    /// </summary>
    IConnection Connection { get; }

    /// <summary>
    /// Creates a new channel for RabbitMQ operations
    /// </summary>
    /// <returns>A new RabbitMQ channel</returns>
    IModel CreateChannel();

    /// <summary>
    /// Checks if the connection is open and healthy
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Attempts to reconnect to RabbitMQ
    /// </summary>
    /// <returns>True if reconnection was successful</returns>
    bool TryReconnect();
}