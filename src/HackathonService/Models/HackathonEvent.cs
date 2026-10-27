namespace HackathonService.Models;

public class HackathonEvent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Planning";
}

public record CreateHackathonRequest(
    string Name,
    string Organizer,
    DateTime Deadline,
    string Location,
    string Status);

public record UpdateHackathonRequest(
    string Name,
    string Organizer,
    DateTime Deadline,
    string Location,
    string Status);
