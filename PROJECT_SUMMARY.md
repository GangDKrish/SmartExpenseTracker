# SmartExpenseTracker - Project Summary

## Overview
SmartExpenseTracker is a full-stack prototype application for personal expense management, built with a **clean architecture** approach using .NET 10. It consists of two projects: a back-end Web API and a front-end Blazor Server UI.

---

## Architecture

| Layer | Project | Technology |
|-------|---------|------------|
| Back-End API | `SmartExpenseTracker` | ASP.NET Core Web API (.NET 10) |
| Front-End UI | `ExpenseTrackerUI` | Blazor Server (.NET 10) |
| Database | SQLite | Entity Framework Core 10 |

The two projects communicate via HTTP REST calls. CORS is configured to allow the Blazor UI to call the API.

---

## Back-End (`SmartExpenseTracker`)

### Key Design Patterns

1. **CQRS (Command Query Responsibility Segregation)** using **MediatR**
	- **Commands**: `AddExpenseCommand` / `UpdateExpenseCommand` / `DeleteExpenseCommand` — handle write operations
   - **Queries**: `GetExpensesQuery` — handles read operations
   - Each command/query has a dedicated handler class, promoting single-responsibility

2. **AutoMapper** — maps between domain models, DTOs, and commands to keep layers decoupled

3. **Repository pattern via EF Core** — `AppDbContext` manages `Expense` and `User` entities

### API Endpoints

| Method | Route | Purpose |
|--------|-------|---------|
| POST | `/api/auth/register` | Register a new user (email + password) |
| POST | `/api/auth/login` | Authenticate and return user info |
| GET | `/api/expense` | Get all expenses for the logged-in user |
| GET | `/api/expense/{id}` | Get a specific expense by ID |
| POST | `/api/expense` | Add a new expense |
| PUT | `/api/expense/{id}` | Update an existing expense |
| DELETE | `/api/expense/{id}` | Delete an expense |

### Authentication
- Authentication uses **JWT Bearer tokens** for secured API access
- Auth endpoints (`/api/auth/register`, `/api/auth/login`) return token + user payload
- Expense endpoints are protected with `[Authorize]` and read user identity from JWT claims
- Password hashing currently uses **SHA-256** (prototype-level; can be upgraded to BCrypt/Argon2)

### Data Model

**Expense**: Id, Title, Amount, Category, Date, UserId (user's email)  
**User**: Id, Email, Name, PasswordHash

### Database
- **SQLite** with EF Core migrations
- Migrations track schema evolution: initial create → added UserId → added User table → added Name field

---

## Front-End (`ExpenseTrackerUI`)

### Technology
- **Blazor Server** with Interactive Server render mode
- **Bootstrap 5** for responsive layout
- **Chart.js** via JS interop for data visualization

### Key Pages & Components

| Page/Component | Purpose |
|----------------|---------|
| `Pages/Login.razor` | User login/registration form |
| `Pages/Expenses.razor` | Main dashboard — full CRUD, statistics, charts |
| `Components/ExpenseChart.razor` | Chart.js-powered spending visualization |
| `Components/StatCard.razor` | Reusable statistic card (total, average, highest, count) |
| `Components/DeleteConfirmModal.razor` | Confirmation dialog before deleting |
| `Components/LoadingSkeleton.razor` | Loading placeholder UI |
| `Components/Layout/MainLayout.razor` | App shell with navigation |

### UI Features
- **Dashboard with statistics**: Total spent, average expense, highest expense, item count
- **Category-based pie chart** for spending breakdown
- **Edit Expense modal** opened from row-level pen icon (before delete)
- **PUT-based update flow** from modal fields (title, amount, category)
- **Dark mode toggle** with theme persistence
- **Toast notifications** for success/error feedback
- **Loading skeletons** for better UX during data fetch
- **Responsive design** — works on mobile and desktop

### Service Layer
- `IExpenseService` / `ExpenseService` — abstracts API calls to the back-end
- Sends `Authorization: Bearer <token>` for user-scoped API access
- Supports `GET`, `POST`, `PUT`, and `DELETE` expense operations

---

## How It All Fits Together

```
┌──────────────────────────────┐                                   ┌──────────────────────────────┐
│ ExpenseTrackerUI             │                                   │ SmartExpenseTracker          │
│ (Blazor Server)              │                                   │ (ASP.NET Core Web API)       │
│                              │ 1) POST /api/auth/login/register  │                              │
│ • Login/Register page        │ ─────────────────────────────────► │ • AuthController             │
│ • Expense dashboard          │ ◄───────────────────────────────── │ • Issues JWT token           │
│ • Edit modal (pen icon)      │    token + user payload           │                              │
│ • Add/Delete/Edit actions    │                                   │                              │
│                              │ 2) GET/POST/PUT/DELETE /api/expense
│                              │    Authorization: Bearer <token>  │
│                              │ ─────────────────────────────────► │ • ExpenseController [Authorize]
│                              │ ◄───────────────────────────────── │ • Claims -> user identity    │
│                              │    user-scoped JSON responses     │ • CQRS handlers + AutoMapper │
└──────────────────────────────┘                                   └──────────────┬───────────────┘
                                                                                  │
                                                                                  │ EF Core
                                                                                  ▼
                                                                      ┌──────────────────────────┐
                                                                      │ SQLite (ExpenseDb.db)    │
                                                                      │ Expenses + Users         │
                                                                      └──────────────────────────┘
```

---

## Design Decisions & Rationale

| Decision | Why |
|----------|-----|
| CQRS with MediatR | Demonstrates separation of read/write concerns; scalable pattern for larger apps |
| AutoMapper | Keeps API contracts (DTOs) decoupled from domain models |
| SQLite | Zero-config database ideal for prototyping; no server setup needed |
| Blazor Server | Rich interactive UI in C# without JavaScript SPA complexity |
| JWT Bearer authentication | Standard secure API auth pattern; claims-based user identity and endpoint protection |
| Chart.js via JS Interop | Shows Blazor's ability to integrate with JavaScript libraries |

---

## Technologies Used

- .NET 10 (ASP.NET Core + Blazor Server)
- Entity Framework Core 10 (SQLite provider)
- MediatR 14 (CQRS mediator)
- AutoMapper 16 (object mapping)
- Microsoft.AspNetCore.Authentication.JwtBearer (JWT authentication)
- Swashbuckle (Swagger/OpenAPI documentation)
- Bootstrap 5 (CSS framework)
- Chart.js (data visualization via JS interop)

---

## How to Run

1. **Start the API**: Run `SmartExpenseTracker` project (defaults to `https://localhost:7162`)
2. **Start the UI**: Run `ExpenseTrackerUI` project (defaults to `https://localhost:7184`)
3. Open the browser to the UI URL, register a user, and start tracking expenses

---

## Talking Points for Interviews/Panels

- **Clean Architecture**: Separation of concerns with Controllers → CQRS → EF Core
- **CQRS Pattern**: Demonstrates understanding of enterprise patterns even in a prototype
- **Full-Stack C#**: Single language across API and UI layers
- **Modern .NET**: Targeting the latest .NET 10 with minimal hosting model
- **UX Polish**: Loading states, toast notifications, dark mode, responsive design, charts, and modal-based edit flow
- **Data Integrity**: Proper validation with data annotations, user-scoped data isolation
- **Security**: JWT-based endpoint protection with claims-driven user scoping
- **Extensibility**: Easy to harden password storage, add refresh tokens, or swap SQLite for SQL Server
