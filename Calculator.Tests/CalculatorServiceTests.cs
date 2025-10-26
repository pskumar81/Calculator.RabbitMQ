using Xunit;
using Calculator.Server.Services;
using Calculator.Server.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Calculator.Tests;

public class CalculatorServiceTests
{
    private readonly CalculatorService _service;
    private readonly Mock<ILogger<CalculatorService>> _loggerMock;

    public CalculatorServiceTests()
    {
        _loggerMock = new Mock<ILogger<CalculatorService>>();
        _service = new CalculatorService(_loggerMock.Object);
    }

    [Fact]
    public async Task CalculateAsync_Add_TwoPositiveNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "Add",
            Number1 = 2, 
            Number2 = 3 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(5, response.Result);
        Assert.Equal(request.CorrelationId, response.CorrelationId);
        Assert.Equal("Add", response.Operation);
    }

    [Fact]
    public async Task CalculateAsync_Add_NegativeNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "Add",
            Number1 = -2, 
            Number2 = -3 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(-5, response.Result);
    }

    [Fact]
    public async Task CalculateAsync_Subtract_ReturnsCorrectDifference()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "Subtract",
            Number1 = 5, 
            Number2 = 3 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(2, response.Result);
    }

    [Fact]
    public async Task CalculateAsync_Multiply_ReturnsCorrectProduct()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "Multiply",
            Number1 = 4, 
            Number2 = 3 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(12, response.Result);
    }

    [Fact]
    public async Task CalculateAsync_Divide_ReturnsCorrectQuotient()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "Divide",
            Number1 = 6, 
            Number2 = 2 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(3, response.Result);
    }

    [Fact]
    public async Task CalculateAsync_Divide_ByZero_ReturnsError()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "Divide",
            Number1 = 6, 
            Number2 = 0 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("Cannot divide by zero", response.ErrorMessage);
        Assert.Equal(request.CorrelationId, response.CorrelationId);
    }

    [Fact]
    public async Task CalculateAsync_InvalidOperation_ReturnsError()
    {
        // Arrange
        var request = new CalculationRequest 
        { 
            CorrelationId = Guid.NewGuid().ToString(),
            Operation = "InvalidOperation",
            Number1 = 6, 
            Number2 = 2 
        };

        // Act
        var response = await _service.CalculateAsync(request);

        // Assert
        Assert.False(response.Success);
        Assert.Contains("Unsupported operation", response.ErrorMessage);
    }

    [Fact]
    public void Add_ReturnsCorrectSum()
    {
        // Act
        var result = _service.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public void Subtract_ReturnsCorrectDifference()
    {
        // Act
        var result = _service.Subtract(5, 3);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public void Multiply_ReturnsCorrectProduct()
    {
        // Act
        var result = _service.Multiply(4, 3);

        // Assert
        Assert.Equal(12, result);
    }

    [Fact]
    public void Divide_ReturnsCorrectQuotient()
    {
        // Act
        var result = _service.Divide(6, 2);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public void Divide_ByZero_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => _service.Divide(6, 0));
    }
}
