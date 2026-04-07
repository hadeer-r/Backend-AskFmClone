# AskFm Clone — Backend API

A feature-rich backend clone of **ASK.fm** built with **ASP.NET Core 9** and a clean N-Tier architecture. Users can ask and answer questions (threads), follow each other, like and comment on threads, and receive real-time notifications — all secured with JWT authentication and Redis-backed token caching.

---

## Table of Contents

- [AskFm Clone — Backend API](#askfm-clone--backend-api)
  - [Table of Contents](#table-of-contents)
  - [Architecture](#architecture)
  - [Tech Stack](#tech-stack)
  - [Features](#features)
    - [Authentication \& Authorization](#authentication--authorization)
    - [User Management](#user-management)
    - [Q\&A Threads](#qa-threads)
    - [Comments](#comments)
    - [Notifications (Real-time)](#notifications-real-time)
    - [Infrastructure](#infrastructure)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Environment Variables](#environment-variables)
    - [Database Setup](#database-setup)
    - [Running the Application](#running-the-application)
  - [API Reference](#api-reference)
  - [Testing](#testing)
  - [Contributors](#contributors)
---

## Architecture

The solution follows a **three-layer (N-Tier)** architecture with a clear separation of concerns, plus a shared library for cross-cutting constants:

```
┌───────────────────────────────────────────────┐
│                  AskFm.API                    │  ← Presentation Layer (Controllers, Middleware)
├───────────────────────────────────────────────┤
│                  AskFm.BLL                    │  ← Business Logic Layer (Services, DTOs, SignalR Hub)
├───────────────────────────────────────────────┤
│                  AskFm.DAL                    │  ← Data Access Layer (EF Core, Repositories, UoW)
├───────────────────────────────────────────────┤
│                  Shared                       │  ← Cross-cutting concerns (Constants)
├───────────────────────────────────────────────┤
│                  Tests                        │  ← Unit Tests (xUnit + Moq)
└───────────────────────────────────────────────┘
```

Key patterns used:

| Pattern | Description |
|---|---|
| **Repository** | Generic `IRepository<T>` abstraction over EF Core `DbSet<T>` |
| **Unit of Work** | `IUnitOfWork` coordinates multiple repositories in a single transaction |
| **Soft Delete** | All entities implement `ITrackable`, deletions set `IsDeleted = true` instead of removing rows, enforced via global query filters |
| **Service Result** | `ServiceResult<T>` wraps operation outcomes with errors, removing the need for exceptions in business logic |

---

## Tech Stack

| Category | Technology |
|---|---|
| **Framework** | ASP.NET Core 9 (.NET 9) |
| **ORM** | Entity Framework Core 9 (Lazy Loading Proxies) |
| **Database** | SQL Server |
| **Caching** | Redis (via `StackExchange.Redis`) |
| **Auth** | ASP.NET Identity + JWT Bearer tokens + Refresh tokens |
| **Real-time** | SignalR |
| **Email** | SMTP (configurable) |
| **Testing** | xUnit, Moq, EF Core InMemory Provider |
| **CI/CD** | GitHub Actions |
| **Code Review** | CodeRabbit (auto-review on all branches) |

---

## Features

### Authentication & Authorization
- User registration and login with JWT access tokens
- Refresh token rotation (stored in HTTP-only cookies)
- Logout with token revocation via Redis cache
- Forgot password / reset password via email
- Account lockout after 5 failed login attempts
- Strong password policy enforcement

### User Management
- View own profile and other users' profiles
- Update profile information (name, bio, avatar)
- Change password
- Soft-delete account
- Follow / unfollow other users with follower & following counts

### Q&A Threads
- Ask questions to other users (with optional anonymous mode)
- Answer received questions
- Thread visibility: Public, Private, Closed, Private Question
- Like / unlike threads
- Save threads for later

### Comments
- Comment on threads with nested replies (parent-child structure)
- Like / unlike comments
- Retrieve likes for a specific comment

### Notifications (Real-time)
- Notification types: Answer, Question, Follow, Comment Like, Question Like, Reply
- Real-time delivery via **SignalR** (`/notificationHub`)
- Paginated notification retrieval
- Filter notifications by type/category
- Mark single or all notifications as read
- Admin-only manual notification creation

### Infrastructure
- Redis-backed JWT token caching for revocation checks
- Automatic audit timestamps (`CreatedAt`, `UpdatedAt`, `DeletedAt`) on all entities
- Global query filters for soft-deleted records
- CORS policy for frontend integration
- Swagger / OpenAPI documentation with JWT auth support

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (local or remote instance)
- [Redis](https://redis.io/download/) (default: `localhost:6379`)

### Installation

```bash
# Clone the repository
git clone https://github.com/hadeer-r/Backend-AskFmClone.git
cd Backend-AskFmClone/AskFm

# Restore NuGet packages
dotnet restore
```

### Environment Variables

Create a `.env` file inside `AskFm/AskFm.API/` (use `.env.example` as a reference):

```env
CONNECTION_STRING="Server=localhost;Database=AskFmDb;Trusted_Connection=True;TrustServerCertificate=True;"
ISSUER="YourIssuer"
AUDIENCE="YourAudience"
SIGNINGKEY="YourSuperSecretSigningKeyAtLeast32Characters"
```

Also configure `appsettings.json` for Redis and email:

```jsonc
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "ExpireTimes": {
    "Jwt_Token_Exp": 10,        // Access token lifetime (minutes)
    "Refresh_Token_Exp": 30      // Refresh token lifetime (days)
  },
  "EmailOption": {
    "client": "smtp.gmail.com",
    "password": "your-app-password",
    "from": "your-email@gmail.com",
    "port": 587
  }
}
```

### Database Setup

```bash
# Apply EF Core migrations
dotnet ef database update --project AskFm.DAL --startup-project AskFm.API
```

### Running the Application

```bash
dotnet run --project AskFm.API
```

The API will be available at `https://localhost:5001` (or the port shown in the console). In development mode, Swagger UI is accessible at the root.

---

## API Reference

The API exposes RESTful endpoints grouped by domain, plus a SignalR hub for real-time notifications. Swagger UI is available in development mode.

| Group | Base Route | Endpoints |
|---|---|---|
| **Auth** | `/api/Auth` | Register, Login, Refresh Token, Logout, Forgot/Reset Password |
| **User** | `/api/User` | Profile CRUD, Follow/Unfollow, Change Password |
| **Comment** | `/api/Comment` | Comment Likes (Get, Add, Remove) |
| **Notification** | `/api/Notification` | List, Filter by Type, Mark as Read |
| **Real-time** | `/notificationHub` | SignalR hub for live notification delivery |

**[Full API Reference →](API_REFERENCE.md)**

---
## Testing

The project uses **xUnit** with **Moq** for mocking and **EF Core InMemory** provider for data layer tests.

```bash
# Run all tests
dotnet test ./AskFm/Tests/Tests.csproj

```

---

## Contributors

| Contributor | GitHub |
|---|---|
| **Hadeer Ramadan** | [@hadeer-r](https://github.com/hadeer-r) |
| **Ziad Ashraf** | [@ziad-ashraf7](https://github.com/ziad-ashraf7) |
| **Mahmoud Ayman** | [@mahmoud-ayman](https://github.com/mahmoud-ayman) |
