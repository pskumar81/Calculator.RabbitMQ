using Calculator.Client.Services;
using Calculator.Client.Services.Interfaces;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Calculator.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorClient(this IServiceCollection services, string serverUrl)
    {
        var channel = GrpcChannel.ForAddress(serverUrl, new GrpcChannelOptions
        {
            Credentials = Grpc.Core.ChannelCredentials.Insecure
        });

        services.AddSingleton(channel);
        services.AddScoped<ICalculatorClientService, CalculatorClientService>();
        
        return services;
    }
}
