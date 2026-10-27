# CIS 3285 — Asg 7 AI Generated Microservice Reflection

**Project:** HackTrack — Hackathon Project Tracker  
**GitHub:** https://github.com/CosmasMandikonza/CIS3285_Asg7_AI_Microservice

## 1. How was AI at designing a microservice architecture?

AI was helpful for turning a general application idea into separate responsibilities. I chose a hackathon project tracker because it is something I can relate to, and the architecture was divided into a Hackathon Service, Project Service, and Task Service. The most useful part was seeing why each service should own its own database instead of putting everything into one SQLite database. I also had to keep the AI from making the design too complicated. For this assignment, adding things like an API gateway, authentication service, message broker, or Kubernetes would have made it harder to understand the basic idea. Limiting the design to three small services made the architecture much easier to follow.

## 2. What prompt did you use for the coding in VS Code?

I used the following staged prompt:

> Build a beginner-friendly C# microservice application named **HackTrack** for student hackathon participants. Use C# and .NET 10, ASP.NET Core Minimal APIs, Entity Framework Core, SQLite, Docker, and Docker Compose.
>
> Create exactly three services. **HackathonService** stores hackathon events with Id, Name, Organizer, Deadline, Location, and Status. **ProjectService** stores projects with Id, HackathonId, Name, RepoUrl, Stage, and Notes. **TaskService** stores project tasks with Id, ProjectId, Title, Priority, Status, and DueDate. Each service must own its own SQLite database and provide basic CRUD endpoints, a root endpoint, and a `/health` endpoint.
>
> ProjectService must use `HttpClient` to call HackathonService. When a project is created or updated, confirm that the HackathonId exists. Add `/projects/{id}/summary` to return project information together with the hackathon information returned by HackathonService. Do not share database files or create EF Core relationships between different services.
>
> **Stage 1:** Create the three .NET projects, models, request DTOs, and DbContext classes. Stop and review the service boundaries.
>
> **Stage 2:** Add SQLite persistence, `Database.EnsureCreated()`, seed data, CRUD endpoints, root endpoints, and health endpoints. Stop and review the API behavior.
>
> **Stage 3:** Add the typed HttpClient from ProjectService to HackathonService, including graceful handling when the other service cannot be reached. Stop and review the inter-service data flow.
>
> **Stage 4:** Add one Linux multi-stage Dockerfile per service using the official .NET 10 SDK and ASP.NET runtime images. Add Docker Compose. Expose ports 5001, 5002, and 5003. Give each SQLite database its own named volume. Inside Docker, use the Compose service name `http://hackathons:8080` for ProjectService to reach HackathonService, not localhost. Stop and review Docker networking and storage.
>
> **Stage 5:** Add a README with an architecture diagram, endpoints, run instructions, and example requests. Add a GitHub Actions workflow that restores and builds all three projects with .NET 10. Review the final project for shared databases, incorrect localhost container URLs, unnecessary infrastructure, and confusing code.
>
> Keep this beginner-friendly. Do not add Kubernetes, a message broker, authentication, an API gateway, Redis, cloud services, or a frontend framework.

## 3. What is something that AI got wrong? Why do you think it made this mistake?

One issue I caught during review was the service-to-service URL for Docker. The first idea was to treat the Hackathon Service like a normal local application and use `localhost`. That does not work between separate Docker containers because `localhost` inside the Project Service container refers to the Project Service container itself. I corrected the Docker configuration so Project Service uses `http://hackathons:8080`, where `hackathons` is the Docker Compose service name. I think this mistake happens because `localhost` is correct when testing services directly on one computer, but container networking changes what `localhost` means.

## 4. Give a brief description of each microservice.

**Hackathon Service:** Stores the hackathons a student is interested in, including the organizer, deadline, location, and current status. It owns `hackathons.db` and provides CRUD endpoints under `/hackathons`.

**Project Service:** Stores projects being built for hackathons, including the repository URL, development stage, and notes. It owns `projects.db`. It also calls the Hackathon Service over HTTP to verify that a HackathonId exists and can return a combined project/hackathon summary.

**Task Service:** Stores individual work items for projects, such as finishing an API or recording a demo. It owns `tasks.db` and provides an endpoint for viewing all tasks associated with a particular ProjectId.

## 5. Select one microservice to give a detailed description of.

I chose the **Project Service** because it demonstrates the most important microservice concept in this example. The Project Service has its own ASP.NET Core Minimal API, EF Core DbContext, and SQLite database. A project stores a `HackathonId`, but the Project Service does not connect to the Hackathon Service database and there is no EF Core foreign key across the two services. Instead, it uses a typed `HttpClient` called `HackathonClient` to make a normal HTTP request to the Hackathon Service.

When a user sends a POST request to create a project, the Project Service first asks the Hackathon Service for `/hackathons/{id}`. If the hackathon is found, the project is stored in `projects.db`. If it cannot confirm the hackathon, the request returns an error instead of adding an invalid reference. The `/projects/{id}/summary` endpoint also demonstrates communication by reading the project from the Project Service database and then requesting the hackathon details from the Hackathon Service before returning both pieces of information together. This helped me understand that microservices can cooperate without sharing the same database or becoming one large application.
