using Calculator.Server.Models;
using Calculator.Server.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Calculator.Server.Services;

/// <summary>
/// Implementation of calculator operations
/// </summary>
public class CalculatorService : ICalculatorService
{
    private readonly ILogger<CalculatorService> _logger;

    public CalculatorService(ILogger<CalculatorService> logger)
    {
        _logger = logger;
    }

    public async Task<CalculationResponse> CalculateAsync(CalculationRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new CalculationResponse
        {
            CorrelationId = request.CorrelationId,
            Operation = request.Operation
        };

        try
        {
            _logger.LogInformation("Processing calculation request: {Operation} for {Number1} and {Number2}, CorrelationId: {CorrelationId}",
                request.Operation, request.Number1, request.Number2, request.CorrelationId);

            var operation = request.Operation.ToOperation();
            
            response.Result = operation switch
            {
                CalculationOperation.Add => Add(request.Number1, request.Number2),
                CalculationOperation.Subtract => Subtract(request.Number1, request.Number2),
                CalculationOperation.Multiply => Multiply(request.Number1, request.Number2),
                CalculationOperation.Divide => Divide(request.Number1, request.Number2),
                _ => throw new ArgumentException($"Unsupported operation: {request.Operation}")
            };

            response.Success = true;
            
            _logger.LogInformation("Calculation completed: {Number1} {Symbol} {Number2} = {Result}, CorrelationId: {CorrelationId}",
                request.Number1, operation.ToSymbol(), request.Number2, response.Result, request.CorrelationId);
        }
        catch (DivideByZeroException)
        {
            response.Success = false;
            response.ErrorMessage = "Cannot divide by zero";
            _logger.LogWarning("Division by zero attempted: {Number1} / {Number2}, CorrelationId: {CorrelationId}",
                request.Number1, request.Number2, request.CorrelationId);
        }
        catch (ArgumentException ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Invalid operation requested: {Operation}, CorrelationId: {CorrelationId}",
                request.Operation, request.CorrelationId);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "An unexpected error occurred during calculation";
            _logger.LogError(ex, "Unexpected error during calculation, CorrelationId: {CorrelationId}",
                request.CorrelationId);
        }
        finally
        {
            stopwatch.Stop();
            response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
        }

        await Task.CompletedTask; // Async for future extensibility
        return response;
    }

    public double Add(double num1, double num2)
    {
        return num1 + num2;
    }

    public double Subtract(double num1, double num2)
    {
        return num1 - num2;
    }

    public double Multiply(double num1, double num2)
    {
        return num1 * num2;
    }

    public double Divide(double num1, double num2)
    {
        if (num2 == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero");
        }
        return num1 / num2;
    }
}