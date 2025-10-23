using Grpc.Core;

namespace Calculator.Server.Services.Interfaces;

public interface ICalculatorService
{
    Task<CalculateReply> Add(CalculateRequest request, ServerCallContext context);
    Task<CalculateReply> Subtract(CalculateRequest request, ServerCallContext context);
    Task<CalculateReply> Multiply(CalculateRequest request, ServerCallContext context);
    Task<CalculateReply> Divide(CalculateRequest request, ServerCallContext context);
}
