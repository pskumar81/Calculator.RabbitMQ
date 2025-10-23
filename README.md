# Calculator gRPC Service

A modern calculator implementation using gRPC for service communication in .NET 9.0. This project demonstrates how to build a distributed calculator service with client-server architecture using gRPC, now with Docker support for easy deployment.

## Project Structure

- **Calculator.Server**: gRPC server implementation with calculator operations
  - Supports basic arithmetic operations (Add, Subtract, Multiply, Divide)
  - Includes input validation and error handling
  - Uses logging for operation tracking
  - Containerized with Docker

- **Calculator.Client**: Console application that connects to the gRPC server
  - Demonstrates how to make gRPC calls to the server
  - Implements dependency injection for better service management
  - Uses structured logging for operation tracking
  - Handles server responses and errors
  - Containerized with Docker
  - Configurable server connection through environment variables

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
- Uses gRPC for service communication over HTTP/2
- Implements async/await pattern
- Implements Dependency Injection (DI) principles
- Includes comprehensive logging with Microsoft.Extensions.Logging
- Follows IoC principles for better maintainability and testing
- Includes proper error handling and logging
- Certificate generation tools for secure communication
- Follows C# best practices

## Getting Started

### Running with Docker (Recommended)

1. Clone the repository:
   ```bash
   git clone https://github.com/pskumar81/CalculatorGrpc.git
   ```

2. Build and run using Docker Compose:
   ```bash
   docker-compose up --build
   ```

The services will be available at:
- Server: http://localhost:5001
- Client: Automatically connects to the server through Docker network

### Running Locally (Alternative)

1. Clone the repository:
   ```bash
   git clone https://github.com/pskumar81/CalculatorGrpc.git
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run the server:
   ```bash
   cd Calculator.Server
   dotnet run
   ```

4. Run the client (in a new terminal):
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
- Configured for HTTP/2 communication
- Includes certificate generation tools for secure communication

### Certificate Generation

The project includes tools for generating certificates for secure communication:

1. Navigate to the certs directory:
   ```bash
   cd certs
   ```

2. Run the certificate generation script:
   ```bash
   ./generate-certs.ps1
   ```

This will generate:
- CA certificate
- Server certificate
- Client certificate

These certificates can be used to enable mutual TLS (mTLS) for secure communication between the client and server.

- **Multi-stage builds** for both client and server
- **Docker Compose** for service orchestration
- **Environment Variables** for configuration
- **Network Isolation** between services
- **Optimized Images** using .NET runtime base images

### Docker Commands

Build and start the services:
```bash
docker-compose up --build
```

Stop the services:
```bash
docker-compose down
```

## Error Handling

- Division by zero throws a proper gRPC exception
- All operations are logged for monitoring and debugging
- Client handles server errors gracefully
- Docker containerization ensures consistent environment

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request
