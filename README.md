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

---

App available at: `http://localhost:4200`

> Both must run simultaneously. The Angular dev proxy forwards `/api` calls to the .NET 10 server.

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/tasks` | Get all tasks (optional: `?title=filter`) |
| `POST` | `/api/tasks` | Create a new task |
| `PATCH` | `/api/tasks/{id}/status` | Mark a task as Done |

## Design Decisions

### Server (.NET 10)

- **In-memory storage**: A singleton service holds a `List<TaskItem>` — no database, no file I/O, satisfies the requirement cleanly.
- **Minimal API pattern**: ASP.NET Core Minimal APIs — modern, concise, no MVC controller boilerplate.
- **CORS**: Explicitly configured to allow `http://localhost:4200` in development.
- **Access to API Documentation, Scalar UI**: `http://localhost:5000/scalar/v1`. The raw OpenAPI JSON specification is available at: `http://localhost:5000/openapi/v1.json`
- **Logs** - it are implemented using Serilog, and written to: server/logs/taskmanager-YYYYMMDD.log.

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

---
