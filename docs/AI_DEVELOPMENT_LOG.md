# AI Development Review Log

This file records what was reviewed after each development stage.

## Stage 1 — Scaffold and models

Created three .NET 10 Minimal API projects and gave each service its own model and EF Core DbContext. The key review decision was to avoid one shared database. `HackathonId` and `ProjectId` are external IDs across service boundaries rather than EF Core relationships between services.

## Stage 2 — REST APIs and persistence

Added CRUD endpoints, `/health`, SQLite configuration, `Database.EnsureCreated()`, and seed data. The endpoints were kept intentionally simple so the behavior can be tested with a browser, REST client, PowerShell, or curl.

## Stage 3 — Service-to-service communication

Added `IHackathonClient` and `HackathonClient` to Project Service. The client verifies a hackathon exists before a project is added or changed. A summary endpoint combines project data with hackathon data over HTTP.

A review point here was failure handling. A microservice call can fail even when the calling service is healthy, so `HackathonClient` catches `HttpRequestException` and returns `null` instead of allowing the Project Service to crash.

## Stage 4 — Docker

Added one multi-stage Dockerfile per API and a root Docker Compose file. During review, the important networking correction was to make Project Service use `http://hackathons:8080` inside Docker rather than `localhost`. Every container has its own network namespace, so `localhost` would point back to the Project Service itself.

Each SQLite database is stored in its own Docker named volume.

## Stage 5 — Documentation and verification

Added README documentation, assignment prompts, reflection, and a GitHub Actions build workflow. The final review checked for:

- separate service databases;
- no cross-service EF Core foreign keys;
- correct Docker service-name networking;
- no unnecessary API gateway, Kubernetes, message broker, or authentication system;
- clear service responsibilities;
- .NET 10 and EF Core SQLite package configuration.
