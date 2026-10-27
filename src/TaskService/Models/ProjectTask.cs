namespace TaskService.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Todo";
    public DateTime? DueDate { get; set; }
}

public record CreateTaskRequest(
    int ProjectId,
    string Title,
    string Priority,
    string Status,
    DateTime? DueDate);

public record UpdateTaskRequest(
    int ProjectId,
    string Title,
    string Priority,
    string Status,
    DateTime? DueDate);
