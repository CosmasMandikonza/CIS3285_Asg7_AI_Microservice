using HackathonService.Data;
using HackathonService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=hackathons.db";

builder.Services.AddDbContext<HackathonDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HackathonDbContext>();
    db.Database.EnsureCreated();

    if (!db.Hackathons.Any())
    {
        db.Hackathons.AddRange(
            new HackathonEvent
            {
                Name = "Open Source Sprint",
                Organizer = "Campus Developer Club",
                Deadline = new DateTime(2026, 11, 15, 23, 59, 0),
                Location = "Virtual",
                Status = "Registered"
            },
            new HackathonEvent
            {
                Name = "Cloud Native Weekend",
                Organizer = "Community Tech Lab",
                Deadline = new DateTime(2026, 12, 5, 18, 0, 0),
                Location = "Minneapolis, MN",
                Status = "Planning"
            });
        db.SaveChanges();
    }
}

app.MapGet("/", () => Results.Ok(new
{
    service = "Hackathon Service",
    purpose = "Stores hackathon events and deadlines.",
    endpoints = new[] { "/hackathons", "/hackathons/{id}", "/health" }
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "hackathons" }));

app.MapGet("/hackathons", async (HackathonDbContext db) =>
    Results.Ok(await db.Hackathons.OrderBy(h => h.Deadline).ToListAsync()));

app.MapGet("/hackathons/{id:int}", async (int id, HackathonDbContext db) =>
{
    var hackathon = await db.Hackathons.FindAsync(id);
    return hackathon is null ? Results.NotFound() : Results.Ok(hackathon);
});

app.MapPost("/hackathons", async (CreateHackathonRequest request, HackathonDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { error = "Name is required." });
    }

    var hackathon = new HackathonEvent
    {
        Name = request.Name.Trim(),
        Organizer = request.Organizer.Trim(),
        Deadline = request.Deadline,
        Location = request.Location.Trim(),
        Status = string.IsNullOrWhiteSpace(request.Status) ? "Planning" : request.Status.Trim()
    };

    db.Hackathons.Add(hackathon);
    await db.SaveChangesAsync();

    return Results.Created($"/hackathons/{hackathon.Id}", hackathon);
});

app.MapPut("/hackathons/{id:int}", async (int id, UpdateHackathonRequest request, HackathonDbContext db) =>
{
    var hackathon = await db.Hackathons.FindAsync(id);
    if (hackathon is null)
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { error = "Name is required." });
    }

    hackathon.Name = request.Name.Trim();
    hackathon.Organizer = request.Organizer.Trim();
    hackathon.Deadline = request.Deadline;
    hackathon.Location = request.Location.Trim();
    hackathon.Status = request.Status.Trim();

    await db.SaveChangesAsync();
    return Results.Ok(hackathon);
});

app.MapDelete("/hackathons/{id:int}", async (int id, HackathonDbContext db) =>
{
    var hackathon = await db.Hackathons.FindAsync(id);
    if (hackathon is null)
    {
        return Results.NotFound();
    }

    db.Hackathons.Remove(hackathon);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
