# E-Commerce Microservices Platform

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com)

A production-ready e-commerce platform built with microservices architecture, demonstrating modern software engineering practices and distributed systems design.

**Author:** Rodrigo Nunes

## 🏗️ Architecture Overview

This project implements a complete e-commerce system using microservices architecture with the following services:

```
┌─────────────────┐
│   API Gateway   │  ← Entry point for all client requests
└────────┬────────┘
         │
    ┌────┴────┬────────────┬──────────────┐
    │         │            │              │
┌───▼───┐ ┌──▼──┐  ┌──────▼─────┐  ┌────▼─────┐
│Catalog│ │Orders│  │  Payments  │  │ Identity │
│Service│ │Service│  │  Service   │  │ Service  │
└───┬───┘ └──┬───┘  └──────┬─────┘  └────┬─────┘
    │        │             │              │
    └────────┴─────────────┴──────────────┘
                   │
            ┌──────▼───────┐
            │  RabbitMQ    │  ← Event Bus
            │ (Message Bus)│
            └──────────────┘
```

### Services

1. **Catalog Service** - Product and category management
   - Product CRUD operations
   - Category management
   - Stock management
   - Search and filtering
   - Pagination support

2. **Orders Service** - Order processing and management
   - Shopping cart functionality
   - Order creation and tracking
   - CQRS pattern implementation
   - Event-driven order processing

3. **Payments Service** - Payment processing
   - Payment gateway integration (simulated)
   - Payment status tracking
   - Event-driven payment confirmation

4. **Identity Service** - Authentication and authorization
   - JWT token generation
   - User management
   - Role-based access control

5. **API Gateway** - Request routing and aggregation
   - Route management
   - Load balancing
   - Rate limiting
   - Request/Response transformation

## 🚀 Features

- ✅ **Microservices Architecture** - Independently deployable services
- ✅ **Event-Driven Communication** - Asynchronous messaging with RabbitMQ
- ✅ **API Gateway Pattern** - Centralized entry point
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Health Checks** - Service health monitoring
- ✅ **Swagger/OpenAPI** - Comprehensive API documentation
- ✅ **Docker Support** - Containerized deployment
- ✅ **Logging** - Structured logging with Serilog
- ✅ **CORS Enabled** - Cross-origin resource sharing
- ✅ **In-Memory Database** - Easy setup for demonstration (can be replaced with SQL Server/PostgreSQL)

## 🛠️ Technology Stack

- **.NET 8.0** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for data access
- **RabbitMQ** - Message broker for event-driven architecture
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, for containerized deployment)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (optional, for event bus functionality)

## 🚀 Getting Started

### Option 1: Run with Docker Compose (Recommended)

```bash
# Clone the repository
git clone https://github.com/yourusername/ecommerce-microservices.git
cd ecommerce-microservices

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f
```

Access the services:
- **Catalog API**: http://localhost:5001
- **Orders API**: http://localhost:5002
- **Payments API**: http://localhost:5003
- **Identity API**: http://localhost:5004
- **API Gateway**: http://localhost:5000

### Option 2: Run Locally

#### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ecommerce-microservices.git
cd ecommerce-microservices
```

#### 2. Restore Dependencies

```bash
dotnet restore
```

#### 3. Build the Solution

```bash
dotnet build
```

#### 4. Run Individual Services

Open multiple terminal windows and run each service:

```bash
# Terminal 1 - Catalog Service
cd src/Services/Catalog/Catalog.API
dotnet run

# Terminal 2 - Orders Service
cd src/Services/Orders/Orders.API
dotnet run

# Terminal 3 - Payments Service
cd src/Services/Payments/Payments.API
dotnet run

# Terminal 4 - Identity Service
cd src/Services/Identity/Identity.API
dotnet run

# Terminal 5 - API Gateway
cd src/ApiGateway
dotnet run
```

## 📚 API Documentation

Each service exposes Swagger UI for interactive API documentation:

- **Catalog API**: http://localhost:5001/swagger
- **Orders API**: http://localhost:5002/swagger
- **Payments API**: http://localhost:5003/swagger
- **Identity API**: http://localhost:5004/swagger

### Sample API Endpoints

#### Catalog Service

```http
GET    /api/products              # Get all products (with pagination)
GET    /api/products/{id}         # Get product by ID
POST   /api/products              # Create new product
PUT    /api/products/{id}         # Update product
DELETE /api/products/{id}         # Delete product (soft delete)
PATCH  /api/products/{id}/stock   # Update product stock

GET    /api/categories            # Get all categories
GET    /api/categories/{id}       # Get category by ID
```

#### Example Request

```bash
# Get all products
curl -X GET "http://localhost:5001/api/products?pageNumber=1&pageSize=10" -H "accept: application/json"

# Create a new product
curl -X POST "http://localhost:5001/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Wireless Keyboard",
    "description": "Mechanical wireless keyboard with RGB lighting",
    "price": 89.99,
    "stockQuantity": 100,
    "imageUrl": "https://example.com/keyboard.jpg",
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }'
```

## 🧪 Testing

### Run Unit Tests

```bash
dotnet test
```

### Run Tests with Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

## 🐳 Docker Deployment

### Build Docker Images

```bash
# Build all images
docker-compose build

# Build specific service
docker build -t catalog-api -f src/Services/Catalog/Catalog.API/Dockerfile .
```

### Docker Compose Services

The `docker-compose.yml` includes:
- All microservices
- RabbitMQ message broker
- SQL Server database (optional)
- Redis cache (optional)

## 📊 Project Structure

```
ecommerce-microservices/
├── src/
│   ├── ApiGateway/                 # API Gateway service
│   ├── Services/
│   │   ├── Catalog/
│   │   │   └── Catalog.API/        # Catalog microservice
│   │   ├── Orders/
│   │   │   └── Orders.API/         # Orders microservice
│   │   ├── Payments/
│   │   │   └── Payments.API/       # Payments microservice
│   │   └── Identity/
│   │       └── Identity.API/       # Identity microservice
│   └── BuildingBlocks/
│       └── EventBus/               # Shared event bus library
├── tests/                          # Unit and integration tests
├── docs/                           # Additional documentation
├── docker-compose.yml              # Docker Compose configuration
├── docker-compose.override.yml     # Development overrides
├── .gitignore
├── README.md
└── LICENSE
```

## 🔧 Configuration

### Environment Variables

Each service can be configured using `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CatalogDb;Trusted_Connection=True;"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🎯 Design Patterns Used

- **Microservices Architecture** - Service decomposition
- **API Gateway Pattern** - Single entry point
- **Repository Pattern** - Data access abstraction
- **CQRS** - Command Query Responsibility Segregation
- **Event Sourcing** - Event-driven architecture
- **Circuit Breaker** - Fault tolerance (planned)
- **Retry Pattern** - Resilience (planned)

## 🔐 Security

- JWT authentication
- Role-based authorization
- HTTPS enforcement
- CORS configuration
- API rate limiting (via API Gateway)

## 📈 Monitoring & Observability

- Health check endpoints (`/health`)
- Structured logging
- Application metrics (planned)
- Distributed tracing (planned)

## 🚧 Roadmap

- [ ] Implement API Gateway with Ocelot/YARP
- [ ] Add Redis caching layer
- [ ] Implement Circuit Breaker pattern
- [ ] Add distributed tracing (OpenTelemetry)
- [ ] Implement event sourcing for Orders
- [ ] Add Kubernetes deployment manifests
- [ ] Implement API versioning
- [ ] Add integration tests
- [ ] Implement gRPC communication between services
- [ ] Add monitoring dashboard (Grafana)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Rodrigo Nunes**

- GitHub: [@rodrigonunes792](https://github.com/rodrigonunes792)
- LinkedIn: [Rodrigo Nunes](https://www.linkedin.com/in/rodrigonunes79/)

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- Microservices patterns by Chris Richardson
- .NET Microservices Architecture Guide by Microsoft

---

⭐ If you find this project useful, please consider giving it a star!
