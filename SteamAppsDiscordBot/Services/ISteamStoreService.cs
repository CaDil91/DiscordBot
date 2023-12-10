using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBot.Services;

public interface ISteamStoreService //Should rethink how I want to organize these services. This suggests I'd want  ISteamService, but is it going to be too generic?
{
    Task<List<SteamApp>> GetAppsAsync(string searchTerm, int iAppReturnCountMax);
}