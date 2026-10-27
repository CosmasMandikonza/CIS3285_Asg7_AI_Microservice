namespace ProjectService.Models;

public class HackathonProject
{
    public int Id { get; set; }
    public int HackathonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RepoUrl { get; set; } = string.Empty;
    public string Stage { get; set; } = "Idea";
    public string Notes { get; set; } = string.Empty;
}

public record CreateProjectRequest(
    int HackathonId,
    string Name,
    string RepoUrl,
    string Stage,
    string Notes);

public record UpdateProjectRequest(
    int HackathonId,
    string Name,
    string RepoUrl,
    string Stage,
    string Notes);
