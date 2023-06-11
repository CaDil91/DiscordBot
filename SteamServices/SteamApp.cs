using Newtonsoft.Json;

namespace SteamServices;

public class SteamApp
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("steam_appid")]
    public int SteamAppid { get; set; }

    [JsonProperty("header_image")]
    public string? HeaderImage { get; set; } = "https://store.akamai.steamstatic.com/public/shared/images/header/logo_steam.svg?t=962016";

    [JsonProperty("detailed_description")]
    public string? DetailedDescription { get; set; }
    
    [JsonProperty("about_the_game")]
    public string? AboutTheGame { get; set; }
    
    [JsonProperty("short_description")]
    public string? ShortDescription { get; set; }
    
    [JsonProperty("price_overview")]
    public PriceOverview? MyPriceOverview { get; set; }
    
    [JsonProperty("categories")]
    public List<Category>? Categories { get; set; }
    
    [JsonProperty("genres")]
    public List<Genre>? Genres { get; set; }
    
    [JsonProperty("screenshots")]
    public List<Screenshot>? Screenshots { get; set; }
    
    [JsonProperty("movies")]
    public List<Movie>? Movies { get; set; }

    /// <summary>
    /// The URL to the Steam Store page for this App.
    /// </summary>
    public string Url => $"https://store.steampowered.com/app/{SteamAppid}";

    public class SystemRequirements
    {
        [JsonProperty("minimum")]
        public string? Minimum { get; set; }
        
        [JsonProperty("recommended")]
        public string? Recommended { get; set; }
    }

    public class PriceOverview
    {
        [JsonProperty("currency")]
        public string? Currency { get; set; }
        
        [JsonProperty("initial")]
        public int Initial { get; set; }
        
        [JsonProperty("final")]
        public int Final { get; set; }
        
        [JsonProperty("discount_percent")]
        public int DiscountPercent { get; set; }
        
        [JsonProperty("initial_formatted")]
        public string? InitialFormatted { get; set; }
        
        [JsonProperty("final_formatted")]
        public string? FinalFormatted { get; set; }
    }

    public class PackageGroup
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        
        [JsonProperty("title")]
        public string? Title { get; set; }
        
        [JsonProperty("description")]
        public string? Description { get; set; }
        
        [JsonProperty("selection_text")]
        public string? SelectionText { get; set; }
        
        [JsonProperty("save_text")]
        public string? SaveText { get; set; }
        
        [JsonProperty("display_type")]
        public int DisplayType { get; set; }
        public string? IsRecurringSubscription { get; set; }
        public List<SubscriptionOption>? Subs { get; set; }
    }

    public class SubscriptionOption
    {
        public int Packageid { get; set; }
        public string? PercentSavingsText { get; set; }
        public int PercentSavings { get; set; }
        public string? OptionText { get; set; }
        public string? OptionDescription { get; set; }
        public string? CanGetFreeLicense { get; set; }
        public bool IsFreeLicense { get; set; }
        public int PriceInCentsWithDiscount { get; set; }
    }

    public class PlatformAvailability
    {
        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string? Description { get; set; }
    }

    public class Genre
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
    }

    public class Screenshot
    {
        public int Id { get; set; }
        public string? PathThumbnail { get; set; }
        public string? PathFull { get; set; }
    }

    public class Movie
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Thumbnail { get; set; }
        public Dictionary<string, string>? Webm { get; set; }
        public Dictionary<string, string>? Mp4 { get; set; }
        public bool Highlight { get; set; }
    }

    public class Achievement
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
    }

    public class Recommendations
    {
        public int Total { get; set; }
    }

    public class Achievements
    {
        public int Total { get; set; }
        public List<Achievement>? Highlighted { get; set; }
    }

    public class ReleaseDate
    {
        public bool ComingSoon { get; set; }
        public string? Date { get; set; }
    }

    public class SupportInfo
    {
        public string? Url { get; set; }
        public string? Email { get; set; }
    }

    public class ContentDescriptors
    {
        public List<int>? Ids { get; set; }
        public string? Notes { get; set; }
    }

    public class GameInfo
    {
        public List<Screenshot>? Screenshots { get; set; }
        public List<Movie>? Movies { get; set; }
        public Recommendations? Recommendations { get; set; }
        public Achievements? Achievements { get; set; }
        public ReleaseDate? ReleaseDate { get; set; }
        public SupportInfo? SupportInfo { get; set; }
        public string? Background { get; set; }
        public string? BackgroundRaw { get; set; }
        public ContentDescriptors? ContentDescriptors { get; set; }
    }
}