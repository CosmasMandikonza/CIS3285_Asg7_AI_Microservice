# Architecture Design Prompt

This is the prompt used to design the microservice architecture for the assignment.

> Let's make a Hackathon Project Tracker system for student developers who participate in hackathons. The goal is to help a student keep track of hackathon events, the projects they are building for those events, and the tasks needed to finish each project.
>
> I am just learning C#, so keep the architecture simple enough that I can understand how microservices work. We will code it in C# using VS Code and .NET 10. My instructor wants SQLite with Entity Framework Core (EF Core) for the databases. We also need Docker and Docker Desktop for running the app.
>
> Please propose a starting architecture with only 2-4 microservices. Each microservice should have one clear responsibility and should own its own SQLite database instead of sharing a database. Use ASP.NET Core Minimal APIs to keep the code readable.
>
> I want the example to demonstrate at least one real service-to-service HTTP call so I can see how independent services communicate. Avoid advanced infrastructure such as Kubernetes, message brokers, authentication servers, API gateways, Redis, or cloud services because those would distract from the core lesson.
>
> For each service, describe its responsibility, main data model, basic REST endpoints, database, port, and whether it communicates with another service. Then draw a simple text architecture diagram and explain why the service boundaries make sense.

## Refined architecture selected

The final design uses three services:

1. **Hackathon Service** — owns hackathon event and deadline data.
2. **Project Service** — owns project data and calls the Hackathon Service to verify/reference a hackathon.
3. **Task Service** — owns tasks related to projects.

The three-service design was selected because it is large enough to demonstrate independent services and data ownership but still small enough to understand in one assignment.
