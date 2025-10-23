using Calculator.Client.Extensions;
using Calculator.Client.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add all calculator client services including RabbitMQ
        services.AddCalculatorClientServices(context.Configuration);
    })
    .Build();

var calculatorService = host.Services.GetRequiredService<ICalculatorClientService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine("Welcome to the Calculator Client!");
Console.WriteLine("Connecting to the Calculator Server via RabbitMQ...");

try
{
    var running = true;
    while (running)
    {
        Console.WriteLine("\nSelect operation:");
        Console.WriteLine("1. Add");
        Console.WriteLine("2. Subtract");
        Console.WriteLine("3. Multiply");
        Console.WriteLine("4. Divide");
        Console.WriteLine("5. Exit");

        var operation = Console.ReadLine();
        if (operation == "5")
        {
            running = false;
            break;
        }

        if (!new[] { "1", "2", "3", "4" }.Contains(operation))
        {
            Console.WriteLine("Invalid operation. Please select a number between 1 and 5.");
            continue;
        }

        Console.WriteLine("Enter first number:");
        if (!double.TryParse(Console.ReadLine(), out double num1))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            continue;
        }

        Console.WriteLine("Enter second number:");
        if (!double.TryParse(Console.ReadLine(), out double num2))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            continue;
        }

        try
        {
            double result;
            string operationSymbol;

            switch (operation)
            {
                case "1":
                    result = await calculatorService.AddAsync(num1, num2);
                    operationSymbol = "+";
                    break;
                case "2":
                    result = await calculatorService.SubtractAsync(num1, num2);
                    operationSymbol = "-";
                    break;
                case "3":
                    result = await calculatorService.MultiplyAsync(num1, num2);
                    operationSymbol = "*";
                    break;
                case "4":
                    result = await calculatorService.DivideAsync(num1, num2);
                    operationSymbol = "/";
                    break;
                default:
                    continue;
            }

            Console.WriteLine($"\nResult: {num1} {operationSymbol} {num2} = {result}");
        }
        catch (TimeoutException ex)
        {
            logger.LogError(ex, "Request timeout occurred");
            Console.WriteLine($"\nError: Request timed out. Please check if the server is running.");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Calculation error occurred");
            Console.WriteLine($"\nError: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while processing the calculation");
            Console.WriteLine($"\nError: An unexpected error occurred. Please try again.");
        }
    }
}
finally
{
    calculatorService.Dispose();
    Console.WriteLine("\nThank you for using the Calculator Client!");
}