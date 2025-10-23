using Calculator.Client.Services.Interfaces;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Calculator.Client.Services;

public class CalculatorClientService : ICalculatorClientService
{
    private readonly CalculatorService.CalculatorServiceClient _client;
    private readonly ILogger<CalculatorClientService> _logger;

    public CalculatorClientService(GrpcChannel channel, ILogger<CalculatorClientService> logger)
    {
        _client = new CalculatorService.CalculatorServiceClient(channel);
        _logger = logger;
    }

    public async Task<double> AddAsync(double num1, double num2)
    {
        _logger.LogInformation($"Calling Add operation with numbers: {num1}, {num2}");
        var reply = await _client.AddAsync(new CalculateRequest { Number1 = num1, Number2 = num2 });
        return reply.Result;
    }

    public async Task<double> SubtractAsync(double num1, double num2)
    {
        _logger.LogInformation($"Calling Subtract operation with numbers: {num1}, {num2}");
        var reply = await _client.SubtractAsync(new CalculateRequest { Number1 = num1, Number2 = num2 });
        return reply.Result;
    }

    public async Task<double> MultiplyAsync(double num1, double num2)
    {
        _logger.LogInformation($"Calling Multiply operation with numbers: {num1}, {num2}");
        var reply = await _client.MultiplyAsync(new CalculateRequest { Number1 = num1, Number2 = num2 });
        return reply.Result;
    }

    public async Task<double> DivideAsync(double num1, double num2)
    {
        _logger.LogInformation($"Calling Divide operation with numbers: {num1}, {num2}");
        var reply = await _client.DivideAsync(new CalculateRequest { Number1 = num1, Number2 = num2 });
        return reply.Result;
    }
}
