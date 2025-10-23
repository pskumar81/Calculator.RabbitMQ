FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Calculator.Server/Calculator.Server.csproj", "Calculator.Server/"]
RUN dotnet restore "Calculator.Server/Calculator.Server.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "Calculator.Server/Calculator.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Calculator.Server/Calculator.Server.csproj" -c Release -o /app/publish

# Use runtime image for console application instead of aspnet
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy configuration files
COPY Calculator.Server/appsettings.json .
COPY Calculator.Server/appsettings.Development.json .
COPY Calculator.Server/appsettings.Docker.json .

# Set environment to use Docker configuration
ENV DOTNET_ENVIRONMENT=Docker

# No need to expose ports for RabbitMQ-based communication
# Health check can be added later if needed

ENTRYPOINT ["dotnet", "Calculator.Server.dll"]
