# Coding Prompt Used for VS Code

The following detailed prompt was used as the coding plan. It intentionally breaks development into five reviewable stages.

> Build a beginner-friendly C# microservice application named **HackTrack** for student hackathon participants.
>
> **Required technology**
> - C# and .NET 10
> - ASP.NET Core Minimal APIs
> - Entity Framework Core
> - SQLite
> - Docker with Linux-based .NET 10 images
> - Docker Compose to run all services together
>
> **Architecture**
> Create exactly three microservices under a `src` folder:
>
> 1. `HackathonService`
>    - Owns hackathon event data.
>    - Fields: Id, Name, Organizer, Deadline, Location, Status.
>    - CRUD endpoints under `/hackathons`.
>    - Own SQLite database named `hackathons.db`.
>
> 2. `ProjectService`
>    - Owns hackathon project data.
>    - Fields: Id, HackathonId, Name, RepoUrl, Stage, Notes.
>    - CRUD endpoints under `/projects`.
>    - Own SQLite database named `projects.db`.
>    - Use an `HttpClient` to call HackathonService. When creating or changing a project, verify that the supplied HackathonId can be found in HackathonService.
>    - Add `/projects/{id}/summary` to return the local project plus the related hackathon information from HackathonService.
>
> 3. `TaskService`
>    - Owns work-item data.
>    - Fields: Id, ProjectId, Title, Priority, Status, DueDate.
>    - CRUD endpoints under `/tasks`.
>    - Add `/tasks/project/{projectId}` to list tasks for one project.
>    - Own SQLite database named `tasks.db`.
>
> Each service must have a `/health` endpoint and a `/` endpoint that explains what the service does. Each service must create its SQLite schema automatically on startup with `Database.EnsureCreated()` and include a small amount of seed data so the APIs are easy to demonstrate.
>
> Keep service databases separate. Do not create Entity Framework relationships across service boundaries. Other services should refer to external records only by ID.
>
> **Stage 1 — Scaffold and models**
> Create the folder structure, .NET 10 project files, entity classes, request DTOs, and DbContext classes. Review the service boundaries before continuing.
>
> **Stage 2 — REST APIs and persistence**
> Implement the CRUD endpoints, SQLite configuration, database creation, seed data, root endpoints, and health endpoints. Keep Program.cs readable for a student learning Minimal APIs. Review the endpoints and data flow before continuing.
>
> **Stage 3 — Service-to-service communication**
> Add an `IHackathonClient` and `HackathonClient` to ProjectService using `HttpClient`. Configure the HackathonService base URL from configuration. Use the client to validate HackathonId on project create/update and to build the project summary endpoint. Handle a missing or unreachable HackathonService gracefully instead of crashing.
>
> **Stage 4 — Docker**
> Add one multi-stage Dockerfile per service using the official `mcr.microsoft.com/dotnet/sdk:10.0` build image and `mcr.microsoft.com/dotnet/aspnet:10.0` runtime image. Add a root `docker-compose.yml` that exposes HackathonService on port 5001, ProjectService on 5002, and TaskService on 5003. Give each service its own named volume for its SQLite database. Inside Docker, configure ProjectService to reach HackathonService by the Compose service name `http://hackathons:8080`, not localhost.
>
> **Stage 5 — Documentation and verification**
> Add a README with an ASCII architecture diagram, startup instructions, endpoint list, and example requests. Add a GitHub Actions workflow that restores and builds each .NET project using .NET 10. Review the final repository for accidental shared databases, hard-coded container localhost URLs, unnecessary infrastructure, and confusing code.
>
> Do not add authentication, Kubernetes, a message broker, an API gateway, a frontend framework, or cloud dependencies. The purpose is to learn the fundamentals of small C# microservices.
