using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=tasks.db";

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.EnsureCreated();

    if (!db.Tasks.Any())
    {
        db.Tasks.AddRange(
            new ProjectTask
            {
                ProjectId = 1,
                Title = "Finish API endpoints",
                Priority = "High",
                Status = "In Progress",
                DueDate = new DateTime(2026, 11, 10, 20, 0, 0)
            },
            new ProjectTask
            {
                ProjectId = 1,
                Title = "Record demo video",
                Priority = "High",
                Status = "Todo",
                DueDate = new DateTime(2026, 11, 14, 18, 0, 0)
            },
            new ProjectTask
            {
                ProjectId = 2,
                Title = "Sketch service boundaries",
                Priority = "Medium",
                Status = "Todo"
            });
        db.SaveChanges();
    }
}

app.MapGet("/", () => Results.Ok(new
{
    service = "Task Service",
    purpose = "Tracks tasks that belong to hackathon projects.",
    endpoints = new[] { "/tasks", "/tasks/{id}", "/tasks/project/{projectId}", "/health" }
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "tasks" }));

app.MapGet("/tasks", async (TaskDbContext db) =>
    Results.Ok(await db.Tasks.OrderBy(t => t.Id).ToListAsync()));

app.MapGet("/tasks/{id:int}", async (int id, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapGet("/tasks/project/{projectId:int}", async (int projectId, TaskDbContext db) =>
{
    var tasks = await db.Tasks
        .Where(t => t.ProjectId == projectId)
        .OrderBy(t => t.DueDate)
        .ToListAsync();

    return Results.Ok(tasks);
});

app.MapPost("/tasks", async (CreateTaskRequest request, TaskDbContext db) =>
{
    if (request.ProjectId <= 0 || string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "A valid projectId and task title are required." });
    }

    var task = new ProjectTask
    {
        ProjectId = request.ProjectId,
        Title = request.Title.Trim(),
        Priority = string.IsNullOrWhiteSpace(request.Priority) ? "Medium" : request.Priority.Trim(),
        Status = string.IsNullOrWhiteSpace(request.Status) ? "Todo" : request.Status.Trim(),
        DueDate = request.DueDate
    };

    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapPut("/tasks/{id:int}", async (int id, UpdateTaskRequest request, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null)
    {
        return Results.NotFound();
    }

    if (request.ProjectId <= 0 || string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { error = "A valid projectId and task title are required." });
    }

    task.ProjectId = request.ProjectId;
    task.Title = request.Title.Trim();
    task.Priority = request.Priority.Trim();
    task.Status = request.Status.Trim();
    task.DueDate = request.DueDate;

    await db.SaveChangesAsync();
    return Results.Ok(task);
});

app.MapDelete("/tasks/{id:int}", async (int id, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null)
    {
        return Results.NotFound();
    }

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
