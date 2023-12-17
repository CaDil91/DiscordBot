namespace SteamAppsDiscordBot;

/// <summary>
/// Represents the options for connecting to Google services.
/// </summary>
public class GoogleOptions
{
    /// <summary>
    /// The name of the section in the configuration file that contains the Google options.
    /// </summary>
    public const string SECTION_NAME = "Google";
    public string Key { get; set; } = string.Empty;
    public string SteamStoreCx { get; set; } = string.Empty;
}