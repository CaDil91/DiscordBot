namespace SteamServices;

public class SteamApp
{
    public int? AppId { get; set; }
    public string? Name { get; set; }
    public string HttpsLink { get; set; }
    public int PlayerCount { get; set; }
    
    
    // TODO: 
    /*private async Task<int> GetAppPlayerCountAsync(int sAppId)
    {
        // Query the api.steampowered.com with the given steamApps AppId.
        HttpResponseMessage sResponse = await _httpClient.SendAsync(new HttpRequestMessage(
            HttpMethod.Get,
            new Uri($"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={sAppId.ToString()}")
        ));

        // Safely convert sResponse to a JObject.
        JObject jResponse = JObject.Parse(await sResponse.Content.ReadAsStringAsync());

        // Safely collect "playerCount" from the response.
        return int.TryParse(jResponse["response"]?["player_count"]?.ToString(), out int iPlayerCount) ? iPlayerCount : 0;
    }*/
}