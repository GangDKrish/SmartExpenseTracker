# CommerceHub

> A .NET 10 microservices-based e-commerce platform demonstrating REST APIs, gRPC service-to-service communication, distributed transaction compensation, asynchronous messaging, background processing, and Entity Framework Core with SQLite.

## Overview

**CommerceHub** is a backend-focused e-commerce platform built with **C# and ASP.NET Core**.

The project is designed to demonstrate practical microservices and distributed-systems concepts rather than simply implementing CRUD APIs.

The system is composed of three independently responsible services:

* **ProductService** — Product catalog and product queries
* **InventoryService** — Stock management and inventory reservations
* **OrderService** — Order creation and orchestration

The project demonstrates an important architectural distinction:

```text
External Client
      |
      | REST
      v
+------------------+
|   OrderService   |
+------------------+
      |
      | gRPC
      v
+---------------------+
|  InventoryService   |
+---------------------+

+------------------+
| ProductService   |
+------------------+

OrderService
     |
     v
OrderCreatedEvent
     |
     v
In-Memory Channel<T>
     |
     v
BackgroundService
     |
     v
Order Audit
```

---

## Key Features

* Microservices architecture with clear service ownership
* ASP.NET Core REST APIs
* Strongly typed gRPC communication
* Entity Framework Core
* SQLite persistence
* DTO-based API and messaging contracts
* Product CRUD operations
* Complex product querying
* HTTP `QUERY` demonstration for complex read operations
* Inventory reservation and release
* Distributed transaction compensation
* In-memory producer-consumer messaging using `Channel<T>`
* Background processing using `.NET BackgroundService`
* Asynchronous order auditing
* Swagger / OpenAPI
* Dependency Injection
* Async/Await
* EF Core migrations

---

# Architecture

CommerceHub separates business responsibilities across independent services.

```text
                         ┌──────────────────┐
                         │      Client      │
                         └────────┬─────────┘
                                  │
                         REST / HTTP APIs
                                  │
              ┌───────────────────┼───────────────────┐
              │                   │                   │
              ▼                   ▼                   ▼
     ┌────────────────┐  ┌────────────────┐  ┌────────────────┐
     │ ProductService │  │InventoryService│  │ OrderService   │
     └───────┬────────┘  └───────▲────────┘  └───────┬────────┘
             │                   │                   │
             │                   │      gRPC         │
             │                   └───────────────────┘
             │
             ▼
          SQLite

OrderService
     │
     │ OrderCreatedEventDTO
     ▼
┌──────────────────────┐
│ In-Memory Channel<T> │
└──────────┬───────────┘
           │
           ▼
┌─────────────────────────────┐
│ OrderMessageBackgroundService│
└────────────┬────────────────┘
             │
             ▼
       OrderAudit
             │
             ▼
          SQLite
```

### Service Responsibilities

| Service                 | Responsibility                      | Communication       |
| ----------------------- | ----------------------------------- | ------------------- |
| ProductService          | Product catalog and product queries | REST + gRPC         |
| InventoryService        | Inventory and reservations          | REST + gRPC         |
| OrderService            | Order orchestration                 | REST + gRPC         |
| Order background worker | Asynchronous audit processing       | In-memory messaging |

Each service owns its own data access and business responsibility rather than allowing another service to directly manipulate its database.

---

# Technology Stack

| Technology            | Purpose                                   |
| --------------------- | ----------------------------------------- |
| C#                    | Primary programming language              |
| .NET 10               | Application runtime                       |
| ASP.NET Core          | REST APIs and service hosting             |
| Entity Framework Core | ORM / persistence                         |
| SQLite                | Local database                            |
| gRPC                  | Internal service-to-service communication |
| Protocol Buffers      | gRPC contracts                            |
| `Channel<T>`          | In-memory asynchronous messaging          |
| `BackgroundService`   | Background event processing               |
| Swagger / OpenAPI     | API exploration and testing               |
| Dependency Injection  | Service composition                       |
| Async/Await           | Asynchronous I/O                          |

---

# 1. ProductService

`ProductService` owns the product catalog.

### Responsibilities

* Create products
* Retrieve products
* Update products
* Delete products
* Search products
* Filter products by category and price
* Perform complex product queries

### REST APIs

```text
GET     /api/products
GET     /api/products/{id}
POST    /api/products
PUT     /api/products/{id}
DELETE  /api/products/{id}
GET     /api/products/search
```

Products are persisted using:

```text
ASP.NET Core
     |
Entity Framework Core
     |
SQLite
```

---

# 2. DTO-Based Contracts

The services do not expose persistence entities directly through their APIs.

DTOs are used to keep API contracts separate from database models.

```text
Database Entity
      |
      v
    Mapping
      |
      v
     DTO
      |
      v
 API Response
```

Examples include:

```text
ProductResponseDTO
ProductQueryRequestDTO

CreateOrderRequestDTO
CreateOrderItemRequestDTO
OrderResponseDTO

CreateInventoryRequestDTO
InventoryResponseDTO

OrderCreatedEventDTO
```

This allows persistence models and external contracts to evolve independently.

---

# 3. Complex Product Queries

ProductService demonstrates complex product filtering using a query request containing multiple criteria.

Example:

```json
{
  "category": "Accessories",
  "minPrice": 1000,
  "maxPrice": 5000,
  "searchTerm": "Keyboard",
  "sortBy": "price",
  "sortDescending": true,
  "page": 1,
  "pageSize": 10
}
```

The project also demonstrates the HTTP `QUERY` method for sending complex read criteria in the request body.

The motivation is to avoid unnecessarily large or complicated query strings when a read operation contains many filtering and sorting parameters.

---

# 4. InventoryService

`InventoryService` owns inventory and stock management.

### Responsibilities

* Retrieve inventory
* Create inventory
* Reserve inventory
* Release reserved inventory

Example inventory state:

```text
AvailableQuantity
ReservedQuantity
```

When inventory is reserved:

```text
AvailableQuantity -= requested quantity
ReservedQuantity  += requested quantity
```

When a reservation is released:

```text
ReservedQuantity  -= released quantity
AvailableQuantity += released quantity
```

The important architectural principle is that **OrderService does not directly modify inventory data**.

Inventory remains owned by `InventoryService`.

---

# 5. gRPC Communication

CommerceHub uses **REST for client-facing APIs** and **gRPC for internal service-to-service communication**.

```text
Client
  |
  | REST
  v
OrderService
  |
  | gRPC
  v
InventoryService
```

The gRPC contract is defined using Protocol Buffers.

Inventory exposes operations such as:

```text
CheckAvailability
ReserveInventory
ReleaseInventory
```

### Why gRPC?

gRPC provides:

* Strongly typed contracts
* Efficient service-to-service communication
* Contract-driven APIs
* Generated client/server implementations
* A good fit for backend-to-backend communication

The OrderService uses a strongly typed generated gRPC client rather than calling InventoryService's REST endpoint.

---

# 6. Order Creation Flow

Order creation is where several distributed-system concepts come together.

```text
Client
  |
  v
OrderService
  |
  v
Validate Order
  |
  v
Get Product Information
  |
  v
Reserve Inventory
  |
  | gRPC
  v
InventoryService
  |
  +----------------------+
  |                      |
Success                Failure
  |                      |
  v                      v
Continue              Reject Order
  |
  v
Persist Order
  |
  v
Create OrderCreatedEventDTO
  |
  v
Enqueue Event
  |
  v
Return Response
```

The order workflow does not require another service to directly access the OrderService database.

---

# 7. Distributed Transaction Compensation

An order can contain multiple items, meaning multiple inventory reservations may be required.

Consider:

```text
Item A → Reservation succeeds
Item B → Reservation succeeds
Item C → Reservation fails
```

Simply rejecting the order would leave the reservations for A and B active.

CommerceHub therefore implements a **compensation mechanism**:

```text
Item A → Reserved
Item B → Reserved
Item C → Failed
             |
             v
       Compensation
         /       \
        v         v
 Release A    Release B
        \         /
         v       v
        Order Rejected
```

This demonstrates an important distributed-systems principle:

> A distributed workflow cannot rely on a single traditional database transaction across multiple independent services.

Instead, previously completed operations can require **compensating actions** when a later operation fails.

---

# 8. Asynchronous Messaging

After an order is successfully created, CommerceHub publishes an `OrderCreatedEventDTO` to an in-memory queue.

```text
OrderService
     |
     v
OrderCreatedEventDTO
     |
     v
Channel<T>
     |
     v
BackgroundService
     |
     v
Order Audit
```

The queue uses .NET's:

```csharp
System.Threading.Channels.Channel<T>
```

This implements a producer-consumer pattern.

### Producer

The order workflow publishes the event:

```text
Order Created
     |
     v
Enqueue Event
```

### Consumer

A hosted background service consumes the event:

```text
Channel<T>
    |
    v
OrderMessageBackgroundService
    |
    v
Create OrderAudit
```

The main order request does not need to wait for the audit operation to complete.

This demonstrates how secondary/non-critical work can be decoupled from the main request path.

---

# 9. Background Processing

`OrderMessageBackgroundService` inherits from:

```csharp
BackgroundService
```

Its responsibility is to continuously consume queued order events.

```text
OrderCreatedEventDTO
        |
        v
   Channel<T>
        |
        v
BackgroundService
        |
        v
   OrderAudit
        |
        v
      SQLite
```

This provides a simple demonstration of asynchronous background processing without introducing an external message broker.

---

# 10. Why In-Memory Messaging Instead of RabbitMQ?

The project intentionally uses an in-memory `Channel<T>` instead of RabbitMQ.

For a single-process demonstration, an in-memory queue is sufficient to demonstrate:

* Producer-consumer messaging
* Event publishing
* Asynchronous processing
* Background workers
* Decoupling

However, it has important limitations.

### Limitations

* Messages are not durable
* Messages can be lost if the process terminates
* It is limited to the application's process
* It is not suitable as a production distributed message broker

For a production deployment, the queue could be replaced with a durable messaging platform such as:

```text
RabbitMQ
Azure Service Bus
Kafka
```

The important architectural concept remains the same:

```text
Producer
   |
   v
Message Broker
   |
   v
Consumer
```

---

# 11. Order Auditing

The asynchronous messaging mechanism has a real business purpose: **order auditing**.

When an order is successfully created:

```text
Order Created
     |
     v
OrderCreatedEventDTO
     |
     v
Channel<T>
     |
     v
Background Worker
     |
     v
OrderAudit
     |
     v
SQLite
```

An audit record contains information such as:

```text
OrderId
EventType
CreatedAt
```

Example:

```text
OrderId        EventType       CreatedAt
------------------------------------------------
12345          OrderCreated    2026-08-24...
```

The audit operation is intentionally separated from the main order transaction.

---

# 12. Complete End-to-End Flow

The complete order workflow can be summarized as:

```text
                         CLIENT
                           |
                           | REST
                           v
                    ┌──────────────┐
                    │ OrderService │
                    └──────┬───────┘
                           |
                           | gRPC
                           v
                  ┌──────────────────┐
                  │ InventoryService │
                  └────────┬─────────┘
                           |
                  Reserve Inventory
                           |
                +----------+----------+
                |                     |
             Success                Failure
                |                     |
                v                     v
          Create Order           Compensation
                |                     |
                v                     v
          Save Order            Release Previous
                |                 Reservations
                v
      OrderCreatedEventDTO
                |
                v
         Channel<OrderEvent>
                |
                v
       BackgroundService
                |
                v
          OrderAudit
                |
                v
              SQLite
```

---

# 13. Database Design

Each service uses SQLite for local persistence.

### ProductService

```text
Products
```

### InventoryService

```text
InventoryItems
```

### OrderService

```text
Orders
OrderItems
OrderAudits
```

Entity Framework Core is responsible for:

* Database access
* Entity mapping
* Migrations
* Persistence

The project includes EF Core migrations for the service databases.

---

# 14. Project Structure

```text
CommerceHub/
│
├── CommerceHub.ProductService/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Migrations/
│   ├── Protos/
│   ├── Services/
│   └── Program.cs
│
├── CommerceHub.InventoryService/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Migrations/
│   ├── Protos/
│   ├── GrpcServices/
│   └── Program.cs
│
├── CommerceHub.OrderService/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Events/
│   ├── Messaging/
│   ├── Models/
│   ├── Migrations/
│   ├── Services/
│   └── Program.cs
│
├── CommerceHub.slnx
├── CommerceHub.slnLaunch
├── Notes.txt
└── ProjectSummary.txt
```

The solution contains all three services and includes a multi-project launch configuration for starting them together.

---

# 15. Local Development

## Prerequisites

* Visual Studio 2026 or compatible .NET IDE
* .NET 10 SDK
* HTTPS development certificate configured
* Git

No external database server or message broker is required.

The application uses SQLite and the in-memory `Channel<T>` messaging implementation.

---

## Running the Application

Open:

```text
CommerceHub.slnx
```

The repository includes a launch configuration for starting:

```text
CommerceHub.ProductService
CommerceHub.InventoryService
CommerceHub.OrderService
```

The services can also be started individually from Visual Studio.

### Service endpoints

| Service          | HTTP                    | HTTPS / gRPC             |
| ---------------- | ----------------------- | ------------------------ |
| ProductService   | `http://localhost:5139` | `https://localhost:7178` |
| InventoryService | `http://localhost:5232` | `https://localhost:7179` |
| OrderService     | `http://localhost:5104` | `https://localhost:7296` |

The HTTPS endpoints for ProductService and InventoryService are used by OrderService for gRPC communication.

Swagger/OpenAPI is available when running the services in Development mode.

---

# 16. Suggested Test Flow

A complete demonstration can be performed in the following order:

### Step 1 — Create a Product

```text
POST /api/products
```

Create a product and capture its `ProductId`.

### Step 2 — Create Inventory

```text
POST /api/inventory
```

Associate inventory with the product.

### Step 3 — Verify Inventory

```text
GET /api/inventory/{productId}
```

Verify the available and reserved quantities.

### Step 4 — Create an Order

```text
POST /api/orders
```

The OrderService will:

1. Validate the request
2. Retrieve required product information
3. Reserve inventory using gRPC
4. Create the order
5. Persist the order
6. Publish `OrderCreatedEventDTO`
7. Return the order response

### Step 5 — Verify Inventory Reservation

```text
GET /api/inventory/{productId}
```

The available quantity should be reduced and the reserved quantity increased.

### Step 6 — Verify Order

```text
GET /api/orders/{orderId}
```

### Step 7 — Verify Asynchronous Audit

The background worker consumes the event and creates an `OrderAudit` record.

### Step 8 — Test Compensation

Create an order containing multiple items where a later inventory reservation fails.

Expected behavior:

```text
Earlier reservations
        |
        v
Compensation
        |
        v
Reservations released
        |
        v
Order rejected
```

---

# 17. Design Decisions

## Why REST?

REST is used for client-facing APIs because it is simple, widely supported, and well suited to resource-oriented operations.

```text
Client → REST → Service
```

## Why gRPC?

gRPC is used for internal service-to-service calls where strongly typed contracts and efficient communication are useful.

```text
Service → gRPC → Service
```

## Why DTOs?

DTOs prevent API contracts from becoming tightly coupled to persistence entities.

```text
API Contract ≠ Database Model
```

## Why Compensation?

Multiple inventory reservations span service boundaries. A single local database transaction cannot roll back changes made by another service.

Compensation provides a way to undo previously completed business operations when a later operation fails.

## Why Asynchronous Processing?

Order auditing is secondary work and does not need to block the primary order creation response.

Therefore:

```text
Critical path
    ↓
Create Order
    ↓
Return Response

Secondary work
    ↓
Audit Event
    ↓
Background Processing
```

## Why `Channel<T>`?

It provides a lightweight producer-consumer mechanism without requiring external infrastructure.

For this demonstration, it keeps the project easy to run locally while still showing the architectural messaging pattern.

---

# 18. Production Considerations

This project intentionally keeps infrastructure lightweight for local development and learning.

A production implementation could evolve toward:

```text
Current                         Production Option
---------------------------------------------------------
SQLite                    →     PostgreSQL / SQL Server
Channel<T>                →     RabbitMQ / Azure Service Bus
Simple compensation      →     Saga / workflow orchestration
Local services            →     Containerized services
Basic APIs               →     Authentication + Authorization
Local configuration      →     Centralized configuration
Development logging      →     Structured logging + tracing
```

The goal is not to simulate a complete production platform, but to demonstrate the architectural concepts that would form the foundation of one.

---

# 19. What This Project Demonstrates

### Microservices

Independent services with clear business ownership.

### REST

Client-facing resource APIs.

### gRPC

Strongly typed internal service-to-service communication.

### Distributed Transactions

Understanding the limitations of traditional database transactions across service boundaries.

### Compensation

Reversing previously completed operations when a distributed workflow fails.

### Asynchronous Messaging

Decoupling secondary processing from the primary request.

### Background Processing

Using `.NET BackgroundService` to consume asynchronous work.

### Producer-Consumer Pattern

Using `Channel<T>` to connect producers and consumers.

### Entity Framework Core

ORM-based database access and migrations.

### DTO Contracts

Separating API/message contracts from persistence entities.

### Dependency Injection

Composing services through ASP.NET Core's built-in DI container.

### Async Programming

Using `async`/`await` for database and service communication.

---

# 20. Interview Talking Points

CommerceHub was designed to demonstrate practical understanding of backend and distributed-system concepts.

A typical architectural explanation would be:

> "CommerceHub is a .NET 10 e-commerce microservices application consisting of Product, Inventory, and Order services. Client-facing communication uses REST, while OrderService communicates with InventoryService using strongly typed gRPC. Since an order can require multiple inventory reservations, the system implements compensation to release previously reserved inventory if a later reservation fails. After successful order creation, an OrderCreated event is placed onto an in-memory Channel<T>, which is consumed by a BackgroundService to create an audit record asynchronously. SQLite and EF Core are used for lightweight local persistence, and DTOs keep API contracts separate from database entities."

### Concepts I can discuss from this project

* Why use microservices?
* Why REST vs gRPC?
* Why should services own their data?
* How do you handle distributed transactions?
* What is compensation?
* Why isn't a database transaction enough?
* Why use asynchronous messaging?
* Why use `Channel<T>`?
* What are the limitations of in-memory messaging?
* How would you replace `Channel<T>` with RabbitMQ?
* What happens if the background worker fails?
* What happens if inventory reservation succeeds but order creation fails?
* How would you make the workflow more reliable?
* How would you implement retries?
* How would you add idempotency?
* How would you monitor distributed requests?
* How would you scale the services independently?

---

# License

This project is intended for learning, experimentation, portfolio demonstration, and interview preparation.
