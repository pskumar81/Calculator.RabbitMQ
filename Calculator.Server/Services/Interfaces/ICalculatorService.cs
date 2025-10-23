using Calculator.Server.Models;

namespace Calculator.Server.Services.Interfaces;

/// <summary>
/// Interface for calculator operations
/// </summary>
public interface ICalculatorService
{
    /// <summary>
    /// Performs a calculation based on the request
    /// </summary>
    /// <param name="request">The calculation request</param>
    /// <returns>The calculation response</returns>
    Task<CalculationResponse> CalculateAsync(CalculationRequest request);

    /// <summary>
    /// Adds two numbers
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <returns>Sum of the numbers</returns>
    double Add(double num1, double num2);

    /// <summary>
    /// Subtracts second number from first
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <returns>Difference of the numbers</returns>
    double Subtract(double num1, double num2);

    /// <summary>
    /// Multiplies two numbers
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <returns>Product of the numbers</returns>
    double Multiply(double num1, double num2);

    /// <summary>
    /// Divides first number by second
    /// </summary>
    /// <param name="num1">First number (dividend)</param>
    /// <param name="num2">Second number (divisor)</param>
    /// <returns>Quotient of the numbers</returns>
    /// <exception cref="DivideByZeroException">Thrown when divisor is zero</exception>
    double Divide(double num1, double num2);
}