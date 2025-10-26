# Calculator RabbitMQ Service

A modern calculator implementation using RabbitMQ for message-based communication in .NET 9.0. This project demonstrates how to build a distributed calculator service with client-server architecture using RabbitMQ messaging, with complete Docker support for easy deployment.

## Project Structure

- **Calculator.Server**: RabbitMQ consumer that processes calculation requests
  - Supports basic arithmetic operations (Add, Subtract, Multiply, Divide)
  - Includes input validation and error handling
  - Uses comprehensive logging for operation tracking
  - Containerized with Docker

- **Calculator.Client**: Console application that sends calculation requests via RabbitMQ
  - Demonstrates how to publish messages and receive responses via RabbitMQ
  - Implements dependency injection for better service management
  - Uses structured logging for operation tracking
  - Handles server responses and errors gracefully
  - Containerized with Docker
  - Configurable RabbitMQ connection through environment variables

- **Calculator.Tests**: Unit tests for the calculator service
  - Tests all arithmetic operations
  - Includes edge cases and error scenarios
  - Uses xUnit testing framework
  
- **Docker Support**: Complete containerization
  - Multi-stage builds for optimal image size
  - Docker Compose for orchestration
  - Isolated network for service communication

## Features

- **Add Operation**: Adds two numbers
- **Subtract Operation**: Subtracts two numbers
- **Multiply Operation**: Multiplies two numbers
- **Divide Operation**: Divides two numbers (includes division by zero validation)

## Technical Details

- Built with .NET 9.0
- Uses RabbitMQ for asynchronous message-based communication
- Implements async/await pattern
- Implements Dependency Injection (DI) principles
- Includes comprehensive logging with Microsoft.Extensions.Logging
- Follows IoC principles for better maintainability and testing
- Includes proper error handling and logging
- Request/Response correlation using correlation IDs
- Follows C# best practices

## Getting Started

### Running with Docker (Recommended)

1. Clone the repository:
   ```bash
   git clone https://github.com/pskumar81/Calculator.RabbitMQ.git
   ```

2. Build and run using Docker Compose:
   ```bash
   docker-compose up --build
   ```

The services will be available at:
- RabbitMQ Management UI: http://localhost:15672 (guest/guest)
- Client: Automatically connects to the server through Docker network

### Running Locally (Alternative)

1. Clone the repository:
   ```bash
   git clone https://github.com/pskumar81/Calculator.RabbitMQ.git
   ```

2. Start RabbitMQ:
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the server:
   ```bash
   cd Calculator.Server
   dotnet run
   ```

5. Run the client (in a new terminal):
   ```bash
   cd Calculator.Client
   dotnet run
   ```

## Testing

Run the tests using:
```bash
dotnet test
```

## Docker Support

The application is fully containerized with Docker support:

- Multi-stage Docker builds for both client and server
- Docker Compose for easy deployment and service orchestration
- RabbitMQ integration with health checks
- Environment-based configuration for different deployment scenarios

### Docker Commands

Build and start the services:
```bash
docker-compose up --build
```

Stop the services:
```bash
docker-compose down
```

Access RabbitMQ Management UI:
```bash
# Open http://localhost:15672 in your browser
# Default credentials: guest/guest
```

## Error Handling

- Division by zero returns a proper error response with detailed message
- All operations are logged for monitoring and debugging
- Client handles server errors gracefully with retry logic
- RabbitMQ message acknowledgment ensures reliable processing
- Docker containerization ensures consistent environment

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request
