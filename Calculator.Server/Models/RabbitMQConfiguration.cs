namespace Calculator.Server.Models;

/// <summary>
/// Configuration settings for RabbitMQ connection and queues
/// </summary>
public class RabbitMQConfiguration
{
    /// <summary>
    /// RabbitMQ server hostname
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ server port
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Username for RabbitMQ authentication
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Password for RabbitMQ authentication
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Virtual host for RabbitMQ
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Name of the request queue
    /// </summary>
    public string RequestQueueName { get; set; } = "calculator.requests";

    /// <summary>
    /// Name of the response queue prefix (will be appended with client ID)
    /// </summary>
    public string ResponseQueuePrefix { get; set; } = "calculator.responses";

    /// <summary>
    /// Name of the exchange for routing messages
    /// </summary>
    public string ExchangeName { get; set; } = "calculator.exchange";

    /// <summary>
    /// Exchange type (direct, topic, fanout, headers)
    /// </summary>
    public string ExchangeType { get; set; } = "direct";

    /// <summary>
    /// Whether queues should be durable (survive broker restart)
    /// </summary>
    public bool Durable { get; set; } = true;

    /// <summary>
    /// Whether queues should be exclusive to the connection
    /// </summary>
    public bool Exclusive { get; set; } = false;

    /// <summary>
    /// Whether queues should auto-delete when no longer used
    /// </summary>
    public bool AutoDelete { get; set; } = false;

    /// <summary>
    /// Request timeout in milliseconds
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 30000; // 30 seconds

    /// <summary>
    /// Connection timeout in milliseconds
    /// </summary>
    public int ConnectionTimeoutMs { get; set; } = 5000; // 5 seconds
}