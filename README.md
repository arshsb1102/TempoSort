# TempoSort

TempoSort is a backend-first productivity and notification platform built around task management, authentication, reminders, and real-time updates. The project is designed to feel like a real-world SaaS backend: users sign up, verify their email, log in with JWT, create tasks, and receive reminders through background jobs and notification channels.

## What this project does

- User authentication and account verification
- JWT-based authorization for protected endpoints
- Task creation, tracking, completion, and updates
- Email support for verification and reminder flows
- Scheduled background jobs for recurring workflows
- Notification-oriented architecture suitable for team productivity tools
- ASP.NET Core API with Swagger for testing and documentation

## Project architecture

The solution is split into focused layers:

| Project | Purpose |
| --- | --- |
| `NotificationService.API` | API layer, controllers, routing, Swagger, auth setup |
| `NotificationService.Business` | Core business services like auth, tasks, email, jobs |
| `NotificationService.DataAccess` | Repositories and database/query logic |
| `NotificationService.Models` | DTOs, request/response contracts, DB object models |

## Core domain

### Authentication
- `POST /api/auth/signup` creates a new user and sends a verification email
- `POST /api/auth/login` verifies credentials and returns a JWT
- `GET /api/auth/verify-email` confirms the email token
- `POST /api/auth/resend-verification` triggers a new verification email

### Tasks
- `POST /api/task` creates a task for the authenticated user
- `GET /api/task` lists tasks
- `GET /api/task/{id}` fetches a single task
- `PUT /api/task/{id}` updates a task
- `DELETE /api/task/{id}` deletes a task
- `POST /api/task/{id}/toggle-complete` marks a task complete or incomplete

### Notification and scheduling
- Email sending via SMTP is configured in app settings
- Background jobs are handled with Quartz.NET
- Notification/email flows are designed for reminders and user onboarding

## Tech stack

- .NET 8 / ASP.NET Core Web API
- JWT authentication
- PostgreSQL / Dapper-style repository access
- Quartz.NET for scheduled jobs
- Swagger / OpenAPI
- SMTP integration for email delivery
- C# solution-based modular project organization

## Local setup

### Prerequisites

- .NET SDK 8+
- PostgreSQL instance
- SMTP provider or local mail test setup

### Run locally

```bash
cd TempoSort

dotnet restore

dotnet run --project NotificationService.API
```

Then open:

- Swagger UI: `http://localhost:5149/swagger`
- Or the configured HTTPS local endpoint from `launchSettings.json`

## Configuration

Update the API configuration in `NotificationService.API/appsettings.json` with:

- `ConnectionStrings`
- `JwtSettings`
- `SmtpSettings`

This file should normally stay local and not be committed if it contains secrets.

## Why this project is valuable

This project is a strong example of a production-style backend because it combines:

- layered architecture
- authentication and authorization
- asynchronous background jobs
- email workflows
- protected API design
- clean separation between API, business logic, and data access

## FastAPI learning track

A separate learning version of this project is being created in the `Temposort-Python` folder using FastAPI. That version mirrors the same core concepts in Python so you can compare:

- ASP.NET Core controllers vs FastAPI routes
- JWT auth patterns vs Python auth patterns
- task CRUD in a simpler Python stack
- Pydantic schema validation and OpenAPI generation

## Recommended next steps

1. Study the API layer and business layer in `NotificationService.API` and `NotificationService.Business`
2. Trace a request from controller to service to repository
3. Compare the C# design with the Python FastAPI version in `Temposort-Python`
4. Extend the app with filters, pagination, user roles, and dashboard endpoints

---

This project is a strong starting point for learning backend architecture, API design, and practical production patterns.
