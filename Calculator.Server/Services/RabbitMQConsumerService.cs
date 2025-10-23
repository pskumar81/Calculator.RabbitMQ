using Calculator.Server.Models;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Calculator.Server.Services;

/// <summary>
/// RabbitMQ consumer service for processing calculation requests
/// </summary>
public class RabbitMQConsumerService : IRabbitMQConsumerService
{
    private readonly IRabbitMQConnectionService _connectionService;
    private readonly ICalculatorService _calculatorService;
    private readonly RabbitMQConfiguration _config;
    private readonly ILogger<RabbitMQConsumerService> _logger;
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;
    private string? _consumerTag;
    private bool _disposed = false;

    public RabbitMQConsumerService(
        IRabbitMQConnectionService connectionService,
        ICalculatorService calculatorService,
        IOptions<RabbitMQConfiguration> config,
        ILogger<RabbitMQConsumerService> logger)
    {
        _connectionService = connectionService;
        _calculatorService = calculatorService;
        _config = config.Value;
        _logger = logger;
    }

    public void SetupQueuesAndExchanges()
    {
        try
        {
            _channel = _connectionService.CreateChannel();

            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: _config.ExchangeName,
                type: _config.ExchangeType,
                durable: _config.Durable,
                autoDelete: _config.AutoDelete);

            // Declare request queue
            _channel.QueueDeclare(
                queue: _config.RequestQueueName,
                durable: _config.Durable,
                exclusive: _config.Exclusive,
                autoDelete: _config.AutoDelete,
                arguments: null);

            // Bind request queue to exchange
            _channel.QueueBind(
                queue: _config.RequestQueueName,
                exchange: _config.ExchangeName,
                routingKey: _config.RequestQueueName);

            // Set QoS to process one message at a time
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation("RabbitMQ queues and exchanges setup completed");
            _logger.LogInformation("Exchange: {ExchangeName}, Request Queue: {RequestQueue}",
                _config.ExchangeName, _config.RequestQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ queues and exchanges");
            throw;
        }
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_channel == null)
            {
                SetupQueuesAndExchanges();
            }

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += async (model, args) => await OnMessageReceived(model, args);

            _consumerTag = _channel!.BasicConsume(
                queue: _config.RequestQueueName,
                autoAck: false, // Manual acknowledgment
                consumer: _consumer);

            _logger.LogInformation("Started consuming messages from queue: {QueueName}", _config.RequestQueueName);

            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Message consumption was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages");
            throw;
        }
    }

    public async Task StopConsumingAsync()
    {
        try
        {
            if (_consumerTag != null && _channel != null)
            {
                _channel.BasicCancel(_consumerTag);
                _consumerTag = null;
                _logger.LogInformation("Stopped consuming messages");
            }
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping message consumption");
        }
    }

    private async Task OnMessageReceived(object? sender, BasicDeliverEventArgs args)
    {
        var correlationId = string.Empty;
        
        try
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogDebug("Received message: {Message}", message);

            var request = JsonSerializer.Deserialize<CalculationRequest>(message);
            if (request == null)
            {
                _logger.LogError("Failed to deserialize calculation request");
                _channel?.BasicNack(args.DeliveryTag, false, false); // Reject and don't requeue
                return;
            }

            correlationId = request.CorrelationId;
            _logger.LogInformation("Processing calculation request with CorrelationId: {CorrelationId}", correlationId);

            // Process the calculation
            var response = await _calculatorService.CalculateAsync(request);

            // Send response back to client
            await SendResponseAsync(request.ReplyTo, response, args.BasicProperties.CorrelationId);

            // Acknowledge the message
            _channel?.BasicAck(args.DeliveryTag, false);
            
            _logger.LogInformation("Successfully processed calculation request with CorrelationId: {CorrelationId}", correlationId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message, CorrelationId: {CorrelationId}", correlationId);
            _channel?.BasicNack(args.DeliveryTag, false, false); // Reject and don't requeue
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message, CorrelationId: {CorrelationId}", correlationId);
            
            // Send error response
            try
            {
                var errorResponse = new CalculationResponse
                {
                    CorrelationId = correlationId,
                    Success = false,
                    ErrorMessage = "Internal server error occurred while processing the request"
                };

                var request = JsonSerializer.Deserialize<CalculationRequest>(Encoding.UTF8.GetString(args.Body.ToArray()));
                if (request != null && !string.IsNullOrEmpty(request.ReplyTo))
                {
                    await SendResponseAsync(request.ReplyTo, errorResponse, args.BasicProperties.CorrelationId);
                }
            }
            catch (Exception responseEx)
            {
                _logger.LogError(responseEx, "Failed to send error response, CorrelationId: {CorrelationId}", correlationId);
            }

            _channel?.BasicNack(args.DeliveryTag, false, false);
        }
    }

    private async Task SendResponseAsync(string replyToQueue, CalculationResponse response, string? correlationId)
    {
        try
        {
            if (string.IsNullOrEmpty(replyToQueue))
            {
                _logger.LogWarning("No reply-to queue specified for response, CorrelationId: {CorrelationId}", response.CorrelationId);
                return;
            }

            var responseJson = JsonSerializer.Serialize(response);
            var responseBody = Encoding.UTF8.GetBytes(responseJson);

            var properties = _channel!.CreateBasicProperties();
            properties.CorrelationId = correlationId ?? response.CorrelationId;
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: "",
                routingKey: replyToQueue,
                basicProperties: properties,
                body: responseBody);

            _logger.LogDebug("Sent response to queue {ReplyToQueue}, CorrelationId: {CorrelationId}", 
                replyToQueue, response.CorrelationId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send response, CorrelationId: {CorrelationId}", response.CorrelationId);
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            StopConsumingAsync().Wait(TimeSpan.FromSeconds(5));
            _channel?.Close();
            _channel?.Dispose();
            _logger.LogInformation("RabbitMQ consumer service disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ consumer service");
        }
        finally
        {
            _disposed = true;
        }
    }
}