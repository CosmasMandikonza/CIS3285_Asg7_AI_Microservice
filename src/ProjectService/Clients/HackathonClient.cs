using System.Net;
using System.Net.Http.Json;
using ProjectService.Models;

namespace ProjectService.Clients;

public class HackathonClient(HttpClient httpClient, ILogger<HackathonClient> logger) : IHackathonClient
{
    public async Task<HackathonSummary?> GetHackathonAsync(
        int hackathonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync($"/hackathons/{hackathonId}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HackathonSummary>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Hackathon Service could not be reached for hackathon {HackathonId}.", hackathonId);
            return null;
        }
    }
}
