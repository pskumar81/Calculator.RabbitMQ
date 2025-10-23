@echo off
REM Docker startup script for Calculator.RabbitMQ (Windows)

echo 🐰 Starting Calculator.RabbitMQ System...
echo =========================================

REM Build and start all services
echo 📦 Building Docker images...
docker-compose build

echo 🚀 Starting RabbitMQ and Calculator Server...
docker-compose up -d rabbitmq calculator-server

echo ⏳ Waiting for services to be ready...
echo    - RabbitMQ Management UI will be available at: http://localhost:15672
echo    - Default credentials: guest/guest

REM Wait for RabbitMQ to be healthy
echo 🔍 Checking RabbitMQ health...
timeout /t 10 /nobreak > nul

echo ✅ System is ready!
echo.
echo 🎯 Next steps:
echo    1. Run the client: docker-compose run --rm calculator-client
echo    2. Or run client interactively: docker-compose up calculator-client
echo    3. Check RabbitMQ management: http://localhost:15672
echo    4. View logs: docker-compose logs -f calculator-server
echo.
echo 🛑 To stop the system: docker-compose down

pause