using System.ComponentModel.DataAnnotations;

namespace SteamServices.MySteamDatabase;

public class FollowedSteamApp
{
    [Key]
    public int AppId { get; set; }

    public string Webhook { get; set; } = "";
    public DateTime? LastNewsCheckDate { get; set; }
}