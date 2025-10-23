using Calculator.Client.Models;
using Calculator.Client.Services;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Client.Extensions;

/// <summary>
/// Extension methods for service collection configuration in the client
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all calculator client services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure RabbitMQ settings
        services.Configure<RabbitMQConfiguration>(configuration.GetSection("RabbitMQ"));

        // Register services
        services.AddSingleton<IRabbitMQConnectionService, RabbitMQConnectionService>();
        services.AddScoped<ICalculatorClientService, CalculatorClientService>();

        return services;
    }

    /// <summary>
    /// Adds calculator client services with custom RabbitMQ configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureRabbitMQ">Action to configure RabbitMQ settings</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorClientServices(this IServiceCollection services, Action<RabbitMQConfiguration> configureRabbitMQ)
    {
        // Configure RabbitMQ settings
        services.Configure(configureRabbitMQ);

        // Register services
        services.AddSingleton<IRabbitMQConnectionService, RabbitMQConnectionService>();
        services.AddScoped<ICalculatorClientService, CalculatorClientService>();

        return services;
    }

    /// <summary>
    /// Adds calculator client services with environment-based RabbitMQ configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="rabbitMQHost">RabbitMQ host (can be overridden by environment variable RABBITMQ_HOST)</param>
    /// <param name="rabbitMQPort">RabbitMQ port (can be overridden by environment variable RABBITMQ_PORT)</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorClientServices(this IServiceCollection services, string? rabbitMQHost = null, int? rabbitMQPort = null)
    {
        services.Configure<RabbitMQConfiguration>(config =>
        {
            // Use environment variables if available, otherwise use provided values or defaults
            config.HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? rabbitMQHost ?? "localhost";
            config.Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var port) ? port : (rabbitMQPort ?? 5672);
            config.UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest";
            config.Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";
            config.VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? "/";
        });

        // Register services
        services.AddSingleton<IRabbitMQConnectionService, RabbitMQConnectionService>();
        services.AddScoped<ICalculatorClientService, CalculatorClientService>();

        return services;
    }
}