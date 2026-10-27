using Microsoft.EntityFrameworkCore;
using ProjectService.Clients;
using ProjectService.Data;
using ProjectService.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=projects.db";
var hackathonServiceUrl = builder.Configuration["Services:HackathonService"]
    ?? "http://localhost:5001";

builder.Services.AddDbContext<ProjectDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddHttpClient<IHackathonClient, HackathonClient>(client =>
{
    client.BaseAddress = new Uri(hackathonServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
    db.Database.EnsureCreated();

    if (!db.Projects.Any())
    {
        db.Projects.AddRange(
            new HackathonProject
            {
                HackathonId = 1,
                Name = "Culture Compass",
                RepoUrl = "https://github.com/example/culture-compass",
                Stage = "Building",
                Notes = "Working on the API and demo flow."
            },
            new HackathonProject
            {
                HackathonId = 2,
                Name = "Cloud Cost Coach",
                RepoUrl = "https://github.com/example/cloud-cost-coach",
                Stage = "Idea",
                Notes = "Architecture still being planned."
            });
        db.SaveChanges();
    }
}

app.MapGet("/", () => Results.Ok(new
{
    service = "Project Service",
    purpose = "Tracks projects being built for hackathons.",
    endpoints = new[] { "/projects", "/projects/{id}", "/projects/{id}/summary", "/health" }
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "projects" }));

app.MapGet("/projects", async (ProjectDbContext db) =>
    Results.Ok(await db.Projects.OrderBy(p => p.Id).ToListAsync()));

app.MapGet("/projects/{id:int}", async (int id, ProjectDbContext db) =>
{
    var project = await db.Projects.FindAsync(id);
    return project is null ? Results.NotFound() : Results.Ok(project);
});

app.MapGet("/projects/{id:int}/summary", async (
    int id,
    ProjectDbContext db,
    IHackathonClient hackathonClient,
    CancellationToken cancellationToken) =>
{
    var project = await db.Projects.FindAsync([id], cancellationToken);
    if (project is null)
    {
        return Results.NotFound();
    }

    var hackathon = await hackathonClient.GetHackathonAsync(project.HackathonId, cancellationToken);

    return Results.Ok(new
    {
        project,
        hackathon,
        note = hackathon is null
            ? "Hackathon details were unavailable, but the project data is still returned."
            : "Project and hackathon data were combined through an HTTP service call."
    });
});

app.MapPost("/projects", async (
    CreateProjectRequest request,
    ProjectDbContext db,
    IHackathonClient hackathonClient,
    CancellationToken cancellationToken) =>
{
    if (request.HackathonId <= 0 || string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { error = "A valid hackathonId and project name are required." });
    }

    var hackathon = await hackathonClient.GetHackathonAsync(request.HackathonId, cancellationToken);
    if (hackathon is null)
    {
        return Results.BadRequest(new
        {
            error = "The Hackathon Service could not confirm that this hackathon exists."
        });
    }

    var project = new HackathonProject
    {
        HackathonId = request.HackathonId,
        Name = request.Name.Trim(),
        RepoUrl = request.RepoUrl.Trim(),
        Stage = string.IsNullOrWhiteSpace(request.Stage) ? "Idea" : request.Stage.Trim(),
        Notes = request.Notes.Trim()
    };

    db.Projects.Add(project);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/projects/{project.Id}", project);
});

app.MapPut("/projects/{id:int}", async (
    int id,
    UpdateProjectRequest request,
    ProjectDbContext db,
    IHackathonClient hackathonClient,
    CancellationToken cancellationToken) =>
{
    var project = await db.Projects.FindAsync([id], cancellationToken);
    if (project is null)
    {
        return Results.NotFound();
    }

    if (request.HackathonId <= 0 || string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { error = "A valid hackathonId and project name are required." });
    }

    var hackathon = await hackathonClient.GetHackathonAsync(request.HackathonId, cancellationToken);
    if (hackathon is null)
    {
        return Results.BadRequest(new { error = "The Hackathon Service could not confirm the hackathon." });
    }

    project.HackathonId = request.HackathonId;
    project.Name = request.Name.Trim();
    project.RepoUrl = request.RepoUrl.Trim();
    project.Stage = request.Stage.Trim();
    project.Notes = request.Notes.Trim();

    await db.SaveChangesAsync(cancellationToken);
    return Results.Ok(project);
});

app.MapDelete("/projects/{id:int}", async (int id, ProjectDbContext db) =>
{
    var project = await db.Projects.FindAsync(id);
    if (project is null)
    {
        return Results.NotFound();
    }

    db.Projects.Remove(project);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
