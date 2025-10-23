#!/bin/bash
# Docker startup script for Calculator.RabbitMQ

echo "🐰 Starting Calculator.RabbitMQ System..."
echo "========================================="

# Build and start all services
echo "📦 Building Docker images..."
docker-compose build

echo "🚀 Starting RabbitMQ and Calculator Server..."
docker-compose up -d rabbitmq calculator-server

echo "⏳ Waiting for services to be ready..."
echo "   - RabbitMQ Management UI will be available at: http://localhost:15672"
echo "   - Default credentials: guest/guest"

# Wait for RabbitMQ to be healthy
echo "🔍 Checking RabbitMQ health..."
timeout=60
counter=0
while [ $counter -lt $timeout ]; do
    if docker-compose exec rabbitmq rabbitmq-diagnostics -q ping > /dev/null 2>&1; then
        echo "✅ RabbitMQ is ready!"
        break
    fi
    echo "   Waiting for RabbitMQ... ($counter/$timeout)"
    sleep 2
    counter=$((counter + 2))
done

if [ $counter -ge $timeout ]; then
    echo "❌ RabbitMQ failed to start within $timeout seconds"
    exit 1
fi

# Wait a bit more for server to establish queues
sleep 5

echo "✅ System is ready!"
echo ""
echo "🎯 Next steps:"
echo "   1. Run the client: docker-compose run --rm calculator-client"
echo "   2. Or run client interactively: docker-compose up calculator-client"
echo "   3. Check RabbitMQ management: http://localhost:15672"
echo "   4. View logs: docker-compose logs -f calculator-server"
echo ""
echo "🛑 To stop the system: docker-compose down"