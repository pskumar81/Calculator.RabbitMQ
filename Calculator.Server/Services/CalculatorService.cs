using Grpc.Core;
using Calculator.Server.Services.Interfaces;

namespace Calculator.Server.Services;

public class CalculatorServiceImpl : CalculatorService.CalculatorServiceBase, ICalculatorService
{
    private readonly ILogger<CalculatorServiceImpl> _logger;
    
    public CalculatorServiceImpl(ILogger<CalculatorServiceImpl> logger)
    {
        _logger = logger;
    }

    public override Task<CalculateReply> Add(CalculateRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Adding numbers: {request.Number1} + {request.Number2}");
        return Task.FromResult(new CalculateReply
        {
            Result = request.Number1 + request.Number2
        });
    }

    public override Task<CalculateReply> Subtract(CalculateRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Subtracting numbers: {request.Number1} - {request.Number2}");
        return Task.FromResult(new CalculateReply
        {
            Result = request.Number1 - request.Number2
        });
    }

    public override Task<CalculateReply> Multiply(CalculateRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Multiplying numbers: {request.Number1} * {request.Number2}");
        return Task.FromResult(new CalculateReply
        {
            Result = request.Number1 * request.Number2
        });
    }

    public override Task<CalculateReply> Divide(CalculateRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Dividing numbers: {request.Number1} / {request.Number2}");
        
        if (request.Number2 == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Cannot divide by zero"));
        }

        return Task.FromResult(new CalculateReply
        {
            Result = request.Number1 / request.Number2
        });
    }
}
