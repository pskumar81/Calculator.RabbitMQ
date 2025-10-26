# Contributing to Calculator RabbitMQ

Thank you for your interest in contributing to the Calculator RabbitMQ project! This guide will help you get started.

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose
- Git
- Visual Studio Code or Visual Studio (optional)

### Local Development Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/pskumar81/Calculator.RabbitMQ.git
   cd Calculator.RabbitMQ
   ```

2. **Start with Docker (Recommended):**
   ```bash
   # Windows
   start-system.bat
   
   # Linux/Mac
   ./start-system.sh
   ```

3. **Or start manually:**
   ```bash
   # Start RabbitMQ
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
   
   # Build and test
   dotnet restore
   dotnet build
   dotnet test
   
   # Run server
   cd Calculator.Server
   dotnet run
   
   # Run client (in new terminal)
   cd Calculator.Client
   dotnet run
   ```

## 📁 Project Structure

```
Calculator.RabbitMQ/
├── Calculator.Server/          # RabbitMQ message consumer/processor
├── Calculator.Client/          # RabbitMQ message publisher/client
├── Calculator.Tests/          # Unit and integration tests
├── .github/workflows/         # CI/CD automation
├── certs/                    # Certificate generation tools
├── docker-compose.yml        # Multi-service Docker setup
├── start-system.*           # Quick start scripts
└── docs/                    # Documentation
```

## 🔧 Development Workflow

### Before Making Changes

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Ensure tests pass:**
   ```bash
   dotnet test
   ```

3. **Check code formatting:**
   ```bash
   dotnet format
   ```

### Making Changes

1. **Follow C# coding standards:**
   - Use meaningful variable and method names
   - Add XML documentation for public APIs
   - Follow async/await patterns
   - Use proper error handling

2. **Add tests for new functionality:**
   - Unit tests for business logic
   - Integration tests for RabbitMQ communication
   - Performance tests for critical paths

3. **Update documentation:**
   - Update README.md if needed
   - Add XML documentation
   - Update API documentation

### Submitting Changes

1. **Commit your changes:**
   ```bash
   git add .
   git commit -m "feat: add new calculation operation"
   ```

2. **Push to your fork:**
   ```bash
   git push origin feature/your-feature-name
   ```

3. **Create a Pull Request:**
   - Provide a clear description
   - Reference any related issues
   - Ensure CI checks pass

## 🎯 Areas for Contribution

### High Priority
- [ ] Add more mathematical operations (power, square root, etc.)
- [ ] Implement request caching
- [ ] Add monitoring and metrics
- [ ] Improve error handling and retry logic

### Medium Priority
- [ ] Add batch calculation support
- [ ] Implement calculation history
- [ ] Add configuration validation
- [ ] Improve logging and tracing

### Low Priority
- [ ] Add web UI client
- [ ] Implement different calculation modes
- [ ] Add calculation result persistence
- [ ] Performance optimizations

## 🧪 Testing Guidelines

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "TestName"
```

### Test Categories

1. **Unit Tests** (`Calculator.Tests/`)
   - Test individual components
   - Mock external dependencies
   - Fast execution

2. **Integration Tests**
   - Test RabbitMQ communication
   - Test end-to-end scenarios
   - Require RabbitMQ instance

3. **Performance Tests** (`.github/workflows/performance.yml`)
   - Test throughput and latency
   - Automated benchmarking
   - Performance regression detection

### Writing Tests

```csharp
[Fact]
public async Task CalculatorService_Add_ReturnsCorrectSum()
{
    // Arrange
    var service = new CalculatorService(_logger);
    var request = new CalculationRequest 
    { 
        Operation = "Add", 
        Number1 = 2, 
        Number2 = 3 
    };

    // Act
    var response = await service.CalculateAsync(request);

    // Assert
    Assert.True(response.Success);
    Assert.Equal(5, response.Result);
}
```

## 🔍 Code Quality

### Automated Checks

Our CI/CD pipeline automatically runs:

- **Build verification** (`.github/workflows/dotnet.yml`)
- **Security scanning** (`.github/workflows/security.yml`)
- **Performance testing** (`.github/workflows/performance.yml`)
- **Documentation generation** (`.github/workflows/documentation.yml`)

### Manual Checks

Before submitting:

```bash
# Format code
dotnet format

# Run security audit
dotnet list package --vulnerable

# Build in release mode
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release
```

## 📚 Architecture Guidelines

### RabbitMQ Communication

```csharp
// Request Pattern
var request = new CalculationRequest
{
    CorrelationId = Guid.NewGuid().ToString(),
    Operation = "Add",
    Number1 = 5,
    Number2 = 3,
    ReplyTo = responseQueueName
};

// Response Pattern
var response = new CalculationResponse
{
    CorrelationId = request.CorrelationId,
    Success = true,
    Result = 8,
    ProcessingTimeMs = stopwatch.ElapsedMilliseconds
};
```

### Error Handling

```csharp
try
{
    var result = await calculatorService.CalculateAsync(request);
    return result;
}
catch (DivideByZeroException)
{
    return new CalculationResponse
    {
        Success = false,
        ErrorMessage = "Cannot divide by zero"
    };
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    return new CalculationResponse
    {
        Success = false,
        ErrorMessage = "Internal server error"
    };
}
```

### Configuration

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "RequestTimeoutMs": 30000
  }
}
```

## 🚨 Security Guidelines

- Never commit secrets or credentials
- Use environment variables for sensitive configuration
- Keep dependencies updated
- Follow secure coding practices
- Run security scans before submitting

## 📞 Getting Help

- **Issues**: Create an issue for bugs or feature requests
- **Discussions**: Use GitHub Discussions for questions
- **Pull Requests**: Submit PRs for code contributions
- **Documentation**: Check the docs/ folder

## 📄 License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

Thank you for contributing to Calculator RabbitMQ! 🎉