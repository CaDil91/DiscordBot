using System.ComponentModel.DataAnnotations;

namespace SteamServices.Repositories;

public class Webhook
{
    [Key] public string WebhookUrl { get; set; } = "";
}