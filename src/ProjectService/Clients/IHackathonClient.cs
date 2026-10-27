using ProjectService.Models;

namespace ProjectService.Clients;

public interface IHackathonClient
{
    Task<HackathonSummary?> GetHackathonAsync(int hackathonId, CancellationToken cancellationToken = default);
}
