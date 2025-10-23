using Calculator.Client.Models;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Calculator.Client.Services;

/// <summary>
/// RabbitMQ-based calculator client service implementation
/// </summary>
public class CalculatorClientService : ICalculatorClientService
{
    private readonly IRabbitMQConnectionService _connectionService;
    private readonly RabbitMQConfiguration _config;
    private readonly ILogger<CalculatorClientService> _logger;
    private readonly string _clientId;
    private readonly string _responseQueueName;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<CalculationResponse>> _pendingRequests;
    
    private IModel? _channel;
    private EventingBasicConsumer? _consumer;
    private bool _disposed = false;

    public CalculatorClientService(
        IRabbitMQConnectionService connectionService,
        IOptions<RabbitMQConfiguration> config,
        ILogger<CalculatorClientService> logger)
    {
        _connectionService = connectionService;
        _config = config.Value;
        _logger = logger;
        _clientId = Guid.NewGuid().ToString("N")[..8]; // Short unique client ID
        _responseQueueName = $"{_config.ResponseQueuePrefix}.{_clientId}";
        _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<CalculationResponse>>();
        
        SetupQueuesAndExchanges();
        SetupResponseConsumer();
    }

    public void SetupQueuesAndExchanges()
    {
        try
        {
            _channel = _connectionService.CreateChannel();

            // Declare exchange (server should have created it, but ensure it exists)
            _channel.ExchangeDeclare(
                exchange: _config.ExchangeName,
                type: _config.ExchangeType,
                durable: _config.Durable,
                autoDelete: _config.AutoDelete);

            // Declare request queue (server should have created it, but ensure it exists)
            _channel.QueueDeclare(
                queue: _config.RequestQueueName,
                durable: _config.Durable,
                exclusive: false,
                autoDelete: _config.AutoDelete,
                arguments: null);

            // Declare exclusive response queue for this client
            _channel.QueueDeclare(
                queue: _responseQueueName,
                durable: false,
                exclusive: true, // Exclusive to this connection
                autoDelete: true, // Auto-delete when client disconnects
                arguments: null);

            _logger.LogInformation("Client queues setup completed. ClientId: {ClientId}, ResponseQueue: {ResponseQueue}",
                _clientId, _responseQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup client queues and exchanges");
            throw;
        }
    }

    private void SetupResponseConsumer()
    {
        try
        {
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += OnResponseReceived;

            _channel!.BasicConsume(
                queue: _responseQueueName,
                autoAck: true, // Auto-ack responses
                consumer: _consumer);

            _logger.LogInformation("Response consumer setup completed for queue: {ResponseQueue}", _responseQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup response consumer");
            throw;
        }
    }

    public async Task<double> AddAsync(double num1, double num2)
    {
        return await SendCalculationRequestAsync(CalculationOperation.Add, num1, num2);
    }

    public async Task<double> SubtractAsync(double num1, double num2)
    {
        return await SendCalculationRequestAsync(CalculationOperation.Subtract, num1, num2);
    }

    public async Task<double> MultiplyAsync(double num1, double num2)
    {
        return await SendCalculationRequestAsync(CalculationOperation.Multiply, num1, num2);
    }

    public async Task<double> DivideAsync(double num1, double num2)
    {
        return await SendCalculationRequestAsync(CalculationOperation.Divide, num1, num2);
    }

    private async Task<double> SendCalculationRequestAsync(CalculationOperation operation, double num1, double num2)
    {
        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<CalculationResponse>();
        
        try
        {
            // Register the pending request
            _pendingRequests[correlationId] = tcs;

            var request = new CalculationRequest
            {
                CorrelationId = correlationId,
                Operation = operation.ToStringValue(),
                Number1 = num1,
                Number2 = num2,
                ReplyTo = _responseQueueName
            };

            _logger.LogInformation("Sending calculation request: {Operation} {Number1} {Symbol} {Number2}, CorrelationId: {CorrelationId}",
                operation.ToStringValue(), num1, operation.ToSymbol(), num2, correlationId);

            // Serialize and send the request
            var requestJson = JsonSerializer.Serialize(request);
            var requestBody = Encoding.UTF8.GetBytes(requestJson);

            var properties = _channel!.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = _responseQueueName;
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: _config.ExchangeName,
                routingKey: _config.RequestQueueName,
                basicProperties: properties,
                body: requestBody);

            _logger.LogDebug("Request published to exchange: {ExchangeName}, routingKey: {RequestQueue}",
                _config.ExchangeName, _config.RequestQueueName);

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_config.RequestTimeoutMs));
            var cancellationTask = Task.Delay(_config.RequestTimeoutMs, cts.Token);
            var completedTask = await Task.WhenAny(tcs.Task, cancellationTask);

            if (completedTask == cancellationTask)
            {
                _pendingRequests.TryRemove(correlationId, out _);
                throw new TimeoutException($"Request timed out after {_config.RequestTimeoutMs}ms");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                throw new InvalidOperationException($"Calculation failed: {response.ErrorMessage}");
            }

            _logger.LogInformation("Calculation completed: {Number1} {Symbol} {Number2} = {Result}, ProcessingTime: {ProcessingTime}ms",
                num1, operation.ToSymbol(), num2, response.Result, response.ProcessingTimeMs);

            return response.Result;
        }
        catch (Exception ex)
        {
            _pendingRequests.TryRemove(correlationId, out _);
            _logger.LogError(ex, "Error sending calculation request, CorrelationId: {CorrelationId}", correlationId);
            throw;
        }
    }

    private void OnResponseReceived(object? sender, BasicDeliverEventArgs args)
    {
        try
        {
            var body = args.Body.ToArray();
            var responseJson = Encoding.UTF8.GetString(body);
            
            _logger.LogDebug("Received response: {ResponseJson}", responseJson);

            var response = JsonSerializer.Deserialize<CalculationResponse>(responseJson);
            if (response == null)
            {
                _logger.LogError("Failed to deserialize calculation response");
                return;
            }

            var correlationId = response.CorrelationId;
            
            if (_pendingRequests.TryRemove(correlationId, out var tcs))
            {
                tcs.SetResult(response);
                _logger.LogDebug("Response processed for CorrelationId: {CorrelationId}", correlationId);
            }
            else
            {
                _logger.LogWarning("Received response for unknown CorrelationId: {CorrelationId}", correlationId);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing response message");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            // Cancel all pending requests
            foreach (var kvp in _pendingRequests)
            {
                kvp.Value.TrySetCanceled();
            }
            _pendingRequests.Clear();

            _channel?.Close();
            _channel?.Dispose();
            
            _logger.LogInformation("Calculator client service disposed. ClientId: {ClientId}", _clientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing calculator client service");
        }
        finally
        {
            _disposed = true;
        }
    }
}