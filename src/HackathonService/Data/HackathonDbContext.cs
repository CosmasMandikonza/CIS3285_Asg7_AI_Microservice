using HackathonService.Models;
using Microsoft.EntityFrameworkCore;

namespace HackathonService.Data;

public class HackathonDbContext(DbContextOptions<HackathonDbContext> options) : DbContext(options)
{
    public DbSet<HackathonEvent> Hackathons => Set<HackathonEvent>();
}
