# 🗂️ TempoSort — Requirements Document

> A powerful task, reminder, and team productivity system. Designed to showcase professional backend skills with real-world architecture, RBAC, email scheduling, and realtime notifications.

---

## ✅ Overview

TempoSort is a backend-driven app with a modular architecture that supports:

- User authentication with JWT
- Task management with scheduling and metadata
- Realtime and email notifications
- Role-based access control (RBAC) with team support
- Hangfire for background jobs
- SignalR for live messages
- PostgreSQL as primary DB
- SMTP for email notifications
- Swagger for API testing

---

## 🧱 Project Structure

| Project                      | Responsibility                          |
|-----------------------------|------------------------------------------|
| `NotificationService.API`   | Controllers, routing, middleware, Swagger |
| `NotificationService.Business` | Core logic, services, SignalR, email, Hangfire |
| `NotificationService.DataAccess` | Repositories, Dapper SQL logic          |
| `NotificationService.Models` | DTOs, enums, data contracts              |

---

## 🔐 Authentication & Authorization

- `SignUp`: Creates user + sends verification email
- `Login`: Verifies credentials, returns JWT
- JWT token is used for all authorized endpoints
- Middleware handles user extraction
- Role-based access via claims & permission checks

---

## 📬 Notification Features

- Create in-app notifications
- Fetch all/unread notifications
- Patch to mark notifications as read
- SignalR for real-time delivery
- Email notifications via SMTP
- Hangfire used for scheduling recurring/email jobs

---

## 🛠️ Health Check Endpoint

- Checks:
  - Application is alive
  - Database connectivity
  - SMTP server connectivity
- Available at `/health`

---

## 📋 Task Management

Each task includes:

| Field              | Type                     |
|-------------------|--------------------------|
| `type`            | once / repetitive        |
| `allDay`          | true / false             |
| `level`           | informative / important / urgent |
| `assignedTo`      | userId (self or other)   |
| `emailReminder`   | true / false             |
| `reminderFreq`    | (optional override)      |
| `description`     | string                   |

Endpoints:
- `POST /tasks` – Create new task
- `GET /tasks` – List tasks
- `PATCH /tasks/{id}` – Update/complete a task

---

## 👥 Team & RBAC

Supports multi-user teams and role-based access.

### Entities

- `Team`: created by a user
- `TeamMember`: user-role mapping within a team
- `Role`: defines access level (e.g., Manager, Member)
- `Permission`: what actions a role can perform
- `RolePermission`: link between roles and permissions

### Example Roles

| Role     | Permissions                                 |
|----------|---------------------------------------------|
| Admin    | All                                         |
| Manager  | Create team, invite users, assign tasks     |
| Member   | View & complete assigned tasks              |
| Viewer   | View tasks and team                         |

### Endpoints

- `POST /teams` – Create team
- `POST /teams/{id}/members` – Add member (Manager only)
- `GET /teams/{id}/members` – List team members
- `PATCH /teams/{id}/members/{userId}/role` – Change role

RBAC enforced via `[HasPermission("action")]` style checks or middleware.

---

## 📨 Email Scheduling (SMTP + Hangfire)

- Configured via `appsettings.json`
- Used for:
  - Account verification
  - Task reminder emails
  - Summary emails (e.g., daily digest)
- EmailService reads SMTP credentials from config
- Hangfire dashboard enabled at `/hangfire`

---

## 🟢 SignalR (Realtime Layer)

- Sends welcome message after login
- Pushes notification instantly when assigned
- Connected on frontend with `HubConnection`

---

## 🔄 Future Enhancements

| Feature                  | Priority | Description                                |
|--------------------------|----------|--------------------------------------------|
| Google Calendar Sync     | ⭐⭐      | Add task to calendar using OAuth           |
| GitHub Webhook Integration | ⭐⭐    | Create task from GitHub issue              |
| Custom Role Templates    | ⭐⭐      | Define team-specific role sets             |
| Slack/Telegram Integrations | ⭐   | Push reminders to team chat apps           |
| Frontend (React/Next.js) | ⭐⭐⭐     | Full client interface for tasks & teams    |
| Mobile App (Flutter/RN)  | ⭐⭐      | Android/iOS client                         |

---

## 🧑‍💻 Developer Notes

- Uses Dapper for performance
- PostgreSQL is real (not in-memory)
- Email uses real SMTP credentials
- JWT tokens signed with 256-bit symmetric key
- `.gitignore` excludes secrets in `appsettings.*.json`
- Designed for local development with potential cloud deployment

---

## 📦 Environment Files

Add to `.gitignore`:
