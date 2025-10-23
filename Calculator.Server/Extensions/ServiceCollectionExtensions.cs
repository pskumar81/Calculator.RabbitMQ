using Calculator.Server.Models;
using Calculator.Server.Services;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Server.Extensions;

/// <summary>
/// Extension methods for service collection configuration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all calculator server services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure RabbitMQ settings
        services.Configure<RabbitMQConfiguration>(configuration.GetSection("RabbitMQ"));

        // Register services
        services.AddSingleton<IRabbitMQConnectionService, RabbitMQConnectionService>();
        services.AddScoped<ICalculatorService, CalculatorService>();
        services.AddScoped<IRabbitMQConsumerService, RabbitMQConsumerService>();
        
        // Register background service
        services.AddHostedService<CalculatorServerBackgroundService>();

        return services;
    }

    /// <summary>
    /// Adds calculator server services with custom RabbitMQ configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureRabbitMQ">Action to configure RabbitMQ settings</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCalculatorServerServices(this IServiceCollection services, Action<RabbitMQConfiguration> configureRabbitMQ)
    {
        // Configure RabbitMQ settings
        services.Configure(configureRabbitMQ);

        // Register services
        services.AddSingleton<IRabbitMQConnectionService, RabbitMQConnectionService>();
        services.AddScoped<ICalculatorService, CalculatorService>();
        services.AddScoped<IRabbitMQConsumerService, RabbitMQConsumerService>();
        
        // Register background service
        services.AddHostedService<CalculatorServerBackgroundService>();

        return services;
    }
}