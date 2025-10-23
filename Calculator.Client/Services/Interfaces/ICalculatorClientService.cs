namespace Calculator.Client.Services.Interfaces;

/// <summary>
/// Interface for calculator client operations - maintains compatibility with original gRPC interface
/// </summary>
public interface ICalculatorClientService : IDisposable
{
    /// <summary>
    /// Adds two numbers asynchronously
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <returns>Sum of the numbers</returns>
    Task<double> AddAsync(double num1, double num2);

    /// <summary>
    /// Subtracts second number from first asynchronously
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <returns>Difference of the numbers</returns>
    Task<double> SubtractAsync(double num1, double num2);

    /// <summary>
    /// Multiplies two numbers asynchronously
    /// </summary>
    /// <param name="num1">First number</param>
    /// <param name="num2">Second number</param>
    /// <returns>Product of the numbers</returns>
    Task<double> MultiplyAsync(double num1, double num2);

    /// <summary>
    /// Divides first number by second asynchronously
    /// </summary>
    /// <param name="num1">First number (dividend)</param>
    /// <param name="num2">Second number (divisor)</param>
    /// <returns>Quotient of the numbers</returns>
    /// <exception cref="InvalidOperationException">Thrown when calculation fails</exception>
    Task<double> DivideAsync(double num1, double num2);

    /// <summary>
    /// Sets up the required queues and exchanges for the client
    /// </summary>
    void SetupQueuesAndExchanges();
}