# Task Manager — Full-Stack Assignment (.NET 10 + Angular 22)

## Project Structure

```text
task-manager/
├── server/          # ASP.NET Core (.NET 10) Web API — in-memory storage
├── client/          # Angular 22 standalone app — Signals-based
└── README.md
```

---

## Prerequisites

| Dependency | Version | Notes |
| --- | --- | --- |
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |
| Node.js | 22+ | https://nodejs.org/ |
| Angular CLI | 22+ | `npm install -g @angular/cli` |

---

## How to Run

### 1. Server — ASP.NET Core (.NET 10) Web API

```bash or terminal
cd server
dotnet run
```

API available at: `http://localhost:5000`

### 2. Client — Angular 22

```bash or terminal
cd client
npm install
npm run start
```

App available at: `http://localhost:4200`

> Both must run simultaneously. The Angular dev proxy forwards `/api` calls to the .NET 10 server.

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/tasks` | Get all tasks (optional: `?title=filter`) |
| `POST` | `/api/tasks` | Create a new task |
| `PATCH` | `/api/tasks/{id}/status` | Mark a task as Done |

## API Documentation with Scalar

The server uses **Scalar** to provide interactive OpenAPI documentation. Once the server is running, you can explore and test the API endpoints through a modern, user-friendly interface.

**Access Scalar UI**: `http://localhost:5000/scalar/v1`

### Features

- **Interactive API Explorer**: Test all endpoints directly from your browser
- **OpenAPI 3.0 Specification**: Full API schema with request/response models
- **Try It Out**: Send real requests and see live responses
- **Code Generation**: View sample requests in multiple programming languages
- **Modern UI**: Clean, dark-mode interface with excellent UX

### OpenAPI Specification

The raw OpenAPI JSON specification is available at: `http://localhost:5000/openapi/v1.json`

### Task Model

```json
{
  "id": "guid",
  "title": "string (3–100 chars, required)",
  "description": "string",
  "createdDate": "ISO 8601",
  "status": "Open | Done"
}
```

---

## Design Decisions

### Server (.NET 10)

- **In-memory storage**: A singleton service holds a `List<TaskItem>` — no database, no file I/O, satisfies the requirement cleanly.
- **Minimal API pattern**: ASP.NET Core Minimal APIs — modern, concise, no MVC controller boilerplate.
- **CORS**: Explicitly configured to allow `http://localhost:4200` in development.

### Client (Angular 22)

- **Signal-based state**: Component state managed entirely with `signal()` and `computed()`.
- **Zoneless change detection**: Uses `provideExperimentalZonelessChangeDetection()` — no `zone.js` dependency, faster and cleaner.
- **Standalone Components**: No NgModules anywhere — aligns with Angular 22 best practices.
- **Reactive Forms with Signals**: Form validation (title required, min 3 / max 100 chars) using reactive forms integrated with signal-based UI feedback.
- **Status transition is one-way**: Open → Done only. UI disables the button once a task is marked Done.
- **Search**: Title filtering is reactive via a `signal` bound to a search input; the API also supports `?title=` server-side filtering.

---

## Assumptions

- No authentication or authorization is required.
- Data does not persist between server restarts (in-memory only).
- Tasks are displayed sorted by creation date, newest first.
- A task's title and description cannot be edited after creation — only status can change.
- The app runs in a local development environment only.
- No unit tests are included (out of scope unless specified).

---

## Log File Location

Logs are written to:

```text
server/logs/taskmanager-YYYYMMDD.log
```

Example: `taskmanager-20260610.log`

## Log Format file, for example

```text
2026-06-10 09:00:24.557 +03:00 [INF] Starting Task Manager API
2026-06-10 09:00:26.142 +03:00 [INF] Now listening on: http://localhost:5000
2026-06-10 09:00:27.133 +03:00 [INF] Application started. Press Ctrl+C to shut down.
2026-06-10 09:00:28.067 +03:00 [INF] Hosting environment: Development
2026-06-10 09:00:29.569 +03:00 [INF] Content root path: D:\Projects\Angular\angular22-netcore10-taskmanager-example\server
2026-06-10 09:00:29.657 +03:00 [INF] Request starting HTTP/1.1 GET http://localhost:5000/api/tasks - null null
2026-06-10 09:00:32.137 +03:00 [INF] Executing endpoint 'HTTP: GET /api/tasks/ => GetTasks'
2026-06-10 09:00:32.192 +03:00 [INF] GetTasks endpoint called with title filter: null
2026-06-10 09:00:32.391 +03:00 [INF] GetTasks returned 0 tasks
2026-06-10 09:00:32.541 +03:00 [INF] Setting HTTP status code 200.
```

