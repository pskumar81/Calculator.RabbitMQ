using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // RabbitMQ services will be added here
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine("Welcome to the Calculator Client!");
Console.WriteLine("Connecting to the Calculator Server via RabbitMQ...");

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
        // RabbitMQ client implementation will be added here
        Console.WriteLine($"\nResult: {num1} [operation] {num2} = [result]");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while processing the calculation");
        Console.WriteLine($"\nError: {ex.Message}");
    }
}