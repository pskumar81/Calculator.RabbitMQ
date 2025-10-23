namespace Calculator.Client.Services.Interfaces;

public interface ICalculatorClientService
{
    Task<double> AddAsync(double num1, double num2);
    Task<double> SubtractAsync(double num1, double num2);
    Task<double> MultiplyAsync(double num1, double num2);
    Task<double> DivideAsync(double num1, double num2);
}
