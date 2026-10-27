# HackTrack Architecture

## Application area

HackTrack is intended for student developers who participate in hackathons. It keeps three related concerns separate: events, projects, and project tasks.

## Service boundaries

### Hackathon Service

This service is the source of truth for hackathon events. Other services do not read its SQLite file directly. The Project Service asks it for hackathon information over HTTP.

### Project Service

This service owns the projects a student is building. It stores the related hackathon as an integer `HackathonId`, but it does not create an EF Core foreign-key relationship to the Hackathon Service database. This is important because each service owns its own data.

### Task Service

This service owns project tasks. It stores `ProjectId` as an external reference and offers an endpoint for listing all tasks for a project. Keeping this service independent means its task data can evolve without changing the Project Service database.

## Communication

```text
                 HTTP GET /hackathons/{id}
Project Service ----------------------------> Hackathon Service
      |                                              |
      v                                              v
 projects.db                                    hackathons.db

Task Service
     |
     v
 tasks.db
```

Only the Project Service currently needs synchronous service-to-service communication. This keeps the educational example small while still demonstrating how one microservice can call another.

## Database ownership

Each container gets a separate SQLite database and a separate Docker volume:

- `hackathon-data` -> `hackathons.db`
- `project-data` -> `projects.db`
- `task-data` -> `tasks.db`

No service opens another service's database file.

## Docker networking

Docker Compose gives each container a DNS name equal to its service name. Project Service therefore reaches Hackathon Service using `http://hackathons:8080` when the application is running in Docker.

Using `localhost:5001` from inside the Project Service container would be incorrect because `localhost` would refer to the Project Service container itself, not the Hackathon Service container.
