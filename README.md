# Task Manager — Full-Stack Assignment (.NET 10 + Angular 22)

## Project Structure
```
task-manager/
├── server/          # ASP.NET Core (.NET 10) Web API — in-memory storage
├── client/          # Angular 22 standalone app — Signals-based
└── README.md
```

---

## Prerequisites

| Dependency | Version | Notes |
|---|---|---|
| .NET SDK | 10.0+ | https://dotnet.microsoft.com/download |
| Node.js | 22+ | https://nodejs.org/ |
| Angular CLI | 22+ | `npm install -g @angular/cli` |

---

## How to Run

### 1. Server — ASP.NET Core (.NET 10) Web API
```bash
cd server
dotnet run
```
API available at: `http://localhost:5000`

### 2. Client — Angular 22
```bash
cd client
npm install
ng serve
```
App available at: `http://localhost:4200`

> Both must run simultaneously. The Angular dev proxy forwards `/api` calls to the .NET 10 server.

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/tasks` | Get all tasks (optional: `?title=filter`) |
| `POST` | `/api/tasks` | Create a new task |
| `PATCH` | `/api/tasks/{id}/status` | Mark a task as Done |

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
- **Native AOT ready**: .NET 10 project configured for AOT-compatible serialization via `System.Text.Json` source generators.
- **CORS**: Explicitly configured to allow `http://localhost:4200` in development.
- **FluentValidation**: Used for server-side request validation (title required, 3–100 chars).

### Client (Angular 22)
- **Signal-based state**: Component state managed entirely with `signal()` and `computed()` — no RxJS `BehaviorSubject` needed for local state.
- **Zoneless change detection**: Uses `provideExperimentalZonelessChangeDetection()` — no `zone.js` dependency, faster and cleaner.
- **Standalone Components**: No NgModules anywhere — aligns with Angular 22 best practices.
- **Reactive Forms with Signals**: Form validation (title required, min 3 / max 100 chars) using reactive forms integrated with signal-based UI feedback.
- **Status transition is one-way**: Open → Done only. UI disables the button once a task is marked Done.
- **Search**: Title filtering is reactive via a `signal` bound to a search input; the API also supports `?title=` server-side filtering.
- **`@let` template syntax**: Used for cleaner template variable declarations (Angular 22 stable feature).

---

## Assumptions

- No authentication or authorization is required.
- Data does not persist between server restarts (in-memory only).
- Tasks are displayed sorted by creation date, newest first.
- A task's title and description cannot be edited after creation — only status can change.
- The app runs in a local development environment only.
- No unit tests are included (out of scope unless specified).

---

## Copilot-Assisted Development Notes

This project was scaffolded and implemented using **GitHub Copilot in VS Code**.
Key Copilot Chat prompts used:

- *"Create an ASP.NET Core .NET 10 minimal API with an in-memory task list, CORS for localhost:4200, FluentValidation, and endpoints to GET, POST, and PATCH task status"*
- *"Generate an Angular 22 standalone component using Signals for state and a reactive form to create a task with title validation (required, 3–100 chars)"*
- *"Create an Angular 22 service to call the task REST API using HttpClient with typed responses"*
- *"Set up zoneless change detection in Angular 22 app.config.ts"*
