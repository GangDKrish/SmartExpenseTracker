# SmartExpenseTracker

> A full-stack expense management application built with **.NET 10, ASP.NET Core Web API, Blazor Server, CQRS with MediatR, Entity Framework Core, SQLite, JWT authentication, and Chart.js**.

SmartExpenseTracker is designed to demonstrate modern .NET application development across both backend and frontend layers, with a focus on **separation of concerns, CQRS, authentication, user-scoped data, reusable Blazor components, and a responsive user experience**.

---

## Overview

SmartExpenseTracker consists of two applications:

* **SmartExpenseTracker** — ASP.NET Core Web API backend
* **ExpenseTrackerUI** — Blazor Server frontend

The frontend communicates with the backend through HTTP REST APIs.

```text
                    ┌──────────────────────────┐
                    │      ExpenseTrackerUI    │
                    │      Blazor Server       │
                    │                          │
                    │  • Login / Register      │
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

# Key Features

* Full-stack application using C# across backend and frontend
* ASP.NET Core Web API
* Blazor Server with Interactive Server rendering
* CQRS using MediatR
* Separate Command and Query handlers
* Entity Framework Core
* SQLite database
* EF Core migrations
* JWT-based API authentication
* Cookie-based authentication session in the Blazor UI
* Claims-based user identification
* User-scoped expense data
* DTO-based API contracts
* AutoMapper
* RESTful CRUD APIs
* Bootstrap 5 responsive UI
* Chart.js data visualization
* JavaScript interop from Blazor
* Dark mode with persisted theme
* Toast notifications
* Loading skeletons
* Confirmation modal for deletion
* Responsive desktop/mobile UI

---

# Architecture

The application follows a layered approach where responsibilities are separated between the UI, API controllers, CQRS handlers, and persistence layer.

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

---

# Technology Stack

| Technology               | Purpose                         |
| ------------------------ | ------------------------------- |
| C#                       | Primary programming language    |
| .NET 10                  | Application runtime             |
| ASP.NET Core             | REST API                        |
| Blazor Server            | Frontend UI                     |
| Entity Framework Core 10 | Data access / ORM               |
| SQLite                   | Database                        |
| MediatR                  | CQRS implementation             |
| AutoMapper               | Object mapping                  |
| JWT Bearer               | API authentication              |
| Cookie Authentication    | Blazor UI session               |
| Swagger / OpenAPI        | API documentation and testing   |
| Bootstrap 5              | Responsive UI                   |
| Chart.js                 | Expense visualization           |
| JavaScript Interop       | Blazor ↔ JavaScript integration |

---

# Backend — SmartExpenseTracker

The backend is an ASP.NET Core Web API targeting .NET 10.

Its main responsibilities are:

* Authentication
* Expense management
* User authorization
* User-scoped data access
* CQRS processing
* Database persistence

---

# CQRS Architecture

The application uses **CQRS — Command Query Responsibility Segregation** through MediatR.

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

Each operation has its own handler.

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

This keeps individual business operations isolated and gives each handler a focused responsibility.

---

# Why CQRS?

Instead of putting all database operations inside a controller:

```text
Controller
    ├── Add
    ├── Update
    ├── Delete
    └── Get
```

the application separates the operations:

```text
Commands
   ├── Add
   ├── Update
   └── Delete

Queries
   └── Get
```

This makes the application easier to extend as the number of operations grows.

MediatR acts as the mediator between the controller and the appropriate handler.

---

# Repository and EF Core

The application intentionally uses `AppDbContext` directly inside CQRS handlers rather than introducing a generic repository layer.

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
EF Core
     │
     ▼
SQLite
```

The project treats `DbContext` as the data-access abstraction and avoids adding another repository abstraction on top of EF Core where it would provide little additional value.

---

# Authentication & Authorization

The backend uses **JWT Bearer authentication**.

Authentication endpoints:

```text
POST /api/auth/register
POST /api/auth/login
```

A successful authentication returns a JWT containing user information.

Protected expense endpoints use:

```csharp
[Authorize]
```

The authenticated user's identity is obtained from JWT claims.

The application then uses the user's email as the `UserId` associated with expenses.

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
User-scoped Expenses
```

This prevents users from retrieving or modifying another user's expenses.

---

# API Endpoints

## Authentication

| Method | Route                | Description         |
| ------ | -------------------- | ------------------- |
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login`    | Authenticate a user |

## Expenses

| Method   | Route               | Description                             |
| -------- | ------------------- | --------------------------------------- |
| `GET`    | `/api/expense`      | Get expenses for the authenticated user |
| `GET`    | `/api/expense/{id}` | Get a specific expense                  |
| `POST`   | `/api/expense`      | Create an expense                       |
| `PUT`    | `/api/expense/{id}` | Update an expense                       |
| `DELETE` | `/api/expense/{id}` | Delete an expense                       |

---

# User Data Isolation

One of the important backend responsibilities is ensuring that users only access their own expenses.

For example, the query handler filters by the authenticated user's identity:

```text
Authenticated User
        │
        ▼
     UserId
        │
        ▼
┌─────────────────────────┐
│ Expenses                │
│                         │
│ User A → Expense 1      │
│ User A → Expense 2      │
│ User B → Expense 3      │
└─────────────────────────┘
        │
        ▼
Only User A's records
are returned to User A
```

Update and delete operations also verify that the expense belongs to the current user before modifying it.

---

# Data Model

## User

```text
User
├── Id
├── Email
├── Name
└── PasswordHash
```

## Expense

```text
Expense
├── Id
├── Title
├── Amount
├── Category
├── Date
└── UserId
```

The database uses indexes for user lookup and enforces unique email addresses.

```text
Users
  │
  └── Email → Unique Index

Expenses
  │
  └── UserId → Index
```

---

# AutoMapper

AutoMapper is used to map between different application models.

For example:

```text
DTO / Command
      │
      ▼
  AutoMapper
      │
      ▼
Expense Entity
```

This helps keep API/request models separate from persistence entities.

---

# Frontend — ExpenseTrackerUI

The frontend is implemented using **Blazor Server** with Interactive Server rendering.

The UI provides a complete expense-management experience.

### Main screens and components

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

# Expense Dashboard

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

Conceptually:

```text
┌─────────────────────────────────────────────┐
│              Expense Dashboard              │
├───────────┬───────────┬───────────┬─────────┤
│   Total   │  Average  │  Highest  │  Count  │
├───────────┴───────────┴───────────┴─────────┤
│                                             │
│          Spending by Category               │
│              Chart.js                      │
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

# Chart.js Integration

The application uses Chart.js to visualize spending by category.

Blazor communicates with JavaScript using **JS Interop**.

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
Expense Chart
```

The chart groups expenses by category and calculates the total spending for each category before rendering the visualization.

---

# Reusable Blazor Components

The UI contains reusable components rather than putting everything into one Razor page.

For example:

```text
ExpenseChart
StatCard
DeleteConfirmModal
LoadingSkeleton
```

This allows common UI behavior to be isolated and reused.

Example:

```text
Expenses.razor
      │
      ├── StatCard
      ├── StatCard
      ├── StatCard
      ├── ExpenseChart
      ├── LoadingSkeleton
      └── DeleteConfirmModal
```

---

# UI Authentication Flow

The frontend maintains its own authenticated UI session using cookie authentication.

The overall authentication flow is:

```text
User
 │
 ▼
Blazor Login Page
 │
 ▼
POST /api/auth/login
 │
 ▼
ASP.NET Core API
 │
 ▼
JWT Token
 │
 ▼
Blazor UI
 │
 ▼
Cookie Authentication Session
 │
 ▼
Authenticated UI
```

The token is retained as part of the authenticated claims and is used when making user-scoped API requests.

---

# API Communication

The frontend uses:

```text
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

The service abstraction keeps HTTP communication separate from Razor components.

This means the UI does not need to directly construct API requests throughout every page.

---

# UI/UX Features

The project goes beyond basic CRUD functionality.

### Dark Mode

The application supports light and dark themes with theme persistence.

### Toast Notifications

Users receive feedback for successful and failed operations.

### Loading Skeletons

Skeleton placeholders are displayed while data is being loaded.

### Delete Confirmation

Deleting an expense requires confirmation through a reusable modal component.

### Responsive Layout

The interface is designed to work across desktop and mobile screen sizes.

### Data Visualization

Expense categories are visualized using Chart.js.

---

# Complete Request Flow

A typical expense creation flow looks like this:

```text
User
 │
 ▼
Blazor Expense Page
 │
 ▼
IExpenseService
 │
 ▼
HttpClient
 │
 │ POST /api/expense
 ▼
SmartExpenseTracker API
 │
 ▼
ExpenseController
 │
 ▼
IMediator
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
EF Core
 │
 ▼
SQLite
 │
 ▼
Response
 │
 ▼
Blazor UI
```

---

# Complete Read Flow

For retrieving expenses:

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
IMediator
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
Filter by UserId
    │
    ▼
Expenses
    │
    ▼
Blazor Dashboard
```

---

# Project Structure

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

# Database

SQLite is used for local persistence.

```text
                    SQLite
                       │
            ┌──────────┴──────────┐
            │                     │
          Users                 Expenses
            │                     │
            │                     │
          Id                    Id
          Email                 Title
          Name                  Amount
          PasswordHash          Category
                                Date
                                UserId
```

Entity Framework Core handles:

* Database access
* Entity mapping
* Migrations
* Querying
* Persistence

---

# Local Development

## Prerequisites

* Visual Studio 2026 or compatible .NET IDE
* .NET 10 SDK
* Git
* HTTPS development certificate

No external database server is required because the application uses SQLite.

---

# Running the Application

Open:

```text
SmartExpenseTracker.slnx
```

The solution contains both:

```text
SmartExpenseTracker
ExpenseTrackerUI
```

### Backend

Run:

```text
SmartExpenseTracker
```

Default development URL:

```text
https://localhost:7162
```

Swagger:

```text
https://localhost:7162/swagger
```

### Frontend

Run:

```text
ExpenseTrackerUI
```

Default development URL:

```text
https://localhost:7184
```

The frontend is configured to communicate with:

```text
https://localhost:7162/
```

---

# Suggested Demo Flow

A complete demonstration can be performed as follows.

### 1. Register

Open the Blazor application and create a user.

```text
Login → Register
```

### 2. Login

Authenticate using the newly created credentials.

### 3. Add Expenses

Create several expenses:

```text
Food      ₹500
Travel    ₹1500
Bills     ₹2500
Shopping  ₹1000
```

### 4. View Dashboard

The dashboard calculates:

```text
Total
Average
Highest
Count
```

and displays the category distribution using a chart.

### 5. Edit Expense

Open an expense and modify:

```text
Title
Amount
Category
```

### 6. Delete Expense

Use the delete action and confirm through the confirmation modal.

### 7. Verify User Isolation

Create another user and verify that each user sees only their own expenses.

---

# Design Decisions

## Why Blazor Server?

Blazor Server allows the frontend to be developed using C# and Razor components while still providing a rich interactive application experience.

It also allows the project to demonstrate:

* Component-based UI
* Dependency injection
* Authentication
* JavaScript interop
* HTTP API communication

---

## Why CQRS?

CQRS separates reads from writes.

```text
Write
  ↓
Command
  ↓
Command Handler


Read
  ↓
Query
  ↓
Query Handler
```

This keeps individual operations focused and provides a structure that can grow as the application becomes more complex.

---

## Why MediatR?

MediatR decouples controllers from individual handlers.

Instead of:

```text
Controller → AddExpenseHandler
```

the controller communicates through:

```text
Controller → IMediator → Handler
```

This keeps controllers thin and allows request handling to be organized independently.

---

## Why AutoMapper?

AutoMapper reduces repetitive mapping code between DTOs, commands, and entities.

```text
DTO
 ↓
AutoMapper
 ↓
Command / Entity
```

---

## Why SQLite?

SQLite provides:

* Zero database-server setup
* Simple local development
* Easy portability
* EF Core support

This makes it suitable for a portfolio and learning project.

---

## Why JWT?

JWT provides a stateless authentication mechanism for the backend API.

The API can validate the token and obtain the authenticated user's claims for authorization and user-scoped data access.

---

# Security Considerations

The current project is a **prototype / portfolio implementation**, not a production authentication system.

One important limitation is that password hashing currently uses **SHA-256**.

For production authentication, password storage should use a password-specific hashing algorithm such as:

```text
BCrypt
Argon2
PBKDF2
```

Additional production improvements could include:

* Refresh tokens
* Token rotation
* Stronger password policies
* Rate limiting
* Account lockout
* Secure secret management
* HTTPS enforcement
* More granular authorization policies

---

# Production Considerations

The project is intentionally lightweight for local development.

A production evolution could look like:

| Current                        | Production Evolution                   |
| ------------------------------ | -------------------------------------- |
| SQLite                         | PostgreSQL / SQL Server                |
| SHA-256 password hashing       | Argon2 / BCrypt / PBKDF2               |
| Local JWT configuration        | Secure secret/key management           |
| Simple authentication          | Refresh tokens + token rotation        |
| Basic logging                  | Structured logging                     |
| Local development              | Containerized deployment               |
| Basic API                      | Rate limiting + monitoring             |
| Single UI/API deployment model | Scaled frontend/backend infrastructure |

---

# What This Project Demonstrates

### Backend Development

* ASP.NET Core Web API
* REST API design
* Dependency Injection
* Entity Framework Core
* SQLite
* Async programming
* Authentication and authorization

### Architecture

* CQRS
* MediatR
* Separation of concerns
* Handler-based request processing
* DTO contracts
* Data-access abstraction

### Frontend Development

* Blazor Server
* Razor components
* Reusable components
* Component parameters
* Event callbacks
* HttpClient
* JavaScript Interop
* Responsive UI

### Security

* JWT authentication
* Claims-based identity
* `[Authorize]`
* User-scoped data access
* Password storage considerations

### UI Engineering

* Loading states
* Toast notifications
* Modal dialogs
* Dark mode
* Responsive design
* Data visualization

---

# Interview Talking Points

This project can be used to discuss several important .NET interview topics.

### Architecture

* Why did you choose CQRS?
* What problem does MediatR solve?
* Why separate commands and queries?
* Why keep controllers thin?
* Why use DTOs?
* Why use EF Core directly instead of a generic repository?

### ASP.NET Core

* How does dependency injection work?
* How does middleware work?
* How does `[Authorize]` work?
* How does JWT authentication work?
* How are claims populated?
* How does model binding work?

### EF Core

* What is `DbContext`?
* What is change tracking?
* What are migrations?
* How does EF Core translate LINQ to SQL?
* Why create an index on `UserId`?
* Why enforce a unique index on email?

### CQRS / MediatR

* What is CQRS?
* Command vs Query?
* What is `IRequest<T>`?
* What is `IRequestHandler<TRequest,TResponse>`?
* Why use MediatR?
* What are the advantages and disadvantages of CQRS?
* When would CQRS be overengineering?

### Blazor

* Blazor Server vs Blazor WebAssembly?
* How does component rendering work?
* What is `IJSRuntime`?
* How does dependency injection work in Blazor?
* How do parent and child components communicate?
* What are `EventCallback`s?
* How does authentication work in Blazor?

### Security

* How does JWT authentication work?
* Where should JWT secrets be stored?
* Why is SHA-256 not ideal for password hashing?
* How would you implement refresh tokens?
* How do you prevent one user from accessing another user's data?

---

# Example Interview Explanation

A concise way to explain the project in an interview:

> **"SmartExpenseTracker is a full-stack .NET 10 application with an ASP.NET Core Web API backend and a Blazor Server frontend. The backend uses CQRS with MediatR, where commands and queries have separate handlers, while EF Core with SQLite handles persistence. The API uses JWT authentication and claims-based user identification to ensure users can only access their own expenses. The Blazor UI communicates with the API through an HttpClient-based service layer and provides reusable components, dashboard statistics, Chart.js visualizations, dark mode, loading states, and CRUD functionality. The project demonstrates both backend architecture and practical full-stack C# development."**

---

# Future Improvements

Potential extensions include:

* Refresh token authentication
* Role-based authorization
* Category management
* Monthly expense reports
* Budget management
* Recurring expenses
* Export to CSV/PDF
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

# License

This project is intended for:

* Learning
* Experimentation
* Portfolio demonstration
* Interview preparation
* Exploring modern .NET development
