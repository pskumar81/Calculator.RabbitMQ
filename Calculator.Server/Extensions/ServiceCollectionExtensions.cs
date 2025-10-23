using Calculator.Server.Services;
using Calculator.Server.Services.Interfaces;

namespace Calculator.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCalculatorServices(this IServiceCollection services)
    {
        services.AddGrpc();
        services.AddScoped<ICalculatorService, CalculatorServiceImpl>();

        return services;
    }
}
