using Calculator.Server.Models;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Calculator.Server.Services;

/// <summary>
/// Service for managing RabbitMQ connections
/// </summary>
public class RabbitMQConnectionService : IRabbitMQConnectionService
{
    private readonly RabbitMQConfiguration _config;
    private readonly ILogger<RabbitMQConnectionService> _logger;
    private IConnection? _connection;
    private IConnectionFactory? _connectionFactory;
    private bool _disposed = false;

    public RabbitMQConnectionService(IOptions<RabbitMQConfiguration> config, ILogger<RabbitMQConnectionService> logger)
    {
        _config = config.Value;
        _logger = logger;
        CreateConnectionFactory();
        CreateConnection();
    }

    public IConnection Connection => _connection ?? throw new InvalidOperationException("RabbitMQ connection is not established");

    public bool IsConnected => _connection?.IsOpen == true;

    public IModel CreateChannel()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Cannot create channel - RabbitMQ connection is not open");
        }

        return Connection.CreateModel();
    }

    public bool TryReconnect()
    {
        try
        {
            _logger.LogInformation("Attempting to reconnect to RabbitMQ...");
            
            _connection?.Close();
            _connection?.Dispose();
            
            CreateConnection();
            
            _logger.LogInformation("Successfully reconnected to RabbitMQ");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reconnect to RabbitMQ");
            return false;
        }
    }

    private void CreateConnectionFactory()
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password,
            VirtualHost = _config.VirtualHost,
            RequestedConnectionTimeout = TimeSpan.FromMilliseconds(_config.ConnectionTimeoutMs),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _logger.LogInformation("RabbitMQ connection factory created for {HostName}:{Port}", _config.HostName, _config.Port);
    }

    private void CreateConnection()
    {
        try
        {
            _connection = _connectionFactory!.CreateConnection("Calculator-Server");
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _connection.CallbackException += OnCallbackException;
            _connection.ConnectionBlocked += OnConnectionBlocked;
            _connection.ConnectionUnblocked += OnConnectionUnblocked;

            _logger.LogInformation("Successfully connected to RabbitMQ at {HostName}:{Port}", _config.HostName, _config.Port);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ broker at {HostName}:{Port}", _config.HostName, _config.Port);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error connecting to RabbitMQ");
            throw;
        }
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shutdown: {Reason}", e.ReplyText);
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "RabbitMQ callback exception occurred");
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection blocked: {Reason}", e.Reason);
    }

    private void OnConnectionUnblocked(object? sender, EventArgs e)
    {
        _logger.LogInformation("RabbitMQ connection unblocked");
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ connection disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
        finally
        {
            _disposed = true;
        }
    }
}