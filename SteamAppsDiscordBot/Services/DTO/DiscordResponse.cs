using Discord;

namespace SteamAppsDiscordBot.Services.DTO;

/// <summary>
/// Holds components for a Discord.Net response.
/// </summary>
public class DiscordResponse
{
    public List<Embed>? Embeds { get; set; }
    public MessageComponent? MessageComponents { get; set; }
    public string Message { get; set; } = "";
}