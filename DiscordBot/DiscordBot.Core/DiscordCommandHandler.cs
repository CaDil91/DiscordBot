using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DiscordBot.Commands;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot.DiscordBot.Core;

public class DiscordCommandHandler : IDiscordCommandHandler
{
    private readonly ILogger<DiscordCommandHandler> _logger;
    private readonly IStoreService _steamService;
    private readonly DiscordSocketClient _discordClient;
    private const string STEAM_FOLLOW_CUSTOM_ID = "SteamFollow";

    public DiscordCommandHandler(ILogger<DiscordCommandHandler> logger, IStoreService steamService, DiscordSocketClient discordClient)
    {
        _logger = logger;
        _steamService = steamService;
        _discordClient = discordClient;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to wrapper/adapter.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="discordNetSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand? discordNetSlashCommand)
    {
        if (discordNetSlashCommand == null)
        {
            _logger.LogWarning("Null command received.");
            return;
        }

        var slashCommand = new SlashCommand(discordNetSlashCommand);
        if (!slashCommand.ValidateCommand()) return;
        
        await slashCommand.DeferAsync();
        List<DiscordResponse> discordResponses = await RunSlashCommandAsync(slashCommand);

        foreach (DiscordResponse response in discordResponses) await slashCommand.FollowupAsync
            (response.Message, embeds: response.Embeds?.ToArray() ?? null, components: response.MessageComponents ?? null);
    }

    public async Task HandleButtonAsync(SocketMessageComponent component)
    {
        switch(component.Data.CustomId)
        {
            case STEAM_FOLLOW_CUSTOM_ID:
                
                // Get message sent above this button.
                IMessage message = await component.Channel.GetMessageAsync(component.Message.Id);
                if (message == null) return;
        
                // Get embeds from message.
                IEmbed? embed = message.Embeds.FirstOrDefault();
        
                // Get title from embed.
                string? title = embed?.Title;
                if (title == null) return;

                var context = new SocketCommandContext(_discordClient, component.Message);
                IReadOnlyCollection<SocketRole>? roles = context.Guild.Roles;
                SocketRole? role = roles?.FirstOrDefault(r => r.Name == title);
                if (role == null || roles == null) return;

                await component.RespondAsync($"Guild name {context.Guild.Name} has roles {string.Join(", ", roles)}");
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slashCommand"></param>
    private async Task<List<DiscordResponse>> RunSlashCommandAsync(SlashCommand slashCommand)
    {
        SocketSlashCommandDataOption? test = slashCommand.Data?.Options.FirstOrDefault();
        
        List<SteamApp> steamApps = new();
        try
        {
            steamApps = await _steamService.GetAppsAsync(test?.Value.ToString() ?? string.Empty, 3);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error getting SteamApps.");
        }

        return steamApps.Select(CreateResponse).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="steamApp"></param>
    /// <returns></returns>
    private static DiscordResponse CreateResponse(SteamApp steamApp)
    {
        // Create embeds
        Embed? embed = new EmbedBuilder()
            .WithTitle(steamApp.Name)
            .WithUrl(steamApp.Url)
            .WithDescription(steamApp.ShortDescription)
            .WithImageUrl(steamApp.HeaderImage)
            .Build();
        if (embed == null) return new DiscordResponse { Message = "Error finding results"};

        ButtonBuilder followButton = new()
        {
            Label = "Follow",
            Style = ButtonStyle.Primary,
            CustomId = STEAM_FOLLOW_CUSTOM_ID
        };
        ComponentBuilder componentBuilder = new();
        MessageComponent component = componentBuilder.WithButton(followButton).Build();

        var response = new DiscordResponse
        {
            Embeds = new List<Embed> { embed },
            MessageComponents = component
        };

        return response;
    }
}

/// <summary>
/// Holds components for a Discord.Net response.
/// </summary>
internal class DiscordResponse
{
    public List<Embed>? Embeds { get; set; }
    public MessageComponent? MessageComponents { get; set; }
    public string Message { get; set; } = "";
}