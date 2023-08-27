using Discord;
using Discord.WebSocket;
using DiscordBot.Controllers.Adapters;
using DiscordBot.Services;
using DiscordBot.Services.Commands;
using DiscordBot.Services.Commands.Factory;
using Microsoft.Extensions.Logging;
using SteamServices;

namespace DiscordBot.Controllers;

public class DiscordCommandController : ICommandController
{
    private readonly ILogger<DiscordCommandController> _logger;
    private readonly ICommandFactory _commandFactory;
    private const string STEAM_FOLLOW_CUSTOM_ID = "Steam_Follow";
    private const string STEAM_UNFOLLOW_CUSTOM_ID = "Steam_Unfollow";

    public DiscordCommandController(ILogger<DiscordCommandController> logger, ICommandFactory commandFactory)
    {
        _logger = logger;
        _commandFactory = commandFactory;
    }

    /// <summary>
    /// Verify SocketSlashCommand and send to wrapper/adapter.
    /// SocketSlashCommand is difficult to mock for unit testing. All private.
    /// </summary>
    /// <param name="socketSlashCommand">Discord.WebSocket.SocketSlashCommand</param>
    public async Task RunSlashCommandAsync(SocketSlashCommand socketSlashCommand)
    {
        // Validate
        if(!TryGetCommandAdapter(socketSlashCommand, out ICommandAdapter? commandAdapter) || commandAdapter == null)
        {
            _logger.LogError("Invalid slash command received.");
            return;
        };
        
        // Run
        BaseDiscordCommand command = _commandFactory.CreateCommand(commandAdapter.CommandName);
        command.SetAdapter(commandAdapter);
        await command.ExecuteAsync();
        
        // Respond
        /*foreach (DiscordResponse response in command.DiscordResponses) 
            await command.RespondAsync(response);*/
    }

    public async Task HandleButtonCommandAsync(SocketMessageComponent socketMessageComponent)
    {
        // Validate
        if(!TryGetCommandAdapter(socketMessageComponent, out ICommandAdapter? commandAdapter) || commandAdapter == null)
        {
            _logger.LogError("Invalid slash command received.");
            return;
        };
        
        // Run
        BaseDiscordCommand command = _commandFactory.CreateCommand(commandAdapter.CommandName);
        command.SetAdapter(commandAdapter);
        await command.ExecuteAsync();
        
        // Respond
        /*foreach (DiscordResponse response in command.DiscordResponses)
            await command.RespondAsync(response);*/
    }
    
    

    private bool TryGetCommandAdapter(SocketSlashCommand socketSlashCommand, out ICommandAdapter? commandAdapter)
    {
        commandAdapter = new SocketSlashCommandCommandAdapter(socketSlashCommand);
        if (string.IsNullOrEmpty(commandAdapter.CommandName))
        {
            _logger.LogError("Unable to get data from slash command.");
            return false;
        }
        return true;
    }
    
    private bool TryGetCommandAdapter(SocketMessageComponent socketSlashCommand, out ICommandAdapter? commandAdapter)
    {
        commandAdapter = new SocketMessageComponentAdapter(socketSlashCommand);
        if (string.IsNullOrEmpty(commandAdapter.CommandName))
        {
            _logger.LogError("Unable to get data from button command.");
            return false;
        }
        return true;
    }

    /*/// <summary>
    /// 
    /// </summary>
    /// <param name="socketSlashCommandCommandAdapter"></param>
    private async Task<List<DiscordResponse>> RunSlashCommandAsync(SocketSlashCommandCommandAdapter socketSlashCommandCommandAdapter)
    {
        SocketSlashCommandDataOption? dataOptions = socketSlashCommandCommandAdapter.Data?.Options.FirstOrDefault();
        
        List<SteamApp> steamApps = new();
        try
        {
            steamApps = await _steamService.GetAppsAsync(dataOptions?.Value.ToString() ?? string.Empty, 1);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error getting SteamApps.");
        }

        return steamApps.Select(CreateResponse).ToList();
    }*/

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
        ButtonBuilder unfollowButton = new()
        {
            Label = "Unfollow",
            Style = ButtonStyle.Danger,
            CustomId = STEAM_UNFOLLOW_CUSTOM_ID
        };
        
        ComponentBuilder componentBuilder = new();
        MessageComponent component = componentBuilder.WithButton(followButton).WithButton(unfollowButton).Build();

        var response = new DiscordResponse
        {
            Embeds = new List<Embed> { embed },
            MessageComponents = component
        };

        return response;
    }
}