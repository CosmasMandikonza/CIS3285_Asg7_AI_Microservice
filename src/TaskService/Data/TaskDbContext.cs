using Microsoft.EntityFrameworkCore;
using TaskService.Models;

namespace TaskService.Data;

public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<ProjectTask> Tasks => Set<ProjectTask>();
}
