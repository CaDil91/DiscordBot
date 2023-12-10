using System.ComponentModel.DataAnnotations;

namespace SteamAppsDiscordBot.Repositories;

public class Webhook
{
    [Key] public string WebhookUrl { get; set; } = "";
}