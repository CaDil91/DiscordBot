using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Repositories;

public class Webhook
{
    [Key] public string WebhookUrl { get; set; } = "";
}