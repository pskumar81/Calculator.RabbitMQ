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

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5001
ENTRYPOINT ["dotnet", "Calculator.Server.dll"]
