using System.ComponentModel.DataAnnotations;

namespace SteamServices.MySteamDatabase;

public class Webhook
{
    [Key] public string WebhookUrl { get; set; } = "";
}