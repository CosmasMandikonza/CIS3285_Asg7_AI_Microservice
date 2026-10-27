namespace ProjectService.Models;

public record HackathonSummary(
    int Id,
    string Name,
    string Organizer,
    DateTime Deadline,
    string Location,
    string Status);
