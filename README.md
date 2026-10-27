# HackTrack Microservices

CIS 3285 Assignment 7 — AI Generated Microservice

HackTrack is a small microservice system for student developers who want to keep track of hackathons, the projects they are building for those events, and the tasks needed to finish each project.

The project is intentionally small so the service boundaries are easy to see while still demonstrating the main ideas behind microservices: separate APIs, separate data ownership, HTTP communication between services, and containerized deployment.

## Architecture

```text
Client / browser / REST tool
        |
        +--------------------+--------------------+
        |                    |                    |
        v                    v                    v
+----------------+   +----------------+   +----------------+
| Hackathon API  |   | Project API    |   | Task API       |
| port 5001      |<--| port 5002      |   | port 5003      |
+----------------+   +----------------+   +----------------+
        |                    |                    |
        v                    v                    v
 hackathons.db          projects.db           tasks.db
   SQLite                 SQLite                SQLite
```

The Project Service calls the Hackathon Service over HTTP when a new project is created. This verifies that the referenced hackathon exists without sharing the Hackathon Service database.

## Microservices

| Service | Responsibility | Host port | Database |
| --- | --- | ---: | --- |
| Hackathon Service | Create and manage hackathon events | 5001 | `hackathons.db` |
| Project Service | Track projects being built for hackathons | 5002 | `projects.db` |
| Task Service | Track work items for projects | 5003 | `tasks.db` |

Each service is an ASP.NET Core Minimal API targeting .NET 10 and uses Entity Framework Core with SQLite.

## Run with Docker Desktop

From the repository root:

```bash
docker compose up --build
```

Then open or test:

- Hackathon Service: `http://localhost:5001`
- Project Service: `http://localhost:5002`
- Task Service: `http://localhost:5003`

Health endpoints:

```text
GET http://localhost:5001/health
GET http://localhost:5002/health
GET http://localhost:5003/health
```

Stop the system with:

```bash
docker compose down
```

To also remove the SQLite volumes and start with fresh databases:

```bash
docker compose down -v
```

## Example API flow

### 1. View hackathons

```http
GET http://localhost:5001/hackathons
```

### 2. Create a project for hackathon 1

```http
POST http://localhost:5002/projects
Content-Type: application/json

{
  "hackathonId": 1,
  "name": "Culture Compass",
  "repoUrl": "https://github.com/example/culture-compass",
  "stage": "Building",
  "notes": "Demo API and final presentation still needed."
}
```

The Project Service first calls the Hackathon Service to confirm that hackathon `1` exists.

### 3. Add a task for project 1

```http
POST http://localhost:5003/tasks
Content-Type: application/json

{
  "projectId": 1,
  "title": "Record demo video",
  "priority": "High",
  "status": "Todo",
  "dueDate": "2026-11-01T20:00:00"
}
```

## Useful endpoints

### Hackathon Service

```text
GET    /hackathons
GET    /hackathons/{id}
POST   /hackathons
PUT    /hackathons/{id}
DELETE /hackathons/{id}
```

### Project Service

```text
GET    /projects
GET    /projects/{id}
GET    /projects/{id}/summary
POST   /projects
PUT    /projects/{id}
DELETE /projects/{id}
```

`GET /projects/{id}/summary` combines local project data with hackathon data returned by the Hackathon Service.

### Task Service

```text
GET    /tasks
GET    /tasks/{id}
GET    /tasks/project/{projectId}
POST   /tasks
PUT    /tasks/{id}
DELETE /tasks/{id}
```

## Local development without Docker

Run all three services in separate terminals:

```bash
dotnet run --project src/HackathonService/HackathonService.csproj --urls http://localhost:5001
```

```bash
dotnet run --project src/ProjectService/ProjectService.csproj --urls http://localhost:5002
```

```bash
dotnet run --project src/TaskService/TaskService.csproj --urls http://localhost:5003
```

For local development, the Project Service defaults to `http://localhost:5001` for the Hackathon Service.

## Assignment documentation

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — architecture and design decisions
- [`docs/ARCHITECTURE_PROMPT.md`](docs/ARCHITECTURE_PROMPT.md) — prompt used to design the system
- [`docs/CODING_PROMPT.md`](docs/CODING_PROMPT.md) — staged prompt used to generate the code
- [`docs/AI_DEVELOPMENT_LOG.md`](docs/AI_DEVELOPMENT_LOG.md) — review notes for each development stage
- [`SUBMISSION.md`](SUBMISSION.md) — completed reflection answers ready to submit
