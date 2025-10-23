using Xunit;
using Calculator.Server.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Grpc.Core;
using Calculator.Server;

namespace Calculator.Tests;

public class CalculatorServiceTests
{
    private readonly CalculatorServiceImpl _service;
    private readonly Mock<ILogger<CalculatorServiceImpl>> _loggerMock;

    public CalculatorServiceTests()
    {
        _loggerMock = new Mock<ILogger<CalculatorServiceImpl>>();
        _service = new CalculatorServiceImpl(_loggerMock.Object);
    }

    [Fact]
    public async Task Add_TwoPositiveNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var request = new CalculateRequest { Number1 = 2, Number2 = 3 };

        // Act
        var response = await _service.Add(request, TestServerCallContext.Create());

        // Assert
        Assert.Equal(5, response.Result);
    }

    [Fact]
    public async Task Add_NegativeNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var request = new CalculateRequest { Number1 = -2, Number2 = -3 };

        // Act
        var response = await _service.Add(request, TestServerCallContext.Create());

        // Assert
        Assert.Equal(-5, response.Result);
    }

    [Fact]
    public async Task Subtract_ReturnsCorrectDifference()
    {
        // Arrange
        var request = new CalculateRequest { Number1 = 5, Number2 = 3 };

        // Act
        var response = await _service.Subtract(request, TestServerCallContext.Create());

        // Assert
        Assert.Equal(2, response.Result);
    }

    [Fact]
    public async Task Multiply_ReturnsCorrectProduct()
    {
        // Arrange
        var request = new CalculateRequest { Number1 = 4, Number2 = 3 };

        // Act
        var response = await _service.Multiply(request, TestServerCallContext.Create());

        // Assert
        Assert.Equal(12, response.Result);
    }

    [Fact]
    public async Task Divide_ReturnsCorrectQuotient()
    {
        // Arrange
        var request = new CalculateRequest { Number1 = 6, Number2 = 2 };

        // Act
        var response = await _service.Divide(request, TestServerCallContext.Create());

        // Assert
        Assert.Equal(3, response.Result);
    }

    [Fact]
    public async Task Divide_ByZero_ThrowsRpcException()
    {
        // Arrange
        var request = new CalculateRequest { Number1 = 6, Number2 = 0 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _service.Divide(request, TestServerCallContext.Create())
        );
        Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        Assert.Equal("Cannot divide by zero", exception.Status.Detail);
    }
}

/// <summary>
/// Helper class to create a test ServerCallContext
/// </summary>
public class TestServerCallContext : ServerCallContext
{
    private TestServerCallContext() { }

    public static ServerCallContext Create()
    {
        return new TestServerCallContext();
    }

    protected override string MethodCore => "";
    protected override string HostCore => "";
    protected override string PeerCore => "";
    protected override DateTime DeadlineCore => DateTime.MaxValue;
    protected override CancellationToken CancellationTokenCore => CancellationToken.None;
    protected override Metadata RequestHeadersCore => new Metadata();
    protected override Metadata ResponseTrailersCore => new Metadata();
    protected override Status StatusCore { get; set; } = Status.DefaultSuccess;
    protected override WriteOptions WriteOptionsCore { get; set; } = new WriteOptions();
    protected override AuthContext AuthContextCore => null;
    
    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options) => null;
    protected override Task WriteResponseHeadersAsyncCore(Metadata headers) => Task.CompletedTask;
}
