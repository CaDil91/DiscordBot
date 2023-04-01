using System.Diagnostics;
using System.Net.Sockets;
using System.Reactive.Subjects;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DiscordBot;

public class DiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly IOptions<DiscordBotOptions> _discordOptions;

    public DiscordBot(IOptions<DiscordBotOptions> discordOptions)
    {
        //When working with events that have Cacheable<IMessage, ulong> parameters, you must enable the message cache in your config settings if you plan to use the cached message entity.
        var discordSocketConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = new DiscordSocketClient(discordSocketConfig);
        _discordOptions = discordOptions;
    }

    public async Task RunAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _discordOptions.Value.DiscordToken);
        await _client.StartAsync();

        //Register commands is a one time operation. Close bot after this. Try to let bot connect 3 times.
        if (_discordOptions.Value.RegisterSlashCommands)
        {
            Thread.Sleep(6000); //Give time for bot to connect.
            await RegisterSlashCommands("steam", "Search the steam store");
            return; //Do not stay connected
        }

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);
    }

    /// <summary>
    /// Register slash commands.
    /// </summary>
    /// <param name="sName"></param>
    /// <param name="sDescription"></param>
    /// <exception>Throws if _client has no guilds</exception>
    public async Task RegisterSlashCommands(string sName, string sDescription = "")
    {
        //Create slash commands
        var guildCommand = new SlashCommandBuilder();
        guildCommand.WithName(sName); //Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        guildCommand.WithDescription(sDescription); //Descriptions can have a max length of 100.

        //Create slash command.
        SocketGuild? socketGuild = _client.Guilds?.FirstOrDefault();
        if (socketGuild == null) throw new Exception("Discord bot is not connected any guilds");
        
        SocketGuild guild = _client.GetGuild(socketGuild.Id);
        await guild.CreateApplicationCommandAsync(guildCommand.Build());
    }

    /// <summary>
    ///     Check if DiscordBotSocketClient.ConnectionState.Connected == True.
    /// </summary>
    /// <returns>bool</returns>
    public bool IsConnected()
    {
        return _client.ConnectionState == ConnectionState.Connected;
    }
}