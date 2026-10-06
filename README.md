# SmartExpenseTracker

A full-stack expense management application built with **.NET 10, ASP.NET Core Web API, Blazor Server, CQRS with MediatR, Entity Framework Core, SQLite, JWT authentication, and Chart.js**.

SmartExpenseTracker provides authenticated users with a responsive interface for managing personal expenses, viewing spending statistics, and visualizing expenses by category.

---

## Overview

SmartExpenseTracker consists of two applications:

* **SmartExpenseTracker** — ASP.NET Core Web API backend
* **ExpenseTrackerUI** — Blazor Server frontend

The Blazor frontend communicates with the backend through HTTP REST APIs.

```text
                    ┌──────────────────────────┐
                    │      ExpenseTrackerUI    │
                    │      Blazor Server       │
                    │                          │
                    │  • Authentication        │
                    │  • Expense Dashboard     │
                    │  • Add / Edit / Delete    │
                    │  • Statistics            │
                    │  • Charts                │
                    │  • Dark Mode             │
                    └────────────┬─────────────┘
                                 │
                                 │ HTTP / REST
                                 ▼
                    ┌──────────────────────────┐
                    │   SmartExpenseTracker    │
                    │   ASP.NET Core Web API   │
                    │                          │
                    │  Controllers             │
                    │       ↓                  │
                    │  MediatR / CQRS          │
                    │       ↓                  │
                    │  EF Core                 │
                    └────────────┬─────────────┘
                                 │
                                 ▼
                    ┌──────────────────────────┐
                    │          SQLite          │
                    │                          │
                    │  Users                   │
                    │  Expenses                │
                    └──────────────────────────┘
```

---

## Features

### Expense Management

* Create, view, update, and delete expenses
* Expense categories
* Expense date tracking
* User-specific expense data
* Expense statistics
* Category-based spending visualization

### Authentication & Authorization

* User registration and login
* JWT-based API authentication
* Cookie-based authentication session in the Blazor UI
* Claims-based user identification
* User-scoped expense access
* Protected API endpoints using `[Authorize]`

### User Interface

* Blazor Server with Interactive Server rendering
* Responsive desktop and mobile layout
* Reusable Razor components
* Dashboard statistics
* Chart.js data visualization
* Dark mode with persisted theme
* Toast notifications
* Loading skeletons
* Delete confirmation modal

### Backend

* ASP.NET Core Web API
* CQRS using MediatR
* Separate command and query handlers
* Entity Framework Core
* SQLite persistence
* EF Core migrations
* DTO-based API contracts
* AutoMapper
* RESTful API endpoints
* Swagger / OpenAPI

---

## Architecture

The application follows a layered architecture with responsibilities separated between the UI, API, CQRS handlers, and persistence layer.

```text
┌───────────────────────────────────────────────┐
│               ExpenseTrackerUI                │
│                 Blazor Server                │
│                                               │
│  Pages / Components                           │
│        │                                      │
│        ▼                                      │
│  IExpenseService / ExpenseService             │
└──────────────────────┬────────────────────────┘
                       │
                       │ HTTP REST
                       ▼
┌───────────────────────────────────────────────┐
│             SmartExpenseTracker               │
│             ASP.NET Core Web API              │
│                                               │
│  Controllers                                  │
│       │                                       │
│       ▼                                       │
│  IMediator                                    │
│       │                                       │
│       ├───────────────┐                       │
│       ▼               ▼                       │
│   Commands          Queries                   │
│       │               │                       │
│       ▼               ▼                       │
│   Handlers          Handlers                  │
│       │               │                       │
│       └───────┬───────┘                       │
│               ▼                               │
│          AppDbContext                         │
└───────────────┬───────────────────────────────┘
                │
                ▼
             SQLite
```

### Request Processing

A typical write operation follows:

```text
Blazor Component
       │
       ▼
ExpenseService
       │
       ▼
HttpClient
       │
       ▼
REST API
       │
       ▼
Controller
       │
       ▼
MediatR
       │
       ▼
Command Handler
       │
       ▼
AppDbContext
       │
       ▼
SQLite
```

A read operation follows the same path, with a query and query handler responsible for retrieving the required data.

---

## CQRS

The backend uses **CQRS (Command Query Responsibility Segregation)** through MediatR.

Write operations are represented as commands:

```text
AddExpenseCommand
UpdateExpenseCommand
DeleteExpenseCommand
```

Read operations are represented as queries:

```text
GetExpensesQuery
```

Each request is handled by a dedicated handler.

```text
Controller
    │
    ▼
IMediator
    │
    ├── AddExpenseCommand
    │       ↓
    │   AddExpenseHandler
    │
    ├── UpdateExpenseCommand
    │       ↓
    │   UpdateExpenseHandler
    │
    ├── DeleteExpenseCommand
    │       ↓
    │   DeleteExpenseHandler
    │
    └── GetExpensesQuery
            ↓
        GetExpensesHandler
```

This keeps individual operations isolated and allows new commands and queries to be added without increasing controller complexity.

---

## Data Access

The project uses **Entity Framework Core** with `AppDbContext` for database access.

A generic repository layer is intentionally not introduced on top of EF Core.

```text
Controller
     │
     ▼
MediatR
     │
     ▼
CQRS Handler
     │
     ▼
AppDbContext
     │
     ▼
Entity Framework Core
     │
     ▼
SQLite
```

EF Core is responsible for:

* Database access
* Entity mapping
* LINQ queries
* Change tracking
* Migrations
* Persistence

---

## Authentication & Authorization

The API uses **JWT Bearer authentication**.

Authentication endpoints:

```text
POST /api/auth/register
POST /api/auth/login
```

After successful authentication, the API returns a JWT containing the authenticated user's claims.

Protected endpoints use:

```csharp
[Authorize]
```

The authenticated user's identity is retrieved from the claims and used to scope expense operations.

```text
User Login
    │
    ▼
JWT Token
    │
    ▼
Authorization Header
    │
    ▼
[Authorize]
    │
    ▼
User Claims
    │
    ▼
User-scoped Expense Data
```

---

## User Data Isolation

Expenses are associated with the authenticated user.

```text
                 Expenses
                    │
        ┌───────────┴───────────┐
        │                       │
      User A                  User B
        │                       │
   Expense 1               Expense 3
   Expense 2               Expense 4
```

Expense retrieval, update, and deletion operations verify the current user's identity before accessing the associated records.

This ensures that users cannot access another user's expenses through the API.

---

## API Endpoints

### Authentication

| Method | Endpoint             | Description         |
| ------ | -------------------- | ------------------- |
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login`    | Authenticate a user |

### Expenses

| Method   | Endpoint            | Description                             |
| -------- | ------------------- | --------------------------------------- |
| `GET`    | `/api/expense`      | Get expenses for the authenticated user |
| `GET`    | `/api/expense/{id}` | Get a specific expense                  |
| `POST`   | `/api/expense`      | Create an expense                       |
| `PUT`    | `/api/expense/{id}` | Update an expense                       |
| `DELETE` | `/api/expense/{id}` | Delete an expense                       |

---

## Data Model

### User

```text
User
├── Id
├── Email
├── Name
└── PasswordHash
```

### Expense

```text
Expense
├── Id
├── Title
├── Amount
├── Category
├── Date
└── UserId
```

Database indexes are used for commonly accessed fields such as user lookup, while email uniqueness is enforced at the database level.

```text
Users
  │
  └── Email → Unique Index

Expenses
  │
  └── UserId → Index
```

---

## Frontend

The frontend is implemented using **Blazor Server** with Interactive Server rendering.

### Main Components

| Component                  | Responsibility           |
| -------------------------- | ------------------------ |
| `Login.razor`              | Login and registration   |
| `Expenses.razor`           | Main expense dashboard   |
| `ExpenseChart.razor`       | Expense visualization    |
| `StatCard.razor`           | Reusable statistics card |
| `DeleteConfirmModal.razor` | Delete confirmation      |
| `LoadingSkeleton.razor`    | Loading state            |
| `MainLayout.razor`         | Application layout       |

---

## Expense Dashboard

The dashboard provides:

* Total spending
* Average expense
* Highest expense
* Number of expenses
* Category-based spending visualization
* Expense listing
* Add expense
* Edit expense
* Delete expense

```text
┌─────────────────────────────────────────────┐
│              Expense Dashboard              │
├───────────┬───────────┬───────────┬─────────┤
│   Total   │  Average  │  Highest  │  Count  │
├───────────┴───────────┴───────────┴─────────┤
│                                             │
│          Spending by Category               │
│               Chart.js                      │
│                                             │
├─────────────────────────────────────────────┤
│ Expense List                                │
│                                             │
│ Food       ₹500        Edit    Delete       │
│ Travel     ₹1200       Edit    Delete       │
│ Bills      ₹2000       Edit    Delete       │
└─────────────────────────────────────────────┘
```

---

## Chart.js Integration

Expense data is visualized using **Chart.js**.

Blazor communicates with JavaScript through JavaScript Interop.

```text
Blazor Component
       │
       ▼
IJSRuntime
       │
       ▼
JavaScript
       │
       ▼
Chart.js
       │
       ▼
Expense Visualization
```

Expenses are grouped by category and aggregated before being passed to the chart.

---

## Reusable Components

The UI separates common functionality into reusable Razor components.

```text
Expenses.razor
      │
      ├── StatCard
      ├── ExpenseChart
      ├── LoadingSkeleton
      └── DeleteConfirmModal
```

This keeps individual pages focused while allowing common UI behavior to be reused throughout the application.

---

## API Communication

The frontend communicates with the backend through an application service layer.

```text
Blazor Components
        │
        ▼
IExpenseService
        │
        ▼
ExpenseService
        │
        ▼
HttpClient
        │
        ▼
SmartExpenseTracker API
```

The service layer keeps HTTP communication separate from Razor components.

---

## UI Features

### Dark Mode

Supports light and dark themes with persisted theme selection.

### Toast Notifications

Provides feedback for successful and failed operations.

### Loading States

Loading skeletons provide visual feedback while expense data is being retrieved.

### Delete Confirmation

Expense deletion requires confirmation through a reusable modal component.

### Responsive Layout

The interface adapts to desktop and mobile screen sizes.

### Data Visualization

Category-based spending is presented using Chart.js.

---

## Technology Stack

| Technology               | Purpose                         |
| ------------------------ | ------------------------------- |
| C#                       | Primary programming language    |
| .NET 10                  | Application runtime             |
| ASP.NET Core             | REST API                        |
| Blazor Server            | Frontend UI                     |
| Entity Framework Core 10 | ORM / data access               |
| SQLite                   | Local database                  |
| MediatR                  | CQRS implementation             |
| AutoMapper               | Object mapping                  |
| JWT Bearer               | API authentication              |
| Cookie Authentication    | Blazor UI session               |
| Swagger / OpenAPI        | API documentation               |
| Bootstrap 5              | Responsive UI                   |
| Chart.js                 | Data visualization              |
| JavaScript Interop       | Blazor ↔ JavaScript integration |

---

## Project Structure

```text
SmartExpenseTracker/
│
├── SmartExpenseTracker/
│   │
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ExpenseController.cs
│   │   └── WeatherForecastController.cs
│   │
│   ├── CQRS/
│   │   ├── Commands/
│   │   │   ├── AddExpenseCommand.cs
│   │   │   ├── AddExpenseHandler.cs
│   │   │   ├── UpdateExpenseCommand.cs
│   │   │   ├── UpdateExpenseHandler.cs
│   │   │   ├── DeleteExpenseCommand.cs
│   │   │   └── DeleteExpenseHandler.cs
│   │   │
│   │   └── Queries/
│   │       ├── GetExpensesQuery.cs
│   │       └── GetExpensesHandler.cs
│   │
│   ├── Data/
│   │   └── AppDbContext.cs
│   │
│   ├── DTOs/
│   │
│   ├── Models/
│   │   ├── Expense.cs
│   │   └── User.cs
│   │
│   ├── Migrations/
│   │
│   ├── Mapping/
│   │
│   ├── Program.cs
│   └── SmartExpenseTracker.csproj
│
├── ExpenseTrackerUI/
│   │
│   ├── Components/
│   │   ├── Layout/
│   │   ├── ExpenseChart.razor
│   │   ├── StatCard.razor
│   │   ├── DeleteConfirmModal.razor
│   │   └── LoadingSkeleton.razor
│   │
│   ├── Pages/
│   │   ├── Login.razor
│   │   └── Expenses.razor
│   │
│   ├── Services/
│   │   ├── IExpenseService.cs
│   │   └── ExpenseService.cs
│   │
│   ├── Models/
│   ├── wwwroot/
│   ├── Program.cs
│   └── ExpenseTrackerUI.csproj
│
└── SmartExpenseTracker.slnx
```

---

## Database

The application uses SQLite for local persistence.

```text
                    SQLite
                       │
            ┌──────────┴──────────┐
            │                     │
          Users                 Expenses
            │                     │
          Id                    Id
          Email                 Title
          Name                  Amount
          PasswordHash          Category
                                Date
                                UserId
```

Entity Framework Core handles database access, entity mapping, querying, migrations, and persistence.

---

## Getting Started

### Prerequisites

* Visual Studio 2026 or another compatible .NET IDE
* .NET 10 SDK
* Git
* HTTPS development certificate

No external database server is required because the application uses SQLite.

### Clone the Repository

Clone the repository and open:

```text
SmartExpenseTracker.slnx
```

The solution contains:

```text
SmartExpenseTracker
ExpenseTrackerUI
```

### Run the Backend

Start:

```text
SmartExpenseTracker
```

Development URL:

```text
https://localhost:7162
```

Swagger:

```text
https://localhost:7162/swagger
```

### Run the Frontend

Start:

```text
ExpenseTrackerUI
```

Development URL:

```text
https://localhost:7184
```

The frontend is configured to communicate with:

```text
https://localhost:7162/
```

---

## Application Flow

### Authentication

```text
Register / Login
       │
       ▼
ASP.NET Core API
       │
       ▼
JWT Token
       │
       ▼
Authenticated Blazor Session
```

### Create Expense

```text
Blazor UI
    │
    ▼
ExpenseService
    │
    ▼
POST /api/expense
    │
    ▼
ExpenseController
    │
    ▼
AddExpenseCommand
    │
    ▼
AddExpenseHandler
    │
    ▼
AppDbContext
    │
    ▼
SQLite
```

### Retrieve Expenses

```text
Blazor UI
    │
    ▼
GET /api/expense
    │
    ▼
ExpenseController
    │
    ▼
GetExpensesQuery
    │
    ▼
GetExpensesHandler
    │
    ▼
AppDbContext
    │
    ▼
SQLite
    │
    ▼
User-scoped Expenses
    │
    ▼
Blazor Dashboard
```

---

## Security Considerations

This project is intended for local development and demonstration rather than production authentication.

The current implementation uses **SHA-256** for password hashing. Production applications should use a password-specific password-hashing algorithm such as:

* Argon2
* BCrypt
* PBKDF2

A production implementation should also consider:

* Secure secret/key management
* Refresh tokens
* Token rotation
* Strong password policies
* Rate limiting
* Account lockout
* HTTPS enforcement
* More granular authorization policies
* Centralized security monitoring

---

## Production Considerations

The project uses lightweight components suitable for local development.

Possible production evolution includes:

| Current                  | Production Evolution            |
| ------------------------ | ------------------------------- |
| SQLite                   | PostgreSQL / SQL Server         |
| SHA-256 password hashing | Argon2 / BCrypt / PBKDF2        |
| Local JWT configuration  | Secure key management           |
| Basic authentication     | Refresh tokens + token rotation |
| Basic logging            | Structured logging              |
| Local deployment         | Containerized deployment        |
| Basic API                | Rate limiting + monitoring      |
| Local SQLite persistence | Managed database infrastructure |

---

## Future Improvements

Potential enhancements include:

* Refresh token authentication
* Role-based authorization
* Category management
* Monthly expense reports
* Budget management
* Recurring expenses
* CSV/PDF export
* Advanced filtering and pagination
* PostgreSQL support
* Unit and integration tests
* Centralized exception handling
* FluentValidation
* Structured logging
* Docker deployment
* CI/CD pipeline
* Cloud deployment

---

## License

This project is intended for learning, experimentation, and exploring modern .NET application development.
